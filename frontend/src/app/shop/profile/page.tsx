'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { authService } from '@/services/auth.service';
import { apiClient } from '@/lib/api-client';
import { CustomerDto } from '@/types/api';

interface SavedCard {
  id: string;
  cardAlias: string;
  cardHolderName: string;
  cardNumber: string;
  cardExpireMonth: string;
  cardExpireYear: string;
  cardCvc: string;
}

export default function ProfilePage() {
  const router = useRouter();
  const [customer, setCustomer] = useState<CustomerDto | null>(null);
  const [orders, setOrders] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'orders' | 'details' | 'addresses' | 'payments'>('orders');

  // Adres Ekle Formu Modal Durumu
  const [showAddressModal, setShowAddressModal] = useState(false);
  const [newAddress, setNewAddress] = useState({
    label: '',
    recipientName: '',
    phoneNumber: '',
    country: 'Türkiye',
    city: '',
    district: '',
    postalCode: '',
    addressLine1: '',
    addressLine2: '',
    isDefault: false,
  });

  // Müşteri Bilgisi Güncelleme Durumu
  const [phoneInput, setPhoneInput] = useState('');
  const [dobInput, setDobInput] = useState('');
  const [updatingProfile, setUpdatingProfile] = useState(false);

  // Kart Yönetimi State'leri
  const [savedCards, setSavedCards] = useState<SavedCard[]>([]);
  const [showCardModal, setShowCardModal] = useState(false);
  const [newCard, setNewCard] = useState({
    cardAlias: '',
    cardHolderName: '',
    cardNumber: '',
    cardExpireMonth: '',
    cardExpireYear: '',
    cardCvc: '',
  });

  // Verileri Çekme Fonksiyonu
  const loadProfileData = () => {
    Promise.all([
      authService.getCurrentCustomer(),
      apiClient.get('/api/orders/me').then((res) => res.data?.items || []).catch(() => []),
    ])
      .then(([customerData, ordersData]) => {
        setCustomer(customerData);
        setOrders(ordersData);
        if (customerData) {
          setPhoneInput(customerData.phoneNumber || '');
          setDobInput(customerData.dateOfBirth ? customerData.dateOfBirth.split('T')[0] : '');
        }

        // Kayıtlı Kartları LocalStorage'dan Çek
        const storedCards = JSON.parse(localStorage.getItem('user_saved_cards') || '[]');
        if (storedCards.length === 0 && customerData) {
          const defaultCards: SavedCard[] = [
            {
              id: '1',
              cardAlias: 'Bonus Kredi Kartım',
              cardHolderName: customerData?.addresses?.[0]?.recipientName || 'Müşteri',
              cardNumber: '5890040000000016',
              cardExpireMonth: '12',
              cardExpireYear: '2030',
              cardCvc: '123',
            },
            {
              id: '2',
              cardAlias: 'Maximum Garanti',
              cardHolderName: customerData?.addresses?.[0]?.recipientName || 'Müşteri',
              cardNumber: '5528790000000001',
              cardExpireMonth: '12',
              cardExpireYear: '2030',
              cardCvc: '123',
            },
          ];
          localStorage.setItem('user_saved_cards', JSON.stringify(defaultCards));
          setSavedCards(defaultCards);
        } else {
          setSavedCards(storedCards);
        }

        setLoading(false);
      })
      .catch(() => {
        router.replace('/auth/login');
      });
  };

  useEffect(() => {
    loadProfileData();
  }, [router]);

  // YENİ KART EKLEME
  const handleAddCard = (e: React.FormEvent) => {
    e.preventDefault();
    const createdCard: SavedCard = {
      id: Date.now().toString(),
      ...newCard,
    };
    const updated = [...savedCards, createdCard];
    setSavedCards(updated);
    localStorage.setItem('user_saved_cards', JSON.stringify(updated));
    setShowCardModal(false);
    setNewCard({
      cardAlias: '',
      cardHolderName: '',
      cardNumber: '',
      cardExpireMonth: '',
      cardExpireYear: '',
      cardCvc: '',
    });
    alert('Yeni ödeme kartınız eklendi!');
  };

  // KART SİLME
  const handleDeleteCard = (cardId: string) => {
    if (!confirm('Bu kartı silmek istediğinize emin misiniz?')) return;
    const updated = savedCards.filter((c) => c.id !== cardId);
    setSavedCards(updated);
    localStorage.setItem('user_saved_cards', JSON.stringify(updated));
  };

  // YENİ ADRES EKLEME
  const handleAddAddress = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await apiClient.post('/api/customers/me/addresses', newAddress);
      alert('Yeni adres başarıyla eklendi!');
      setShowAddressModal(false);
      setNewAddress({
        label: '',
        recipientName: '',
        phoneNumber: '',
        country: 'Türkiye',
        city: '',
        district: '',
        postalCode: '',
        addressLine1: '',
        addressLine2: '',
        isDefault: false,
      });
      loadProfileData();
    } catch {
      alert('Adres eklenirken bir hata oluştu!');
    }
  };

  // ADRES SİLME
  const handleDeleteAddress = async (addressId: string) => {
    if (!confirm('Bu adresi silmek istediğinize emin misiniz?')) return;
    try {
      await apiClient.delete(`/api/customers/me/addresses/${addressId}`);
      loadProfileData();
    } catch {
      alert('Adres silinemedi.');
    }
  };

  // MÜŞTERİ BİLGİLERİNİ GÜNCELLEME
  const handleUpdateCustomer = async (e: React.FormEvent) => {
    e.preventDefault();
    setUpdatingProfile(true);
    try {
      await apiClient.put('/api/customers/me', {
        phoneNumber: phoneInput,
        dateOfBirth: dobInput || null,
      });
      alert('Bilgileriniz güncellendi!');
      loadProfileData();
    } catch {
      alert('Güncelleme sırasında hata oluştu.');
    } finally {
      setUpdatingProfile(false);
    }
  };

  const handleLogout = async () => {
    await authService.logout();
    router.replace('/auth/login');
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fafafa', color: '#666', fontFamily: 'sans-serif' }}>
        Profiliniz yükleniyor...
      </div>
    );
  }

  const defaultAddress = customer?.addresses?.find((a) => a.isDefault) || customer?.addresses?.[0];
  const totalOrdersCount = orders.length;
  const activeOrdersCount = orders.filter((o) => o.status !== 3 && o.status !== 4).length;

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#fafafa', color: '#111', fontFamily: 'system-ui, -apple-system, sans-serif' }}>
      
      {/* Header */}
      <header style={{ borderBottom: '1px solid #eaeaea', backgroundColor: '#fff', position: 'sticky', top: 0, zIndex: 10 }}>
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '1rem 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Link href="/shop/home" style={{ fontSize: '1.25rem', fontWeight: '700', letterSpacing: '0.1em', textTransform: 'uppercase', textDecoration: 'none', color: '#000' }}>
            LUXE STORE
          </Link>
          <nav style={{ display: 'flex', gap: '2rem', fontSize: '0.9rem', fontWeight: '500' }}>
            <Link href="/shop/home" style={{ color: '#666', textDecoration: 'none' }}>Koleksiyonlar</Link>
            <Link href="/shop/cart" style={{ color: '#666', textDecoration: 'none' }}>Sepet</Link>
            <Link href="/shop/profile" style={{ color: '#000', fontWeight: '600', textDecoration: 'none' }}>Hesabım</Link>
          </nav>
        </div>
      </header>

      {/* Main Container */}
      <main style={{ maxWidth: '1100px', margin: '0 auto', padding: '3rem 2rem' }}>
        
        {/* Üst Profil Kartı */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', backgroundColor: '#fff', padding: '1.5rem 2rem', borderRadius: '8px', border: '1px solid #eee', marginBottom: '2rem' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
            <div style={{ width: '56px', height: '56px', borderRadius: '50%', backgroundColor: '#111', color: '#fff', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.2rem', fontWeight: '600' }}>
              {defaultAddress?.recipientName ? defaultAddress.recipientName.charAt(0).toUpperCase() : 'M'}
            </div>
            <div>
              <h1 style={{ fontSize: '1.2rem', fontWeight: '600', margin: 0 }}>
                {defaultAddress?.recipientName || 'Değerli Müşterimiz'}
              </h1>
              <p style={{ color: '#666', fontSize: '0.85rem', margin: '0.2rem 0 0 0' }}>
                {customer?.phoneNumber || 'Telefon eklenmedi'}
              </p>
            </div>
          </div>

          <div style={{ display: 'flex', gap: '1.5rem' }}>
            <div style={{ backgroundColor: '#fafafa', border: '1px solid #eee', padding: '0.8rem 1.5rem', borderRadius: '6px', textAlign: 'center', minWidth: '90px' }}>
              <div style={{ fontSize: '1.4rem', fontWeight: '700', color: '#111' }}>{totalOrdersCount}</div>
              <div style={{ fontSize: '0.75rem', color: '#666', textTransform: 'uppercase' }}>Siparişler</div>
            </div>
            <div style={{ backgroundColor: '#fafafa', border: '1px solid #eee', padding: '0.8rem 1.5rem', borderRadius: '6px', textAlign: 'center', minWidth: '90px' }}>
              <div style={{ fontSize: '1.4rem', fontWeight: '700', color: '#2563eb' }}>{activeOrdersCount}</div>
              <div style={{ fontSize: '0.75rem', color: '#666', textTransform: 'uppercase' }}>Yoldakiler</div>
            </div>
            <div style={{ backgroundColor: '#fafafa', border: '1px solid #eee', padding: '0.8rem 1.5rem', borderRadius: '6px', textAlign: 'center', minWidth: '90px' }}>
              <div style={{ fontSize: '1.4rem', fontWeight: '700', color: '#111' }}>{customer?.addresses?.length || 0}</div>
              <div style={{ fontSize: '0.75rem', color: '#666', textTransform: 'uppercase' }}>Adresler</div>
            </div>
          </div>

          <button
            onClick={handleLogout}
            style={{ backgroundColor: 'transparent', color: '#dc2626', border: '1px solid #fca5a5', padding: '0.6rem 1.2rem', borderRadius: '4px', fontSize: '0.85rem', fontWeight: '600', cursor: 'pointer' }}
          >
            Çıkış Yap
          </button>
        </div>

        {/* Dashboard Grid */}
        <div style={{ display: 'grid', gridTemplateColumns: '240px 1fr', gap: '2rem' }}>
          
          {/* Sol Navigasyon Menüsü */}
          <div style={{ backgroundColor: '#fff', borderRadius: '8px', border: '1px solid #eee', overflow: 'hidden', height: 'fit-content' }}>
            <button
              onClick={() => setActiveTab('orders')}
              style={{ width: '100%', padding: '1rem 1.25rem', textAlign: 'left', border: 'none', borderBottom: '1px solid #eee', backgroundColor: activeTab === 'orders' ? '#fafafa' : '#fff', fontWeight: activeTab === 'orders' ? '600' : '400', cursor: 'pointer', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}
            >
              <span>📦 Siparişlerim</span>
              {activeOrdersCount > 0 && (
                <span style={{ backgroundColor: '#dbeafe', color: '#1e40af', fontSize: '0.75rem', padding: '0.2rem 0.5rem', borderRadius: '12px', fontWeight: '600' }}>
                  {activeOrdersCount} AKTİF
                </span>
              )}
            </button>

            <button
              onClick={() => setActiveTab('details')}
              style={{ width: '100%', padding: '1rem 1.25rem', textAlign: 'left', border: 'none', borderBottom: '1px solid #eee', backgroundColor: activeTab === 'details' ? '#fafafa' : '#fff', fontWeight: activeTab === 'details' ? '600' : '400', cursor: 'pointer' }}
            >
              👤 Hesap Detayları
            </button>

            <button
              onClick={() => setActiveTab('addresses')}
              style={{ width: '100%', padding: '1rem 1.25rem', textAlign: 'left', border: 'none', borderBottom: '1px solid #eee', backgroundColor: activeTab === 'addresses' ? '#fafafa' : '#fff', fontWeight: activeTab === 'addresses' ? '600' : '400', cursor: 'pointer' }}
            >
              📍 Adreslerim ({customer?.addresses?.length || 0})
            </button>

            <button
              onClick={() => setActiveTab('payments')}
              style={{ width: '100%', padding: '1rem 1.25rem', textAlign: 'left', border: 'none', backgroundColor: activeTab === 'payments' ? '#fafafa' : '#fff', fontWeight: activeTab === 'payments' ? '600' : '400', cursor: 'pointer' }}
            >
              💳 Ödeme Yöntemleri ({savedCards.length})
            </button>
          </div>

          {/* Sağ Sekme İçerikleri */}
          <div style={{ backgroundColor: '#fff', borderRadius: '8px', border: '1px solid #eee', padding: '2rem' }}>
            
            {/* 1. SİPARİŞLERİM */}
            {activeTab === 'orders' && (
              <div>
                <h2 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: '1.5rem' }}>Sipariş Geçmişi</h2>
                {orders.length > 0 ? (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                    {orders.map((order) => (
                      <div key={order.id} style={{ border: '1px solid #eee', borderRadius: '6px', padding: '1.25rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <div>
                          <div style={{ fontSize: '0.85rem', color: '#888' }}>Sipariş Kodu: <strong style={{ color: '#111' }}>#{order.id?.substring(0, 8)}</strong></div>
                          <div style={{ fontSize: '0.9rem', fontWeight: '600', color: '#111', marginTop: '0.2rem' }}>Toplam: ₺{Number(order.grandTotal || 0).toFixed(2)}</div>
                          <div style={{ fontSize: '0.8rem', color: '#666', marginTop: '0.2rem' }}>Tarih: {new Date(order.createdAt).toLocaleDateString('tr-TR')}</div>
                        </div>
                        <span style={{ backgroundColor: '#f3f4f6', color: '#374151', padding: '0.4rem 0.8rem', borderRadius: '4px', fontSize: '0.8rem', fontWeight: '600' }}>
                          Sipariş Alındı
                        </span>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p style={{ color: '#888', fontSize: '0.9rem' }}>Henüz verdiğiniz bir sipariş bulunmuyor.</p>
                )}
              </div>
            )}

            {/* 2. HESAP DETAYLARI */}
            {activeTab === 'details' && (
              <div>
                <h2 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: '1.5rem' }}>Müşteri Bilgilerini Düzenle</h2>
                <form onSubmit={handleUpdateCustomer} style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem', maxWidth: '400px' }}>
                  <div>
                    <label style={{ display: 'block', fontSize: '0.85rem', color: '#666', marginBottom: '0.4rem' }}>Telefon Numarası</label>
                    <input
                      type="text"
                      value={phoneInput}
                      onChange={(e) => setPhoneInput(e.target.value)}
                      placeholder="05XXXXXXXXX"
                      style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem' }}
                    />
                  </div>

                  <div>
                    <label style={{ display: 'block', fontSize: '0.85rem', color: '#666', marginBottom: '0.4rem' }}>Doğum Tarihi</label>
                    <input
                      type="date"
                      value={dobInput}
                      onChange={(e) => setDobInput(e.target.value)}
                      style={{ width: '100%', padding: '0.75rem', border: '1px solid #ccc', borderRadius: '4px', fontSize: '0.9rem' }}
                    />
                  </div>

                  <button
                    type="submit"
                    disabled={updatingProfile}
                    style={{ backgroundColor: '#111', color: '#fff', border: 'none', padding: '0.8rem', borderRadius: '4px', fontWeight: '600', cursor: 'pointer', marginTop: '0.5rem' }}
                  >
                    {updatingProfile ? 'Kaydediliyor...' : 'Bilgileri Kaydet'}
                  </button>
                </form>
              </div>
            )}

            {/* 3. ADRESLERİM */}
            {activeTab === 'addresses' && (
              <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
                  <h2 style={{ fontSize: '1.2rem', fontWeight: '600', margin: 0 }}>Kayıtlı Adresler</h2>
                  <button
                    onClick={() => setShowAddressModal(true)}
                    style={{ backgroundColor: '#111', color: '#fff', border: 'none', padding: '0.6rem 1rem', borderRadius: '4px', fontSize: '0.85rem', fontWeight: '600', cursor: 'pointer' }}
                  >
                    + Yeni Adres Ekle
                  </button>
                </div>

                {customer?.addresses && customer.addresses.length > 0 ? (
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                    {customer.addresses.map((addr) => (
                      <div key={addr.id} style={{ border: '1px solid #eee', borderRadius: '6px', padding: '1.25rem', backgroundColor: addr.isDefault ? '#fff' : '#fafafa', position: 'relative' }}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem' }}>
                          <strong style={{ fontSize: '0.95rem', color: '#111' }}>{addr.label}</strong>
                          {addr.isDefault && (
                            <span style={{ fontSize: '0.7rem', backgroundColor: '#111', color: '#fff', padding: '0.1rem 0.4rem', borderRadius: '3px' }}>VARSAYILAN</span>
                          )}
                        </div>
                        <p style={{ margin: 0, fontSize: '0.85rem', color: '#555', lineHeight: '1.4' }}>
                          <strong>{addr.recipientName}</strong> ({addr.phoneNumber})<br />
                          {addr.addressLine1} {addr.addressLine2 || ''}<br />
                          {addr.district} / {addr.city}
                        </p>
                        <button
                          onClick={() => handleDeleteAddress(addr.id)}
                          style={{ marginTop: '0.8rem', background: 'none', border: 'none', color: '#dc2626', fontSize: '0.8rem', cursor: 'pointer', padding: 0 }}
                        >
                          Adresi Sil
                        </button>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p style={{ color: '#888', fontSize: '0.9rem' }}>Kayıtlı adresiniz yok.</p>
                )}
              </div>
            )}

            {/* 4. ÖDEME YÖNTEMLERİ & KART EKLEME */}
            {activeTab === 'payments' && (
              <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
                  <div>
                    <h2 style={{ fontSize: '1.2rem', fontWeight: '600', margin: 0 }}>Kayıtlı Ödeme Kartları</h2>
                    <p style={{ fontSize: '0.8rem', color: '#666', margin: '0.3rem 0 0 0' }}>Sipariş verirken hızlıca kullanabileceğiniz kayıtlı kartlarınız.</p>
                  </div>
                  <button
                    onClick={() => setShowCardModal(true)}
                    style={{ backgroundColor: '#111', color: '#fff', border: 'none', padding: '0.6rem 1rem', borderRadius: '4px', fontSize: '0.85rem', fontWeight: '600', cursor: 'pointer' }}
                  >
                    + Yeni Kart Ekle
                  </button>
                </div>

                {savedCards.length > 0 ? (
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                    {savedCards.map((card) => (
                      <div key={card.id} style={{ border: '1px solid #eee', borderRadius: '8px', padding: '1.25rem', backgroundColor: '#fff', boxShadow: '0 1px 3px rgba(0,0,0,0.05)' }}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.75rem' }}>
                          <strong style={{ fontSize: '0.95rem', color: '#111' }}>💳 {card.cardAlias}</strong>
                          <span style={{ fontSize: '0.7rem', backgroundColor: '#e5e7eb', padding: '0.15rem 0.4rem', borderRadius: '3px', fontWeight: '600' }}>VISA / MASTER</span>
                        </div>
                        <p style={{ margin: 0, fontSize: '0.85rem', color: '#444', lineHeight: '1.5' }}>
                          <strong>{card.cardHolderName}</strong><br />
                          **** **** **** {card.cardNumber.slice(-4)}<br />
                          Son Kullanma: {card.cardExpireMonth}/{card.cardExpireYear}
                        </p>
                        <button
                          onClick={() => handleDeleteCard(card.id)}
                          style={{ marginTop: '0.8rem', background: 'none', border: 'none', color: '#dc2626', fontSize: '0.8rem', cursor: 'pointer', padding: 0 }}
                        >
                          Kartı Sil
                        </button>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p style={{ color: '#888', fontSize: '0.9rem' }}>Henüz kayıtlı bir ödeme kartınız yok.</p>
                )}
              </div>
            )}

          </div>
        </div>
      </main>

      {/* MODAL: YENİ ADRES EKLEME */}
      {showAddressModal && (
        <div style={{ position: 'fixed', top: 0, left: 0, right: 0, bottom: 0, backgroundColor: 'rgba(0,0,0,0.5)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 }}>
          <div style={{ backgroundColor: '#fff', padding: '2rem', borderRadius: '8px', width: '100%', maxWidth: '500px', maxHeight: '90vh', overflowY: 'auto' }}>
            <h2 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: '1.5rem' }}>Yeni Adres Ekle</h2>
            
            <form onSubmit={handleAddAddress} style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
              <div style={{ gridColumn: 'span 2' }}>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Adres Başlığı</label>
                <input required type="text" value={newAddress.label} onChange={(e) => setNewAddress({ ...newAddress, label: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Teslim Alacak Kişi</label>
                <input required type="text" value={newAddress.recipientName} onChange={(e) => setNewAddress({ ...newAddress, recipientName: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Telefon</label>
                <input required type="text" value={newAddress.phoneNumber} onChange={(e) => setNewAddress({ ...newAddress, phoneNumber: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>İl (Şehir)</label>
                <input required type="text" value={newAddress.city} onChange={(e) => setNewAddress({ ...newAddress, city: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>İlçe</label>
                <input required type="text" value={newAddress.district} onChange={(e) => setNewAddress({ ...newAddress, district: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div style={{ gridColumn: 'span 2' }}>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Adres Satırı</label>
                <input required type="text" value={newAddress.addressLine1} onChange={(e) => setNewAddress({ ...newAddress, addressLine1: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Posta Kodu</label>
                <input required type="text" value={newAddress.postalCode} onChange={(e) => setNewAddress({ ...newAddress, postalCode: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div style={{ gridColumn: 'span 2', display: 'flex', justifyContent: 'flex-end', gap: '1rem', marginTop: '1rem' }}>
                <button type="button" onClick={() => setShowAddressModal(false)} style={{ backgroundColor: '#f3f4f6', border: 'none', padding: '0.6rem 1.2rem', borderRadius: '4px', cursor: 'pointer' }}>İptal</button>
                <button type="submit" style={{ backgroundColor: '#111', color: '#fff', border: 'none', padding: '0.6rem 1.2rem', borderRadius: '4px', cursor: 'pointer' }}>Kaydet</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* MODAL: YENİ KART EKLEME */}
      {showCardModal && (
        <div style={{ position: 'fixed', top: 0, left: 0, right: 0, bottom: 0, backgroundColor: 'rgba(0,0,0,0.5)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 }}>
          <div style={{ backgroundColor: '#fff', padding: '2rem', borderRadius: '8px', width: '100%', maxWidth: '450px' }}>
            <h2 style={{ fontSize: '1.2rem', fontWeight: '600', marginBottom: '1.5rem' }}>Yeni Ödeme Kartı Ekle</h2>
            
            <form onSubmit={handleAddCard} style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
              <div style={{ gridColumn: 'span 2' }}>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Kart Rumuzu</label>
                <input required type="text" placeholder="İş Kartım" value={newCard.cardAlias} onChange={(e) => setNewCard({ ...newCard, cardAlias: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div style={{ gridColumn: 'span 2' }}>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Kart Üzerindeki İsim</label>
                <input required type="text" placeholder="John Doe" value={newCard.cardHolderName} onChange={(e) => setNewCard({ ...newCard, cardHolderName: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div style={{ gridColumn: 'span 2' }}>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Kart Numarası</label>
                <input required type="text" maxLength={19} placeholder="5890040000000016" value={newCard.cardNumber} onChange={(e) => setNewCard({ ...newCard, cardNumber: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Ay (MM)</label>
                <input required type="text" maxLength={2} placeholder="12" value={newCard.cardExpireMonth} onChange={(e) => setNewCard({ ...newCard, cardExpireMonth: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>Yıl (YYYY)</label>
                <input required type="text" maxLength={4} placeholder="2030" value={newCard.cardExpireYear} onChange={(e) => setNewCard({ ...newCard, cardExpireYear: e.target.value })} style={{ width: '100%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div style={{ gridColumn: 'span 2' }}>
                <label style={{ display: 'block', fontSize: '0.8rem', color: '#666', marginBottom: '0.2rem' }}>CVC</label>
                <input required type="text" maxLength={4} placeholder="123" value={newCard.cardCvc} onChange={(e) => setNewCard({ ...newCard, cardCvc: e.target.value })} style={{ width: '40%', padding: '0.6rem', border: '1px solid #ccc', borderRadius: '4px' }} />
              </div>

              <div style={{ gridColumn: 'span 2', display: 'flex', justifyContent: 'flex-end', gap: '1rem', marginTop: '1rem' }}>
                <button type="button" onClick={() => setShowCardModal(false)} style={{ backgroundColor: '#f3f4f6', border: 'none', padding: '0.6rem 1.2rem', borderRadius: '4px', cursor: 'pointer' }}>İptal</button>
                <button type="submit" style={{ backgroundColor: '#111', color: '#fff', border: 'none', padding: '0.6rem 1.2rem', borderRadius: '4px', cursor: 'pointer' }}>Kaydet</button>
              </div>
            </form>
          </div>
        </div>
      )}

    </div>
  );
}