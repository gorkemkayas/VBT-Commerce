'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { authService } from '@/services/auth.service';

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await authService.login({ email, password });

      if (res?.accessToken) {
        router.refresh();
        router.push('/shop/profile');
      }
    } catch (error) {
      alert('Giriş başarısız! Lütfen bilgilerinizi kontrol edin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#fafafa' }}>
      <form onSubmit={handleLogin} style={{ backgroundColor: '#fff', padding: '2.5rem', border: '1px solid #eee', borderRadius: '8px', width: '100%', maxWidth: '400px' }}>
        <h1 style={{ textAlign: 'center', marginBottom: '2rem', fontSize: '1.5rem', fontWeight: '600' }}>GİRİŞ YAP</h1>
        
        <input
          type="email"
          placeholder="E-posta"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          style={{ width: '100%', padding: '0.8rem', marginBottom: '1rem', border: '1px solid #ccc', borderRadius: '4px' }}
        />
        
        <input
          type="password"
          placeholder="Şifre"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          style={{ width: '100%', padding: '0.8rem', marginBottom: '1.5rem', border: '1px solid #ccc', borderRadius: '4px' }}
        />

        <button 
          type="submit" 
          disabled={loading}
          style={{ width: '100%', backgroundColor: '#111', color: '#fff', padding: '0.9rem', border: 'none', borderRadius: '4px', fontWeight: '600', cursor: 'pointer' }}
        >
          {loading ? 'Giriş Yapılıyor...' : 'GİRİŞ YAP'}
        </button>
      </form>
    </div>
  );
}