'use client';
import Link from 'next/link';

export default function ProfilePage() {
  return (
    <main className="max-w-md mx-auto min-h-screen bg-white font-sans border-x p-6 flex flex-col justify-between">
      <div>
        <h1 className="text-lg font-bold">My Account</h1>

        <div className="flex items-center gap-4 mt-6 pb-6 border-b">
          <div className="w-12 h-12 bg-neutral-200 rounded-full flex items-center justify-center font-bold text-sm">
            JD
          </div>
          <div>
            <h2 className="text-sm font-bold text-neutral-900">Jane Doe</h2>
            <p className="text-xs text-neutral-500">jane@example.com</p>
          </div>
        </div>

        {/* Dynamic Metric Cards */}
        <div className="grid grid-cols-3 gap-2 my-6">
          <div className="border p-3 text-center rounded-sm">
            <p className="text-lg font-bold">12</p>
            <p className="text-[9px] font-semibold text-neutral-400 uppercase">Orders</p>
          </div>
          <div className="border p-3 text-center rounded-sm">
            <p className="text-lg font-bold text-blue-600">1</p>
            <p className="text-[9px] font-semibold text-neutral-400 uppercase">In Transit</p>
          </div>
          <div className="border p-3 text-center rounded-sm">
            <p className="text-lg font-bold">7</p>
            <p className="text-[9px] font-semibold text-neutral-400 uppercase">Wishlist</p>
          </div>
        </div>

        {/* Account Menu */}
        <div className="divide-y text-xs font-semibold text-neutral-800">
          <Link href="/profile/orders" className="py-4 flex justify-between items-center hover:bg-neutral-50">
            <span>My Orders</span>
            <span className="text-[10px] bg-blue-100 text-blue-600 px-2 py-0.5 rounded font-bold">1 ACTIVE ›</span>
          </Link>
          <div className="py-4 flex justify-between items-center cursor-pointer">
            <span>Track an order</span>
            <span>›</span>
          </div>
          <div className="py-4 flex justify-between items-center cursor-pointer">
            <span>Account details</span>
            <span>›</span>
          </div>
          <div className="py-4 flex justify-between items-center cursor-pointer">
            <span>Addresses</span>
            <span>›</span>
          </div>
        </div>
      </div>

      <button className="w-full py-4 border border-neutral-300 text-xs font-bold tracking-widest uppercase hover:bg-neutral-50 mt-8">
        Sign Out
      </button>
    </main>
  );
}