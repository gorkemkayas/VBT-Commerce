'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { cartService } from '../../../services/cart.service';
import { priceService } from '../../../services/price.service';
import { apiClient } from '@/lib/api-client';

export default function CartPage() {
  const router = useRouter();
  const [cartItems, setCartItems] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  // Sepeti ve detaylarını yükle
  const loadCart = async () => {
    setLoading(true);
    try {
      const cart = await cartService.getMyCart();
      
      if (cart && Array.isArray(cart.items) && cart.items.length > 0) {
        const enrichedItems = await Promise.all(
          cart.items.map(async (item: any) => {
            let name = 'Lüks Ürün';
            let imageUrl = null;
            let sizeName = '';
            let price = 0;

            const itemType = item.sellableItemType ?? 0; // 0: Product, 1: Variant
            const itemId = item.sellableItemId;

            try {
              // 1. Fiyat Bilgisini Çek (GET /api/prices/{type}/{id})
              price = await priceService.getPrice(itemType, itemId);

              // 2. Ürün / Varyant Detayını Çek
              if (itemType === 0) {
                // Ana Ürün
                const prodRes = await apiClient.get(`/api/products/${itemId}`).catch(() => null);
                if (prodRes?.data) {
                  name = prodRes.data.name;
                  imageUrl = prodRes.data.images?.find((img: any) => img.isPrimary)?.url || prodRes.data.images?.[0]?.url || null;
                }
              } else {
                // Varyantlı Ürün ise: Tüm ürünleri taramak yerine filtreliyoruz
                const productsRes = await apiClient.get('/api/products?pageSize=100').catch(() => null);
                const products = productsRes?.data?.items || [];

                for (const prod of products) {
                  const detailRes = await apiClient.get(`/api/products/${prod.id}`).catch(() => null);
                  const prodDetail = detailRes?.data;
                  const matchedVariant = prodDetail?.variants?.find((v: any) => v.id === itemId);

                  if (matchedVariant) {
                    name = prodDetail.name;
                    sizeName = matchedVariant.optionValues?.[0]?.value || matchedVariant.sku || '';
                    imageUrl = prodDetail.images?.find((img: any) => img.isPrimary)?.url || prodDetail.images?.[0]?.url || null;
                    break;
                  }
                }
              }
            } catch (err) {
              console.error('Sepet ürünü detaylandırma hatası:', err);
            }

            return {
              ...item,
              name,
              sizeName,
              imageUrl,
              price: price > 0 ? price : 0,
              quantity: item.quantity || 1,
            };
          })
        );
        setCartItems(enrichedItems);
      } else {
        setCartItems([]);
      }
    } catch (error) {
      console.error('Sepet yükleme hatası:', error);
      setCartItems([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCart();
  }, []);

  // Miktar Güncelleme (PUT /api/carts/me/items/{itemId})
  const handleUpdateQuantity = async (cartItemId: string, currentQty: number, delta: number) => {
    const newQty = currentQty + delta;
    if (newQty < 1) return;

    // Fast UI Update
    setCartItems((prev) =>
      prev.map((item) => (item.id === cartItemId ? { ...item, quantity: newQty } : item))
    );

    try {
      // Backend Sepet Miktarını Güncelle
      await apiClient.put(`/api/carts/me/items/${cartItemId}`, { quantity: newQty });
    } catch (err) {
      console.error('Miktar backend tarafında güncellenemedi:', err);
      // Hata durumunda sepeti yeniden çek
      loadCart();
    }
  };

  // Ürün Silme (DELETE /api/carts/me/items/{itemId})
  const handleRemove = async (cartItemId: string) => {
    try {
      await cartService.removeItem(cartItemId);
      await loadCart();
    } catch (error) {
      console.error('Silme hatası:', error);
    }
  };

  const totalItemCount = cartItems.reduce((sum, item) => sum + (item.quantity || 1), 0);
  const totalPrice = cartItems.reduce((sum, item) => sum + item.price * (item.quantity || 1), 0);

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fafafa', color: '#666', fontFamily: 'sans-serif' }}>
        Sepetiniz yükleniyor...
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#fafafa', color: '#111', fontFamily: 'system-ui, -apple-system, sans-serif' }}>
      
      {/* Header */}
      <header style={{ borderBottom: '1px solid #eaeaea', backgroundColor: '#fff' }}>
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '1rem 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Link href="/shop/home" style={{ fontSize: '1.25rem', fontWeight: '700', letterSpacing: '0.1em', textTransform: 'uppercase', textDecoration: 'none', color: '#000' }}>
            LUXE STORE
          </Link>
          <Link href="/shop/home" style={{ fontSize: '0.85rem', color: '#666', textDecoration: 'none' }}>
            ← Alışverişe Dön
          </Link>
        </div>
      </header>

      {/* Main Container */}
      <main style={{ maxWidth: '1100px', margin: '0 auto', padding: '3rem 2rem' }}>
        
        <p style={{ color: '#666', fontSize: '0.95rem', marginBottom: '1.5rem', fontWeight: '400' }}>
          {totalItemCount} {totalItemCount === 1 ? 'ürün' : 'ürün'} sepetinizde
        </p>

        {cartItems.length === 0 ? (
          <div style={{ textAlign: 'center', padding: '4rem 0', backgroundColor: '#fff', borderRadius: '8px', border: '1px solid #eee' }}>
            <p style={{ color: '#666', marginBottom: '1.5rem' }}>Sepetinizde henüz ürün bulunmuyor.</p>
            <Link href="/shop/home" style={{ display: 'inline-block', backgroundColor: '#111', color: '#fff', padding: '0.8rem 2rem', borderRadius: '4px', textDecoration: 'none', fontSize: '0.85rem', fontWeight: '600' }}>
              KOLEKSİYONU KEŞFEDİN
            </Link>
          </div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 360px', gap: '3.5rem', alignItems: 'start' }}>
            
            {/* SOL KOLON: Ürün Listesi */}
            <div style={{ display: 'flex', flexDirection: 'column', gap: '2rem' }}>
              {cartItems.map((item: any) => {
                const cartItemId = item.id; // CartItem UUID

                return (
                  <div key={cartItemId} style={{ borderBottom: '1px solid #eee', paddingBottom: '1.5rem' }}>
                    <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'flex-start' }}>
                      
                      {/* Ürün Görseli */}
                      <div style={{ width: '100px', height: '110px', backgroundColor: '#e5e5e5', flexShrink: 0, overflow: 'hidden', borderRadius: '4px' }}>
                        {item.imageUrl ? (
                          <img src={item.imageUrl} alt={item.name} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                        ) : null}
                      </div>

                      {/* Ürün Detayları */}
                      <div style={{ flexGrow: 1 }}>
                        <h3 style={{ fontSize: '1.1rem', fontWeight: '600', margin: '0 0 0.25rem 0', color: '#111' }}>
                          {item.name}
                        </h3>
                        
                        <p style={{ fontSize: '0.8rem', color: '#777', textTransform: 'uppercase', letterSpacing: '0.05em', margin: '0 0 1rem 0', fontWeight: '500' }}>
                          {item.sizeName ? `BEDEN: ${item.sizeName}` : 'STANDART'}
                        </p>

                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                          <span style={{ fontSize: '1.1rem', fontWeight: '700', color: '#111' }}>
                            ₺{(item.price * item.quantity).toFixed(2)}
                          </span>

                          {/* Stepper */}
                          <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                            <button
                              type="button"
                              onClick={() => handleUpdateQuantity(cartItemId, item.quantity, -1)}
                              style={{ width: '32px', height: '32px', border: '1px solid #ccc', backgroundColor: '#fff', fontSize: '1rem', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#333', borderRadius: '4px' }}
                            >
                              −
                            </button>
                            <span style={{ fontSize: '0.9rem', fontWeight: '600', minWidth: '16px', textAlign: 'center' }}>
                              {item.quantity}
                            </span>
                            <button
                              type="button"
                              onClick={() => handleUpdateQuantity(cartItemId, item.quantity, 1)}
                              style={{ width: '32px', height: '32px', border: '1px solid #ccc', backgroundColor: '#fff', fontSize: '1rem', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#333', borderRadius: '4px' }}
                            >
                              +
                            </button>
                          </div>
                        </div>
                      </div>

                    </div>

                    {/* REMOVE Butonu */}
                    <div style={{ marginTop: '0.75rem' }}>
                      <button 
                        type="button"
                        onClick={() => handleRemove(cartItemId)} 
                        style={{ background: 'none', border: 'none', color: '#dc2626', fontSize: '0.8rem', fontWeight: '600', letterSpacing: '0.05em', textTransform: 'uppercase', cursor: 'pointer', padding: 0 }}
                      >
                        SİL
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>

            {/* SAĞ KOLON: Sipariş Özeti */}
            <div style={{ backgroundColor: '#fff', border: '1px solid #eee', borderRadius: '8px', padding: '2rem', position: 'sticky', top: '2rem' }}>
              <h2 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: '1.5rem', borderBottom: '1px solid #eee', paddingBottom: '0.75rem' }}>
                Sipariş Özeti
              </h2>

              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1rem', fontSize: '0.9rem', color: '#555' }}>
                <span>Ara Toplam</span>
                <span>₺{totalPrice.toFixed(2)}</span>
              </div>

              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1.5rem', fontSize: '0.9rem', color: '#555' }}>
                <span>Kargo</span>
                <span style={{ color: '#16a34a', fontWeight: '600' }}>Ödeme Adımında Hesaplanır</span>
              </div>

              <hr style={{ border: 'none', borderTop: '1px solid #eee', margin: '1rem 0' }} />

              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '2rem', fontSize: '1.2rem', fontWeight: '700', color: '#111' }}>
                <span>Toplam</span>
                <span>₺{totalPrice.toFixed(2)}</span>
              </div>

              <button 
                type="button"
                onClick={() => router.push('/shop/checkout')}
                style={{ width: '100%', backgroundColor: '#111', color: '#fff', padding: '1.1rem', border: 'none', borderRadius: '4px', fontWeight: '700', letterSpacing: '0.1em', textTransform: 'uppercase', cursor: 'pointer', transition: 'background 0.2s' }}
              >
                ÖDEMEYE GEÇ
              </button>
            </div>

          </div>
        )}
      </main>
    </div>
  );
}