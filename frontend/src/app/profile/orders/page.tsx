'use client';
import Link from 'next/link';

export default function OrdersPage() {
  const orders = [
    { id: '#LX-24815', date: '14 Jul 2026', items: 3, total: 1171.80, status: 'IN TRANSIT', color: 'bg-blue-100 text-blue-600' },
    { id: '#LX-24102', date: '28 Jun 2026', items: 1, total: 385.00, status: 'DELIVERED', color: 'bg-green-100 text-green-700' },
  ];

  return (
    <main className="max-w-md mx-auto min-h-screen bg-white font-sans border-x p-6">
      <header className="flex justify-between items-center mb-6">
        <Link href="/profile" className="text-sm font-bold">← Back</Link>
        <h1 className="font-bold text-base">My Orders</h1>
        <span className="w-8"></span>
      </header>

      <div className="flex gap-2 overflow-x-auto pb-4 mb-4 text-xs font-semibold">
        <button className="px-4 py-2 bg-blue-600 text-white rounded-full">All</button>
        <button className="px-4 py-2 bg-neutral-100 text-neutral-600 rounded-full">In transit</button>
        <button className="px-4 py-2 bg-neutral-100 text-neutral-600 rounded-full">Delivered</button>
      </div>

      <div className="space-y-4">
        {orders.map((o) => (
          <div key={o.id} className="border p-4 rounded-sm">
            <div className="flex justify-between items-start mb-3">
              <div>
                <p className="text-xs font-bold text-neutral-900">{o.id}</p>
                <p className="text-[10px] text-neutral-500">{o.date} • {o.items} items</p>
              </div>
              <span className={`text-[9px] font-bold px-2 py-0.5 rounded ${o.color}`}>{o.status}</span>
            </div>
            <div className="flex justify-between items-center pt-2 border-t">
              <span className="text-sm font-bold">${o.total.toFixed(2)}</span>
              <button className="px-4 py-2 bg-neutral-950 text-white text-[10px] font-bold uppercase tracking-wider">
                Track Order
              </button>
            </div>
          </div>
        ))}
      </div>
    </main>
  );
}