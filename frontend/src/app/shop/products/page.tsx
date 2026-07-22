'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams } from 'next/navigation';
import { productService } from '@/services/product.service';
import { cartService } from '@/services/cart.service';
import { getAccessToken } from '@/lib/api-client';
import { Product } from '@/types/api';

export default function ProductDetailPage() {
  const params = useParams();
  const slug = params?.slug as string;

  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedSize, setSelectedSize] = useState('M');
  const [added, setAdded] = useState(false);

  const isLoggedIn = !!getAccessToken();

  useEffect(() => {
    if (slug) {
      productService.getProductBySlug(slug).then((data) => {
        setProduct(data);
        setLoading(false);
      });
    }
  }, [slug]);

  const handleAddToCart = async () => {
    if (!product) return;

    // API'nin beklediği itemId (id veya ilk variant/stockItem)
    const targetItemId = product.id;

    try {
      await cartService.addItem({
        sellableItemId: targetItemId,
        sellableItemType: 1,
        quantity: 1
     });
      setAdded(true);
      setTimeout(() => setAdded(false), 2000);
    } catch (error) {
      console.error('Sepete eklenirken hata oluştu:', error);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center font-sans text-sm text-neutral-500">
        Ürün detayları yükleniyor...
      </div>
    );
  }

  return (
    <main className="max-w-md mx-auto min-h-screen bg-white font-sans border-x flex flex-col justify-between">
      <div>
        <header className="p-4 flex justify-between items-center border-b">
          <Link href="/shop/home" className="text-sm font-bold">
            ← Back
          </Link>
          <span className="font-bold text-lg tracking-widest text-black">LUXE</span>
          <span className="w-8"></span>
        </header>

        {/* Product Image */}
        <div className="aspect-[3/4] bg-neutral-100 w-full overflow-hidden flex items-center justify-center text-xs text-neutral-400">
          {product?.images && product.images.length > 0 ? (
            <img
              src={product.images[0].url}
              alt={product.name}
              className="w-full h-full object-cover"
            />
          ) : (
            `[ Görsel: ${product?.slug || 'Luxe Product'} ]`
          )}
        </div>

        {/* Product Details */}
        <div className="p-6">
          <h1 className="text-xl font-medium text-neutral-900">
            {product?.name || 'Lüks Ürün'}
          </h1>
          <p className="text-lg font-semibold text-neutral-900 mt-1">
            ${product?.price ? Number(product.price).toFixed(2) : '0.00'}
          </p>

          {/* Size Selector */}
          <div className="mt-6">
            <p className="text-xs font-semibold text-neutral-500 uppercase tracking-wider mb-2">
              Select Size
            </p>
            <div className="grid grid-cols-5 gap-2">
              {['XS', 'S', 'M', 'L', 'XL'].map((size) => (
                <button
                  key={size}
                  onClick={() => setSelectedSize(size)}
                  className={`py-3 text-xs font-medium border ${
                    selectedSize === size
                      ? 'border-black text-black bg-neutral-100 font-bold'
                      : 'border-neutral-200 text-neutral-800'
                  }`}
                >
                  {size}
                </button>
              ))}
            </div>
          </div>

          {/* Description */}
          <div className="mt-6">
            <p className="text-xs font-semibold text-neutral-500 uppercase tracking-wider mb-1">
              Description
            </p>
            <p className="text-xs text-neutral-600 leading-relaxed">
              {product?.description ||
                'An exercise in precision. Architectural shoulders and a tapered waist in high-twist Italian wool.'}
            </p>
          </div>
        </div>
      </div>

      {/* Add To Cart Button */}
      <div className="p-4 border-t bg-white">
        <button
          onClick={handleAddToCart}
          className="w-full py-4 bg-neutral-950 text-white text-xs font-bold tracking-widest uppercase hover:bg-neutral-800 transition-colors"
        >
          {added ? 'Added To Bag ✓' : 'Add To Cart'}
        </button>
      </div>
    </main>
  );
}