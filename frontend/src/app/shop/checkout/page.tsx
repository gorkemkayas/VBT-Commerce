'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { apiClient } from '@/lib/api-client';
import { authService } from '@/services/auth.service';
import { checkoutService } from '@/services/checkout.service';
import { CustomerAddressDto } from '@/types/api';

interface ShippingCompany {
  id: string;
  name: string;
  fee: number;
  isActive: boolean;
}

export default function CheckoutPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  // Adresler ve Kargo
  const [addresses, setAddresses] = useState<CustomerAddressDto[]>([]);
  const [selectedAddressId, setSelectedAddressId] = useState<string>('');
  const [shippingCompanies, setShippingCompanies] = useState<ShippingCompany[]>([]);
  const [selectedShippingCompanyId, setSelectedShippingCompanyId] = useState<string>('');

  const [couponCode, setCouponCode] = useState<string>('');

  // Ödeme Tipi Sekmesi: 'saved' (Kayıtlı Kart) veya 'manual' (Kredi / Banka Kartı)
  const [paymentTab, setPaymentTab] = useState<'saved' | 'manual'>('manual');

  // Kayıtlı Kartlar Listesi
  const [savedCards] = useState([
    { 
      id: '1', 
      name: 'Ziraat Bankası', 
      cardHolderName: 'John Doe', 
      cardNumber: '5890040000000016', 
      cardExpireMonth: '12', 
      cardExpireYear: '2030', 
      cardCvc: '123', 
      buyerIdentityNumber: '11111111110',
      lastFour: '0016' 
    },
    { 
      id: '2', 
      name: 'Garanti BBVA', 
      cardHolderName: 'John Doe', 
      cardNumber: '5890040000000016', 
      cardExpireMonth: '10', 
      cardExpireYear: '2028', 
      cardCvc: '456', 
      buyerIdentityNumber: '11111111110',
      lastFour: '0016' 
    },
  ]);
  const [selectedCardId, setSelectedCardId] = useState<string>('1');

  // Kart Formu
  const [cardForm, setCardForm] = useState({
    cardHolderName: 'John Doe',
    cardNumber: '5890040000000016',
    cardExpireMonth: '12',
    cardExpireYear: '2030',
    cardCvc: '123',
    buyerIdentityNumber: '11111111110',
  });

  useEffect(() => {
    let isMounted = true;

    async function loadCheckoutData() {
      try {
        const [customerData, shippingRes, cartRes] = await Promise.all([
          authService.getCurrentCustomer(),
          apiClient.get<ShippingCompany[]>('/api/shipping-companies').then((r) => r.data || []).catch(() => []),
          apiClient.get('/api/carts/me').then((r) => r.data).catch(() => null),
        ]);

        if (!isMounted) return;

        if (!cartRes || !cartRes.items || cartRes.items.length === 0) {
          alert('Sepetinizde ürün bulunmuyor. Lütfen önce sepete ürün ekleyiniz.');
          router.replace('/shop/cart');
          return;
        }

        const userAddrs = customerData?.addresses || [];
        setAddresses(userAddrs);
        const defaultAddr = userAddrs.find((a: CustomerAddressDto) => a.isDefault) || userAddrs[0];
        if (defaultAddr) {
          setSelectedAddressId(defaultAddr.id);
        }

        const activeShips = shippingRes.filter((s) => s.isActive !== false);
        setShippingCompanies(activeShips);
        if (activeShips.length > 0) {
          setSelectedShippingCompanyId(activeShips[0].id);
        }

      } catch {
        if (isMounted) {
          router.replace('/auth/login');
        }
      } finally {
        if (isMounted) setLoading(false);
      }
    }

    loadCheckoutData();

    return () => {
      isMounted = false;
    };
  }, [router]);

  const handlePlaceOrder = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!selectedAddressId) {
      alert('Lütfen bir teslimat adresi seçiniz.');
      return;
    }

    if (!selectedShippingCompanyId) {
      alert('Lütfen bir kargo şirketi seçiniz.');
      return;
    }

    setSubmitting(true);

    try {
      let activeCardData = cardForm;
      if (paymentTab === 'saved') {
        const found = savedCards.find((c) => c.id === selectedCardId);
        if (found) {
          activeCardData = {
            cardHolderName: found.cardHolderName,
            cardNumber: found.cardNumber,
            cardExpireMonth: found.cardExpireMonth,
            cardExpireYear: found.cardExpireYear,
            cardCvc: found.cardCvc,
            buyerIdentityNumber: found.buyerIdentityNumber,
          };
        }
      }

      const payload = {
        addressId: selectedAddressId,
        shippingCompanyId: selectedShippingCompanyId,
        couponCodes: couponCode.trim() ? [couponCode.trim()] : [],
        ...activeCardData,
      };

      const orderId = await checkoutService.placeMyOrder(payload);
      alert('Siparişiniz başarıyla alındı! Sipariş ID: ' + orderId);
      router.push('/shop/profile');
    } catch (error: any) {
      const errData = error?.response?.data;
      let msg = 'Bilinmeyen hata oluştu.';
      if (typeof errData === 'string') {
        msg = errData;
      } else if (errData?.detail) {
        msg = errData.detail;
      } else if (errData?.title) {
        msg = errData.title;
      } else if (errData?.message) {
        msg = errData.message;
      } else if (errData?.errors) {
        msg = Object.entries(errData.errors)
          .map(([key, val]) => `${key}: ${Array.isArray(val) ? val.join(', ') : val}`)
          .join('\n');
      } else if (error?.message) {
        msg = error.message;
      }

      alert(`Ödeme Reddedildi:\n${msg}`);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fafafa', color: '#666' }}>
        Ödeme bilgileri yükleniyor...
      </div>
    );
  }

  const selectedShipping = shippingCompanies.find((s) => s.id === selectedShippingCompanyId);
  const shippingFee = selectedShipping ? selectedShipping.fee : 0;

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#fafafa', color: '#111', fontFamily: 'system-ui, -apple-system, sans-serif' }}>
      
      <header style={{ borderBottom: '1px solid #eaeaea', backgroundColor: '#fff', position: 'sticky', top: 0, zIndex: 10 }}>
        <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '1rem 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Link href="/shop/home" style={{ fontSize: '1.25rem', fontWeight: '700', letterSpacing: '0.1em', textDecoration: 'none', color: '#000' }}>
            LUXE STORE
          </Link>
          <Link href="/shop/cart" style={{ fontSize: '0.85rem', color: '#666', textDecoration: 'none' }}>
            ← Sepete Dön
          </Link>
        </div>
      </header>

      <main style={{ maxWidth: '1000px', margin: '0 auto', padding: '2.5rem 2rem' }}>
        <h1 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: '2rem' }}>ÖDEME VE SİPARİŞ</h1>

        <form onSubmit={handlePlaceOrder} style={{ display: 'grid', gridTemplateColumns: '1fr 360px', gap: '2rem' }}>
          
          <div style={{ display: 'flex', flexDirection: 'column', gap: '2rem' }}>
            
            {/* TESLİMAT ADRESİ SEÇİMİ */}
            <div style={{ backgroundColor: '#fff', padding: '1.5rem', border: '1px solid #eee', borderRadius: '8px' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <h2 style={{ fontSize: '1.1rem', fontWeight: '600', margin: 0 }}>Teslimat Adresi</h2>
                <Link href="/shop/profile" style={{ fontSize: '0.8rem', color: '#2563eb', textDecoration: 'none' }}>
                  + Yeni Adres Ekle
                </Link>
              </div>

              {addresses.length > 0 ? (
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                  {addresses.map((addr) => {
                    const isSelected = selectedAddressId === addr.id;
                    return (
                      <div
                        key={addr.id}
                        onClick={() => setSelectedAddressId(addr.id)}
                        style={{
                          border: isSelected ? '2px solid #111' : '1px solid #e5e7eb',
                          borderRadius: '6px',
                          padding: '1rem',
                          cursor: 'pointer',
                          backgroundColor: isSelected ? '#fafafa' : '#fff',
                        }}
                      >
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.4rem' }}>
                          <strong style={{ fontSize: '0.9rem', color: '#111' }}>{addr.label}</strong>
                          {isSelected && <span style={{ fontSize: '0.7rem', backgroundColor: '#111', color: '#fff', padding: '0.1rem 0.4rem', borderRadius: '3px' }}>SEÇİLİ</span>}
                        </div>
                        <p style={{ fontSize: '0.8rem', color: '#555', margin: 0, lineHeight: '1.4' }}>
                          <strong>{addr.recipientName}</strong> ({addr.phoneNumber})<br />
                          {addr.addressLine1}<br />
                          {addr.district} / {addr.city}
                        </p>
                      </div>
                    );
                  })}
                </div>
              ) : (
                <div style={{ padding: '1rem', backgroundColor: '#fef2f2', border: '1px solid #fecaca', borderRadius: '6px', color: '#991b1b', fontSize: '0.85rem' }}>
                  Kayıtlı adresiniz bulunamadı. Lütfen profilinizden adres ekleyiniz.
                </div>
              )}
            </div>

            {/* KARGO FİRMASI SEÇİMİ */}
            <div style={{ backgroundColor: '#fff', padding: '1.5rem', border: '1px solid #eee', borderRadius: '8px' }}>
              <h2 style={{ fontSize: '1.1rem', fontWeight: '600', marginBottom: '1rem' }}>Kargo Şirketi Seçimi</h2>
              
              {shippingCompanies.length > 0 ? (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                  {shippingCompanies.map((ship) => {
                    const isSelected = selectedShippingCompanyId === ship.id;
                    const isCheap = ship.fee < 20; 
                    const deliveryTimeText = isCheap ? ' (4-6 İş Günü)' : ' (1-2 İş Günü)';

                    return (
                      <label
                        key={ship.id}
                        onClick={() => setSelectedShippingCompanyId(ship.id)}
                        style={{
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'space-between',
                          padding: '0.85rem 1rem',
                          border: isSelected ? '2px solid #111' : '1px solid #e5e7eb',
                          borderRadius: '6px',
                          cursor: 'pointer',
                          backgroundColor: isSelected ? '#fafafa' : '#fff',
                        }}
                      >
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                          <input
                            type="radio"
                            name="shipping"
                            checked={isSelected}
                            onChange={() => setSelectedShippingCompanyId(ship.id)}
                          />
                          <div>
                            <span style={{ fontSize: '0.9rem', fontWeight: '600', color: '#111' }}>
                              {ship.name}
                            </span>
                            <span style={{ fontSize: '0.8rem', color: '#666', marginLeft: '0.3rem' }}>
                              {deliveryTimeText}
                            </span>
                          </div>
                        </div>

                        <strong style={{ fontSize: '0.9rem', color: '#111' }}>
                          ₺{Number(ship.fee).toFixed(2)}
                        </strong>
                      </label>
                    );
                  })}
                </div>
              ) : (
                <p style={{ fontSize: '0.85rem', color: '#888' }}>Sistemde aktif kargo şirketi bulunamadı.</p>
              )}
            </div>

            {/* ÖDEME BİLGİLERİ */}
            <div style={{ backgroundColor: '#fff', padding: '1.5rem', border: '1px solid #eee', borderRadius: '8px' }}>
              <h2 style={{ fontSize: '1.1rem', fontWeight: '600', marginBottom: '1rem' }}>Ödeme Bilgileri</h2>
              
              {/* Sekme Seçimi - Profesyonel İsimlendirme */}
              <div style={{ display: 'flex', gap: '1.5rem', marginBottom: '1.5rem', borderBottom: '1px solid #eaeaea', paddingBottom: '0.75rem' }}>
                <button
                  type="button"
                  onClick={() => setPaymentTab('saved')}
                  style={{
                    background: 'none',
                    border: 'none',
                    fontSize: '0.9rem',
                    fontWeight: paymentTab === 'saved' ? '700' : '400',
                    color: paymentTab === 'saved' ? '#111' : '#666',
                    cursor: 'pointer',
                    paddingBottom: '0.2rem',
                    borderBottom: paymentTab === 'saved' ? '2px solid #111' : 'none',
                  }}
                >
                  Kayıtlı Kartlarım
                </button>
                <button
                  type="button"
                  onClick={() => setPaymentTab('manual')}
                  style={{
                    background: 'none',
                    border: 'none',
                    fontSize: '0.9rem',
                    fontWeight: paymentTab === 'manual' ? '700' : '400',
                    color: paymentTab === 'manual' ? '#111' : '#666',
                    cursor: 'pointer',
                    paddingBottom: '0.2rem',
                    borderBottom: paymentTab === 'manual' ? '2px solid #111' : 'none',
                  }}
                >
                  Kredi / Banka Kartı
                </button>
              </div>

              {/* SEKME 1: Kayıtlı Kart Seçimi */}
              {paymentTab === 'saved' && (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                  <p style={{ fontSize: '0.8rem', color: '#666', marginBottom: '0.5rem' }}>
                    Daha önce kayıt ettiğiniz kartlardan birini seçerek hızlıca ödeme yapabilirsiniz.
                  </p>
                  {savedCards.map((card) => {
                    const isSelected = selectedCardId === card.id;
                    return (
                      <div
                        key={card.id}
                        onClick={() => setSelectedCardId(card.id)}
                        style={{
                          padding: '1rem',
                          border: isSelected ? '2px solid #111' : '1px solid #e5e7eb',
                          borderRadius: '6px',
                          cursor: 'pointer',
                          backgroundColor: isSelected ? '#fafafa' : '#fff',
                          display: 'flex',
                          justifyContent: 'space-between',
                          alignItems: 'center',
                        }}
                      >
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                          <input
                            type="radio"
                            name="savedCard"
                            checked={isSelected}
                            onChange={() => setSelectedCardId(card.id)}
                          />
                          <div>
                            <span style={{ fontSize: '0.9rem', fontWeight: '600', display: 'block' }}>{card.name}</span>
                            <span style={{ fontSize: '0.8rem', color: '#666' }}>•••• •••• •••• {card.lastFour} ({card.cardHolderName})</span>
                          </div>
                        </div>
                        <span style={{ fontSize: '0.8rem', color: '#666', fontWeight: '500' }}>SKT: {card.cardExpireMonth}/{card.cardExpireYear}</span>
                      </div>
                    );
                  })}
                </div>
              )}

              {/* SEKME 2: Kredi / Banka Kartı Formu */}
              {paymentTab === 'manual' && (
                <div>
                  <p style={{ fontSize: '0.8rem', color: '#16a34a', marginBottom: '1.2rem', fontWeight: '500' }}>
                    ✓ iyzico test kartı otomatik doldurulmuştur.
                  </p>

                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                    <div style={{ gridColumn: 'span 2' }}>
                      <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.3rem' }}>Kart Üzerindeki İsim</label>
                      <input
                        type="text"
                        value={cardForm.cardHolderName}
                        onChange={(e) => setCardForm({ ...cardForm, cardHolderName: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem' }}
                      />
                    </div>

                    <div style={{ gridColumn: 'span 2' }}>
                      <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.3rem' }}>Kart Numarası</label>
                      <input
                        type="text"
                        value={cardForm.cardNumber}
                        onChange={(e) => setCardForm({ ...cardForm, cardNumber: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem', backgroundColor: '#f9f9f9', fontWeight: '600' }}
                      />
                    </div>

                    <div>
                      <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.3rem' }}>Ay (MM)</label>
                      <input
                        type="text"
                        value={cardForm.cardExpireMonth}
                        onChange={(e) => setCardForm({ ...cardForm, cardExpireMonth: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem' }}
                      />
                    </div>

                    <div>
                      <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.3rem' }}>Yıl (YYYY)</label>
                      <input
                        type="text"
                        value={cardForm.cardExpireYear}
                        onChange={(e) => setCardForm({ ...cardForm, cardExpireYear: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem' }}
                      />
                    </div>

                    <div>
                      <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.3rem' }}>CVC</label>
                      <input
                        type="text"
                        value={cardForm.cardCvc}
                        onChange={(e) => setCardForm({ ...cardForm, cardCvc: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem' }}
                      />
                    </div>

                    <div>
                      <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.3rem' }}>TCKN</label>
                      <input
                        type="text"
                        value={cardForm.buyerIdentityNumber}
                        onChange={(e) => setCardForm({ ...cardForm, buyerIdentityNumber: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem' }}
                      />
                    </div>
                  </div>
                </div>
              )}

            </div>

          </div>

          {/* Sağ Kolon: Sipariş Özeti */}
          <div style={{ backgroundColor: '#fff', padding: '1.5rem', border: '1px solid #eee', borderRadius: '8px', height: 'fit-content' }}>
            <h2 style={{ fontSize: '1.1rem', fontWeight: '600', marginBottom: '1.2rem' }}>Sipariş Özeti</h2>
            
            <div style={{ marginBottom: '1.2rem', fontSize: '0.85rem', color: '#555', display: 'flex', justifyContent: 'space-between' }}>
              <span>Kargo Ücreti:</span>
              <strong>{shippingFee === 0 ? 'Ücretsiz' : `₺${shippingFee.toFixed(2)}`}</strong>
            </div>

            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.3rem' }}>İndirim / Kupon Kodu</label>
              <input
                type="text"
                placeholder="Varsa giriniz"
                value={couponCode}
                onChange={(e) => setCouponCode(e.target.value)}
                style={{ width: '100%', padding: '0.65rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.85rem' }}
              />
            </div>

            <button
              type="submit"
              disabled={submitting || addresses.length === 0 || shippingCompanies.length === 0}
              style={{
                width: '100%',
                backgroundColor: addresses.length === 0 || shippingCompanies.length === 0 ? '#ccc' : '#111',
                color: '#fff',
                padding: '1rem',
                border: 'none',
                borderRadius: '4px',
                fontWeight: '600',
                fontSize: '0.9rem',
                cursor: addresses.length === 0 || shippingCompanies.length === 0 ? 'not-allowed' : 'pointer',
              }}
            >
              {submitting ? 'İşleniyor...' : 'SİPARİŞİ TAMAMLA VE ÖDE'}
            </button>
          </div>

        </form>
      </main>
    </div>
  );
}