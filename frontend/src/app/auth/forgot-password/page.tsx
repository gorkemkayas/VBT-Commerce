'use client';
import { useState } from 'react';
import Link from 'next/link';

export default function ForgotPasswordPage() {
  const [sent, setSent] = useState(false);

  return (
    <main className="max-w-md mx-auto min-h-screen bg-white font-sans border-x p-6">
      <header className="flex justify-between items-center mb-8">
        <Link href="/login" className="text-sm font-bold">← Back</Link>
        <span className="font-bold text-lg tracking-widest text-blue-600">LUXE</span>
        <span className="w-8"></span>
      </header>

      {!sent ? (
        <div>
          <h1 className="text-xl font-bold text-neutral-900">Reset your password</h1>
          <p className="text-xs text-neutral-500 mt-2 leading-relaxed">
            Enter the email associated with your account and we&apos;ll send you a link to set a new password.
          </p>

          <form onSubmit={(e) => { e.preventDefault(); setSent(true); }} className="mt-6">
            <label className="text-[10px] font-bold text-neutral-500 uppercase tracking-wider">Email Address</label>
            <input
              type="email"
              required
              placeholder="name@example.com"
              className="w-full p-3 border border-neutral-200 text-xs mt-1 focus:outline-blue-600 mb-4"
            />
            <button
              type="submit"
              className="w-full py-4 bg-neutral-950 text-white text-xs font-bold tracking-widest uppercase hover:bg-neutral-800"
            >
              Send Reset Link
            </button>
          </form>
        </div>
      ) : (
        <div className="bg-neutral-50 p-6 rounded-sm text-center border mt-8">
          <div className="w-10 h-10 bg-neutral-200 rounded-full flex items-center justify-center mx-auto mb-4 font-bold">✓</div>
          <h2 className="text-base font-bold text-neutral-900">Check your email</h2>
          <p className="text-xs text-neutral-500 mt-2">We sent a reset link to your email address. It expires in 30 minutes.</p>
        </div>
      )}
    </main>
  );
}