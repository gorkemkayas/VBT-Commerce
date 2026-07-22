import React from 'react';

export const metadata = {
  title: 'Luxe Storefront',
  description: 'Luxury E-Commerce App',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="tr">
      <body>{children}</body>
    </html>
  );
}