'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams } from 'next/navigation';
import { productService } from '../../../../services/product.service';
import { cartService } from '../../../../services/cart.service';
import { stockService } from '../../../../services/stock.service';
import { priceService } from '../../../../services/price.service';
import { Product } from '@/types/api';

// Bedenlerin standart görünüm sırası
const SIZE_ORDER = ['XS', 'S', 'M', 'L', 'XL', '2XL', '3XL'];

export default function ProductDetailPage() {
  const params = useParams();
  const slug = params?.slug as string;

  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedVariantId, setSelectedVariantId] = useState<string>('');
  const [currentPrice, setCurrentPrice] = useState<number>(0);
  const [availableStock, setAvailableStock] = useState<number | null>(null);
  const [added, setAdded] = useState(false);
  const [cartCount, setCartCount] = useState(0);

  const refreshCartCount = async () => {
    const cart = await cartService.getMyCart();
    if (cart && Array.isArray(cart.items)) {
      const total = cart.items.reduce((sum: number, item: any) => sum + (item.quantity || 1), 0);
      setCartCount(total);
    }
  };

  // 1. Ürün Verisini Çekme
  useEffect(() => {
    if (slug) {
      productService.getProductBySlug(slug).then((data) => {
        setProduct(data);

        const prodDetail = data as any;
        if (prodDetail?.variants && prodDetail.variants.length > 0) {
          setSelectedVariantId(prodDetail.variants[0].id);
        } else if (data?.id) {
          setSelectedVariantId(data.id);
        }

        if (data?.price) {
          setCurrentPrice(data.price);
        }

        setLoading(false);
      });
    }
    refreshCartCount();
  }, [slug]);

  // 2. Seçili Varyant Değiştiğinde Hem Stok Hem Fiyat Sorgulama
  useEffect(() => {
    if (selectedVariantId) {
      // Varyanta Özel Stok Sorgusu (SellableItemType = 1)
      stockService.getStockBySellableItem(selectedVariantId, 1).then((stockData) => {
        if (stockData) {
          setAvailableStock(stockData.availableQuantity);
        } else {
          setAvailableStock(0);
        }
      });

      // Varyanta Özel Fiyat Sorgusu (SellableItemType = 1)
      priceService.getPrice(1, selectedVariantId).then((price) => {
        if (price > 0) {
          setCurrentPrice(price);
        } else if (product?.price) {
          setCurrentPrice(product.price);
        }
      });
    }
  }, [selectedVariantId, product]);

  const handleAddToCart = async () => {
    if (!product || availableStock === 0) return;

    try {
      await cartService.addItem({
        sellableItemId: selectedVariantId,
        sellableItemType: 1,
        quantity: 1,
      });

      setAdded(true);
      await refreshCartCount();
      setTimeout(() => setAdded(false), 2000);
    } catch (error: any) {
      const serverError = JSON.stringify(error?.response?.data) || error?.message;
      alert(`[HATA]: ${serverError}`);
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fafafa', color: '#888', fontSize: '0.9rem', fontFamily: 'sans-serif' }}>
        Ürün detayları yükleniyor...
      </div>
    );
  }

  const formattedPrice = Number(currentPrice).toFixed(2);

  const imageUrl = product?.images && product.images.length > 0 
    ? (product.images.find((img) => img.isPrimary)?.url || product.images[0].url)
    : null;

  const prodDetail = product as any;
  const rawVariants = prodDetail?.variants || [];

  // Varyantları S, M, L, XL sırasına göre sıralama
  const sortedVariants = [...rawVariants].sort((a: any, b: any) => {
    const sizeA = (a.optionValues?.[0]?.value || a.sku?.split('-').pop() || '').toUpperCase();
    const sizeB = (b.optionValues?.[0]?.value || b.sku?.split('-').pop() || '').toUpperCase();

    const indexA = SIZE_ORDER.indexOf(sizeA);
    const indexB = SIZE_ORDER.indexOf(sizeB);

    return (indexA === -1 ? 99 : indexA) - (indexB === -1 ? 99 : indexB);
  });

  const isOutOfStock = availableStock === 0;

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#fafafa', color: '#111', fontFamily: 'system-ui, -apple-system, sans-serif' }}>
      
      {/* Header */}
      <header style={{ borderBottom: '1px solid #eaeaea', backgroundColor: '#fff', position: 'sticky', top: 0, zIndex: 10 }}>
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '1rem 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Link href="/shop/home" style={{ fontSize: '1.25rem', fontWeight: '700', letterSpacing: '0.1em', textTransform: 'uppercase', textDecoration: 'none', color: '#000' }}>
            LUXE STORE
          </Link>
          <nav style={{ display: 'flex', gap: '2rem', fontSize: '0.9rem', fontWeight: '500' }}>
            <Link href="/shop/home" style={{ color: '#000', textDecoration: 'none' }}>Koleksiyonlar</Link>
            <Link href="/shop/cart" style={{ color: '#111', fontWeight: '600', textDecoration: 'none' }}>
              Sepet ({cartCount})
            </Link>
            <Link href="/auth/login" style={{ color: '#666', textDecoration: 'none' }}>Hesabım</Link>
          </nav>
        </div>
      </header>

      {/* Main Content */}
      <main style={{ maxWidth: '1200px', margin: '0 auto', padding: '1.5rem 2rem 4rem' }}>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(350px, 1fr))', gap: '4rem', alignItems: 'start' }}>
          
          {/* Görsel Alanı */}
          <div style={{ width: '100%', height: '550px', backgroundColor: '#f0f0f0', borderRadius: '4px', overflow: 'hidden', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            {imageUrl ? (
              <img src={imageUrl} alt={product?.name || 'Ürün Görseli'} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
            ) : (
              <span style={{ color: '#aaa', fontSize: '0.9rem' }}>Görsel Bulunmuyor</span>
            )}
          </div>

          {/* Ürün Bilgileri ve Seçenekler */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', paddingTop: '1rem' }}>
            <div>
              <span style={{ fontSize: '0.8rem', color: '#888', textTransform: 'uppercase', letterSpacing: '0.1em' }}>
                {product?.categoryName || 'Lüks Koleksiyon'}
              </span>
              <h1 style={{ fontSize: '2rem', fontWeight: '400', margin: '0.4rem 0 0.8rem', color: '#111' }}>
                {product?.name || 'Ürün Adı'}
              </h1>
              <p style={{ fontSize: '1.6rem', fontWeight: '600', color: '#111', margin: 0 }}>
                ₺{formattedPrice}
              </p>
            </div>

            <hr style={{ border: 'none', borderTop: '1px solid #eee', margin: '0.5rem 0' }} />

            {/* Beden / Varyant Listesi (Sıralı) */}
            {sortedVariants.length > 0 && (
              <div>
                <p style={{ fontSize: '0.8rem', fontWeight: '600', textTransform: 'uppercase', letterSpacing: '0.05em', color: '#555', marginBottom: '0.75rem' }}>
                  Beden Seçin:
                </p>
                <div style={{ display: 'flex', gap: '0.75rem', flexWrap: 'wrap' }}>
                  {sortedVariants.map((v: any) => {
                    const isSelected = selectedVariantId === v.id;
                    const sizeLabel = v.optionValues?.[0]?.value || v.sku?.split('-').pop() || 'Standart';

                    return (
                      <button
                        key={v.id}
                        type="button"
                        onClick={() => setSelectedVariantId(v.id)}
                        style={{
                          padding: '0.8rem 1.4rem',
                          fontSize: '0.85rem',
                          fontWeight: '600',
                          borderRadius: '6px',
                          cursor: 'pointer',
                          transition: 'all 0.25s ease-in-out',
                          backgroundColor: isSelected ? '#000000' : '#ffffff',
                          color: isSelected ? '#ffffff' : '#333333',
                          border: isSelected ? '2px solid #000000' : '1px solid #e5e7eb',
                          boxShadow: isSelected 
                            ? '0 0 12px rgba(0, 0, 0, 0.25), 0 2px 4px rgba(0,0,0,0.1)' 
                            : 'none',
                          transform: isSelected ? 'scale(1.03)' : 'scale(1)',
                        }}
                      >
                        {sizeLabel}
                      </button>
                    );
                  })}
                </div>
              </div>
            )}

            {/* Stok Bilgisi Durumu (Adet Bilgisi Kaldırıldı) */}
            {availableStock !== null && (
              <p style={{ fontSize: '0.85rem', color: isOutOfStock ? '#dc2626' : '#16a34a', fontWeight: '600', margin: 0 }}>
                {isOutOfStock ? '⚠️ Stok Tükendi' : '✓ Stokta Var'}
              </p>
            )}

            {/* Sepete Ekle Butonu */}
            <div style={{ marginTop: '0.5rem' }}>
              <button
                type="button"
                disabled={isOutOfStock}
                onClick={handleAddToCart}
                style={{
                  width: '100%',
                  padding: '1.1rem',
                  backgroundColor: isOutOfStock ? '#d1d5db' : (added ? '#16a34a' : '#111'),
                  color: isOutOfStock ? '#6b7280' : '#fff',
                  border: 'none',
                  borderRadius: '4px',
                  fontSize: '0.85rem',
                  fontWeight: '700',
                  letterSpacing: '0.15em',
                  textTransform: 'uppercase',
                  cursor: isOutOfStock ? 'not-allowed' : 'pointer',
                  transition: 'background-color 0.2s'
                }}
              >
                {isOutOfStock ? 'STOKTA YOK' : (added ? 'SEPETE EKLENDİ ✓' : 'SEPETE EKLE')}
              </button>
            </div>

          </div>

        </div>
      </main>

    </div>
  );
}