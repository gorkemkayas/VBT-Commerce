'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { apiClient } from '@/lib/api-client';
import { ProductListItemDto } from '@/types/api';

interface Category {
  id: string;
  name: string;
  slug: string;
}

interface ProductWithPrice extends ProductListItemDto {
  price?: number;
}

export default function HomePage() {
  const router = useRouter();
  const [products, setProducts] = useState<ProductWithPrice[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  // 1. Kategorileri Çek
  useEffect(() => {
    apiClient
      .get('/api/categories/tree')
      .then((res) => {
        setCategories(res.data || []);
      })
      .catch((err) => {
        console.error('Kategoriler çekilemedi:', err);
      });
  }, []);

  // 2. Ürünleri, Varyantları ve Fiyatları Akıllıca Çek
  useEffect(() => {
    setLoading(true);

    let url = '/api/products?isActive=true';
    if (selectedCategoryId) {
      url += `&categoryId=${selectedCategoryId}`;
    }

    apiClient
      .get(url)
      .then(async (res) => {
        const items: ProductListItemDto[] = res.data?.items || (Array.isArray(res.data) ? res.data : []);
        
        // Pasif ürünleri temizle
        const activeOnly = items.filter((p: any) => p.isActive !== false);

        // Her ürün için Fiyat Mantığını Çalıştır (Product mı Variant mı?)
        const productsWithPrices = await Promise.all(
          activeOnly.map(async (product) => {
            let finalPrice = 0;

            // ADIM 1: Önce Ana Ürün Fiyatını Dene (sellableItemType = 0)
            try {
              const priceRes = await apiClient.get(`/api/prices/0/${product.id}`);
              if (priceRes.data?.amount && priceRes.data.amount > 0) {
                finalPrice = priceRes.data.amount;
              }
            } catch {
              /* Ana ürün fiyatı yoksa pas geç, varyanta bakacağız */
            }

            // ADIM 2: Eğer Ana Ürün Fiyatı Bulunamadıysa Varyant Fiyatına Bak (sellableItemType = 1)
            if (finalPrice === 0) {
              try {
                // Ürün detayını çekerek varyant ID'lerini öğreniyoruz
                const detailRes = await apiClient.get(`/api/products/${product.id}`);
                const variants = detailRes.data?.variants || [];
                const firstVariant = variants.find((v: any) => v.isActive !== false) || variants[0];

                if (firstVariant?.id) {
                  const variantPriceRes = await apiClient.get(`/api/prices/1/${firstVariant.id}`);
                  if (variantPriceRes.data?.amount) {
                    finalPrice = variantPriceRes.data.amount;
                  }
                }
              } catch {
                /* Varyant fiyatı da bulunamazsa 0 kalır */
              }
            }

            return {
              ...product,
              price: finalPrice,
            };
          })
        );

        setProducts(productsWithPrices);
      })
      .catch(() => {
        setProducts([]);
      })
      .finally(() => {
        setLoading(false);
      });
  }, [selectedCategoryId]);

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#fafafa', color: '#111', fontFamily: 'system-ui, -apple-system, sans-serif' }}>
      
      {/* Navigation Header */}
      <header style={{ borderBottom: '1px solid #eaeaea', backgroundColor: '#fff', position: 'sticky', top: 0, zIndex: 10 }}>
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '1rem 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Link href="/shop/home" style={{ fontSize: '1.25rem', fontWeight: '700', letterSpacing: '0.1em', textTransform: 'uppercase', textDecoration: 'none', color: '#000' }}>
            LUXE STORE
          </Link>
          <nav style={{ display: 'flex', gap: '2rem', fontSize: '0.9rem', fontWeight: '500' }}>
            <Link href="/shop/home" style={{ color: '#000', textDecoration: 'none' }}>Koleksiyonlar</Link>
            <Link href="/shop/cart" style={{ color: '#666', textDecoration: 'none' }}>Sepet</Link>
            <Link href="/shop/profile" style={{ color: '#666', textDecoration: 'none' }}>Hesabım</Link>
          </nav>
        </div>
      </header>

      {/* Hero Banner Section */}
      <section style={{ backgroundColor: '#111', color: '#fff', padding: '4rem 2rem', textAlign: 'center' }}>
        <p style={{ fontSize: '0.8rem', letterSpacing: '0.2em', textTransform: 'uppercase', color: '#aaa', marginBottom: '0.5rem' }}>
          Yeni Sezon 2026
        </p>
        <h1 style={{ fontSize: '2.5rem', fontWeight: '300', letterSpacing: '0.05em', marginBottom: '1rem' }}>
          MİNİMALİST & LÜKS TASARIMLAR
        </h1>
        <p style={{ color: '#ccc', maxWidth: '600px', margin: '0 auto', fontSize: '0.95rem' }}>
          Zamansız parçalar, yüksek kaliteli kumaşlar ve modern kesimlerle stilinizi baştan tanımlayın.
        </p>
      </section>

      {/* Main Content Section */}
      <main style={{ maxWidth: '1200px', margin: '0 auto', padding: '3rem 2rem' }}>
        
        {/* KATEGORİ FİLTRELEME BARI */}
        <div style={{ marginBottom: '2.5rem' }}>
          <h3 style={{ fontSize: '0.85rem', textTransform: 'uppercase', letterSpacing: '0.1em', color: '#888', marginBottom: '1rem' }}>
            Kategorilere Göre Filtrele
          </h3>
          <div style={{ display: 'flex', gap: '0.75rem', flexWrap: 'wrap', alignItems: 'center' }}>
            <button
              onClick={() => setSelectedCategoryId(null)}
              style={{
                padding: '0.6rem 1.2rem',
                borderRadius: '20px',
                border: selectedCategoryId === null ? '1px solid #111' : '1px solid #e5e7eb',
                backgroundColor: selectedCategoryId === null ? '#111' : '#fff',
                color: selectedCategoryId === null ? '#fff' : '#4b5563',
                fontSize: '0.85rem',
                fontWeight: '500',
                cursor: 'pointer',
                transition: 'all 0.2s ease',
              }}
            >
              Tüm Ürünler
            </button>

            {categories.map((cat) => {
              const isSelected = selectedCategoryId === cat.id;
              return (
                <button
                  key={cat.id}
                  onClick={() => setSelectedCategoryId(cat.id)}
                  style={{
                    padding: '0.6rem 1.2rem',
                    borderRadius: '20px',
                    border: isSelected ? '1px solid #111' : '1px solid #e5e7eb',
                    backgroundColor: isSelected ? '#111' : '#fff',
                    color: isSelected ? '#fff' : '#4b5563',
                    fontSize: '0.85rem',
                    fontWeight: '500',
                    cursor: 'pointer',
                    transition: 'all 0.2s ease',
                  }}
                >
                  {cat.name}
                </button>
              );
            })}
          </div>
        </div>

        {/* Katalog Başlık ve Sayacı */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', borderBottom: '1px solid #eee', paddingBottom: '0.75rem' }}>
          <h2 style={{ fontSize: '1.4rem', fontWeight: '400', letterSpacing: '0.05em' }}>
            {selectedCategoryId 
              ? `${categories.find(c => c.id === selectedCategoryId)?.name || 'Kategori'} Ürünleri`
              : 'Öne Çıkanlar'}
          </h2>
          <span style={{ fontSize: '0.85rem', color: '#666' }}>{products.length} Ürün Gösteriliyor</span>
        </div>

        {/* Ürün Listesi */}
        {loading ? (
          <div style={{ textAlign: 'center', padding: '4rem 0', color: '#888' }}>
            Koleksiyon ve fiyatlar yükleniyor...
          </div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))', gap: '2rem' }}>
            {Array.isArray(products) && products.length > 0 ? (
              products.map((product) => {
                const productSlug = product.slug || product.id;
                const priceValue = product.price ?? 0;
                const formattedPrice = Number(priceValue).toFixed(2);
                const imageUrl = product.primaryImageUrl || null;

                return (
                  <div
                    key={product.id || productSlug}
                    onClick={() => router.push(`/shop/products/${productSlug}`)}
                    style={{
                      backgroundColor: '#fff',
                      border: '1px solid #eee',
                      borderRadius: '4px',
                      overflow: 'hidden',
                      display: 'flex',
                      flexDirection: 'column',
                      justifyContent: 'space-between',
                      cursor: 'pointer',
                      transition: 'transform 0.2s, box-shadow 0.2s',
                    }}
                  >
                    <div style={{ width: '100%', height: '320px', backgroundColor: '#f0f0f0', display: 'flex', alignItems: 'center', justifyContent: 'center', overflow: 'hidden' }}>
                      {imageUrl ? (
                        <img src={imageUrl} alt={product.name} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                      ) : (
                        <span style={{ color: '#aaa', fontSize: '0.85rem' }}>Görsel Yok</span>
                      )}
                    </div>
                    <div style={{ padding: '1.25rem', display: 'flex', flexDirection: 'column', flexGrow: 1 }}>
                      <span style={{ fontSize: '0.75rem', color: '#888', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '0.25rem' }}>
                        Lüks Koleksiyon
                      </span>
                      <h3 style={{ fontSize: '1rem', fontWeight: '500', marginBottom: '0.5rem', color: '#111' }}>
                        {product.name}
                      </h3>
                      <div style={{ marginTop: 'auto', paddingTop: '0.5rem' }}>
                        <span style={{ fontSize: '1.1rem', fontWeight: '600', color: '#111' }}>
                          ₺{formattedPrice}
                        </span>
                      </div>
                    </div>
                  </div>
                );
              })
            ) : (
              <div style={{ color: '#666', gridColumn: 'span 3', padding: '2rem 0' }}>
                Bu kategoride henüz ürün bulunmamaktadır.
              </div>
            )}
          </div>
        )}
      </main>
    </div>
  );
}