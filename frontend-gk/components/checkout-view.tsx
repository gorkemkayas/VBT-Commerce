"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { X } from "lucide-react"
import { useAuth } from "@/lib/auth-context"
import { useCart } from "@/components/cart-provider"
import { getOrCreateAnonymousId } from "@/lib/anonymous-id"
import { getMyProfile } from "@/lib/api/customers"
import { createGuestCustomer } from "@/lib/api/customers"
import { getActiveShippingCompanies } from "@/lib/api/shipping"
import { calculateGuestOrderPrice, calculateMyOrderPrice } from "@/lib/api/pricing"
import { placeGuestOrder, placeMyOrder } from "@/lib/api/orders"
import { ApiError } from "@/lib/api/client"
import type { CustomerAddress, PriceCalculationResult, ShippingCompany } from "@/lib/api/types"
import { formatPrice } from "@/lib/format"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { FormMessage } from "@/components/ui/form-message"
import { PageSpinner } from "@/components/ui/spinner"

type GuestInfo = {
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  recipientName: string
  country: string
  city: string
  district: string
  postalCode: string
  addressLine1: string
  addressLine2: string
}

type GuestBillingInfo = {
  recipientName: string
  country: string
  city: string
  district: string
  postalCode: string
  addressLine1: string
  addressLine2: string
}

type CardInfo = {
  cardHolderName: string
  cardNumber: string
  cardExpireMonth: string
  cardExpireYear: string
  cardCvc: string
  buyerIdentityNumber: string
}

const emptyGuestInfo: GuestInfo = {
  firstName: "",
  lastName: "",
  email: "",
  phoneNumber: "",
  recipientName: "",
  country: "Türkiye",
  city: "",
  district: "",
  postalCode: "",
  addressLine1: "",
  addressLine2: "",
}

const emptyGuestBillingInfo: GuestBillingInfo = {
  recipientName: "",
  country: "Türkiye",
  city: "",
  district: "",
  postalCode: "",
  addressLine1: "",
  addressLine2: "",
}

const emptyCard: CardInfo = {
  cardHolderName: "",
  cardNumber: "",
  cardExpireMonth: "",
  cardExpireYear: "",
  cardCvc: "",
  buyerIdentityNumber: "",
}

export function CheckoutView() {
  const router = useRouter()
  const { isAuthenticated, bootstrapping } = useAuth()
  const { items, subtotal, loading: cartLoading, refresh: refreshCart } = useCart()

  const [step, setStep] = useState<1 | 2>(1)
  const [loadingPrereqs, setLoadingPrereqs] = useState(true)
  const [companies, setCompanies] = useState<ShippingCompany[]>([])
  const [addresses, setAddresses] = useState<CustomerAddress[]>([])

  const [addressId, setAddressId] = useState("")
  const [sameBillingAddress, setSameBillingAddress] = useState(true)
  const [billingAddressId, setBillingAddressId] = useState("")
  const [guestBillingInfo, setGuestBillingInfo] = useState<GuestBillingInfo>(emptyGuestBillingInfo)
  const [shippingCompanyId, setShippingCompanyId] = useState("")
  const [couponInput, setCouponInput] = useState("")
  const [couponCodes, setCouponCodes] = useState<string[]>([])
  const [guestInfo, setGuestInfo] = useState<GuestInfo>(emptyGuestInfo)
  const [card, setCard] = useState<CardInfo>(emptyCard)

  const [guestCustomerId, setGuestCustomerId] = useState<string | null>(null)
  const [calculation, setCalculation] = useState<PriceCalculationResult | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    if (bootstrapping) return
    let cancelled = false
    ;(async () => {
      try {
        const [companiesResult, profileResult] = await Promise.all([
          getActiveShippingCompanies(),
          isAuthenticated ? getMyProfile().catch(() => null) : Promise.resolve(null),
        ])
        if (cancelled) return
        setCompanies(companiesResult)
        if (companiesResult.length > 0) setShippingCompanyId(companiesResult[0].id)
        if (profileResult) {
          setAddresses(profileResult.addresses)
          const def = profileResult.addresses.find((a) => a.isDefault) ?? profileResult.addresses[0]
          if (def) setAddressId(def.id)
        }
      } finally {
        if (!cancelled) setLoadingPrereqs(false)
      }
    })()
    return () => {
      cancelled = true
    }
  }, [isAuthenticated, bootstrapping])

  function addCoupon() {
    const code = couponInput.trim().toUpperCase()
    if (code && !couponCodes.includes(code)) setCouponCodes((prev) => [...prev, code])
    setCouponInput("")
  }

  function removeCoupon(code: string) {
    setCouponCodes((prev) => prev.filter((c) => c !== code))
  }

  const priceItems = items.map((i) => ({ sellableItemId: i.sellableItemId, sellableItemType: i.sellableItemType, quantity: i.quantity }))

  async function handleContinue() {
    setError(null)
    if (!shippingCompanyId) {
      setError("Lütfen bir kargo firması seçin.")
      return
    }
    if (isAuthenticated && !addressId) {
      setError("Lütfen bir teslimat adresi seçin.")
      return
    }
    if (isAuthenticated && !sameBillingAddress && !billingAddressId) {
      setError("Lütfen bir fatura adresi seçin.")
      return
    }
    if (!isAuthenticated && !sameBillingAddress) {
      const requiredBilling: (keyof GuestBillingInfo)[] = ["recipientName", "city", "district", "postalCode", "addressLine1"]
      if (requiredBilling.some((k) => !guestBillingInfo[k].trim())) {
        setError("Lütfen fatura adresi alanlarını doldurun.")
        return
      }
    }
    if (!isAuthenticated) {
      const required: (keyof GuestInfo)[] = [
        "firstName",
        "lastName",
        "email",
        "phoneNumber",
        "recipientName",
        "city",
        "district",
        "postalCode",
        "addressLine1",
      ]
      if (required.some((k) => !guestInfo[k].trim())) {
        setError("Lütfen tüm zorunlu alanları doldurun.")
        return
      }
    }

    setSubmitting(true)
    try {
      if (isAuthenticated) {
        const result = await calculateMyOrderPrice(priceItems, couponCodes)
        setCalculation(result)
      } else {
        let gcId = guestCustomerId
        if (!gcId) {
          gcId = await createGuestCustomer({
            firstName: guestInfo.firstName,
            lastName: guestInfo.lastName,
            email: guestInfo.email,
            phoneNumber: guestInfo.phoneNumber,
          })
          setGuestCustomerId(gcId)
        }
        const result = await calculateGuestOrderPrice(gcId, priceItems, couponCodes)
        setCalculation(result)
      }
      setStep(2)
      window.scrollTo({ top: 0, behavior: "smooth" })
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Fiyat hesaplanamadı.")
    } finally {
      setSubmitting(false)
    }
  }

  async function handlePlaceOrder(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSubmitting(true)
    try {
      let orderId: string
      if (isAuthenticated) {
        orderId = await placeMyOrder({
          addressId,
          billingAddressId: sameBillingAddress ? null : billingAddressId,
          shippingCompanyId,
          couponCodes,
          ...card,
        })
        await refreshCart()
        router.push(`/checkout/${orderId}`)
      } else {
        orderId = await placeGuestOrder({
          guestCustomerId: guestCustomerId!,
          anonymousId: getOrCreateAnonymousId(),
          shippingCompanyId,
          couponCodes,
          recipientName: guestInfo.recipientName,
          phoneNumber: guestInfo.phoneNumber,
          country: guestInfo.country,
          city: guestInfo.city,
          district: guestInfo.district,
          postalCode: guestInfo.postalCode,
          addressLine1: guestInfo.addressLine1,
          addressLine2: guestInfo.addressLine2 || null,
          ...(sameBillingAddress
            ? {}
            : {
                billingRecipientName: guestBillingInfo.recipientName,
                billingCountry: guestBillingInfo.country,
                billingCity: guestBillingInfo.city,
                billingDistrict: guestBillingInfo.district,
                billingPostalCode: guestBillingInfo.postalCode,
                billingAddressLine1: guestBillingInfo.addressLine1,
                billingAddressLine2: guestBillingInfo.addressLine2 || null,
              }),
          ...card,
        })
        await refreshCart()
        router.push(`/checkout/${orderId}?guestCustomerId=${guestCustomerId}`)
      }
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Sipariş oluşturulamadı.")
      setSubmitting(false)
    }
  }

  const selectedCompany = companies.find((c) => c.id === shippingCompanyId)
  const shippingFee = selectedCompany?.fee ?? 0
  const grandTotal = (calculation?.grandTotal ?? subtotal) + (calculation ? shippingFee : 0)

  if (bootstrapping || cartLoading || loadingPrereqs) return <PageSpinner label="Ödeme sayfası hazırlanıyor…" />

  if (items.length === 0) {
    return (
      <div className="mx-auto flex min-h-[50vh] max-w-7xl flex-col items-center justify-center px-4 text-center">
        <h1 className="font-serif text-3xl font-medium tracking-tight">Sepetiniz Boş</h1>
        <p className="mt-3 text-sm text-muted-foreground">Ödeme adımına geçmeden önce sepetinize ürün ekleyin.</p>
        <Link href="/shop" className="mt-8 border border-foreground px-8 py-4 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background">
          Alışverişe Başla
        </Link>
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-7xl px-4 py-10 md:px-6">
      <div className="flex items-center gap-3 border-b border-border pb-8">
        <h1 className="font-serif text-3xl font-medium tracking-tight md:text-4xl">Ödeme</h1>
        <span className="text-xs uppercase tracking-widest text-muted-foreground">
          Adım {step}/2 — {step === 1 ? "Teslimat" : "Ödeme"}
        </span>
      </div>

      <div className="grid grid-cols-1 gap-10 py-10 lg:grid-cols-3">
        <div className="lg:col-span-2">
          {step === 1 ? (
            <div className="space-y-8">
              {!isAuthenticated && (
                <section>
                  <h2 className="text-xs font-medium uppercase tracking-widest">Misafir Bilgileri</h2>
                  <p className="mt-1 text-xs text-muted-foreground">
                    Zaten hesabınız var mı?{" "}
                    <Link href="/login?redirect=/checkout" className="underline underline-offset-4">
                      Giriş yapın
                    </Link>
                  </p>
                  <div className="mt-4 grid grid-cols-1 gap-4 sm:grid-cols-2">
                    <Field label="Ad">
                      <Input required value={guestInfo.firstName} onChange={(e) => setGuestInfo({ ...guestInfo, firstName: e.target.value })} />
                    </Field>
                    <Field label="Soyad">
                      <Input required value={guestInfo.lastName} onChange={(e) => setGuestInfo({ ...guestInfo, lastName: e.target.value })} />
                    </Field>
                    <Field label="E-posta">
                      <Input type="email" required value={guestInfo.email} onChange={(e) => setGuestInfo({ ...guestInfo, email: e.target.value })} />
                    </Field>
                    <Field label="Telefon">
                      <Input required value={guestInfo.phoneNumber} onChange={(e) => setGuestInfo({ ...guestInfo, phoneNumber: e.target.value })} />
                    </Field>
                  </div>

                  <h2 className="mt-8 text-xs font-medium uppercase tracking-widest">Teslimat Adresi</h2>
                  <div className="mt-4 grid grid-cols-1 gap-4 sm:grid-cols-2">
                    <Field label="Alıcı Adı Soyadı" className="sm:col-span-2">
                      <Input required value={guestInfo.recipientName} onChange={(e) => setGuestInfo({ ...guestInfo, recipientName: e.target.value })} />
                    </Field>
                    <Field label="Ülke">
                      <Input required value={guestInfo.country} onChange={(e) => setGuestInfo({ ...guestInfo, country: e.target.value })} />
                    </Field>
                    <Field label="Şehir">
                      <Input required value={guestInfo.city} onChange={(e) => setGuestInfo({ ...guestInfo, city: e.target.value })} />
                    </Field>
                    <Field label="İlçe">
                      <Input required value={guestInfo.district} onChange={(e) => setGuestInfo({ ...guestInfo, district: e.target.value })} />
                    </Field>
                    <Field label="Posta Kodu">
                      <Input required value={guestInfo.postalCode} onChange={(e) => setGuestInfo({ ...guestInfo, postalCode: e.target.value })} />
                    </Field>
                    <Field label="Adres Satırı 1" className="sm:col-span-2">
                      <Input required value={guestInfo.addressLine1} onChange={(e) => setGuestInfo({ ...guestInfo, addressLine1: e.target.value })} />
                    </Field>
                    <Field label="Adres Satırı 2 (opsiyonel)" className="sm:col-span-2">
                      <Input value={guestInfo.addressLine2} onChange={(e) => setGuestInfo({ ...guestInfo, addressLine2: e.target.value })} />
                    </Field>
                  </div>

                  <h2 className="mt-8 text-xs font-medium uppercase tracking-widest">Fatura Adresi</h2>
                  <label className="mt-4 flex items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={sameBillingAddress}
                      onChange={(e) => setSameBillingAddress(e.target.checked)}
                      className="accent-foreground"
                    />
                    Fatura adresim teslimat adresimle aynı
                  </label>
                  {!sameBillingAddress && (
                    <div className="mt-4 grid grid-cols-1 gap-4 sm:grid-cols-2">
                      <Field label="Alıcı Adı Soyadı" className="sm:col-span-2">
                        <Input
                          required
                          value={guestBillingInfo.recipientName}
                          onChange={(e) => setGuestBillingInfo({ ...guestBillingInfo, recipientName: e.target.value })}
                        />
                      </Field>
                      <Field label="Ülke">
                        <Input required value={guestBillingInfo.country} onChange={(e) => setGuestBillingInfo({ ...guestBillingInfo, country: e.target.value })} />
                      </Field>
                      <Field label="Şehir">
                        <Input required value={guestBillingInfo.city} onChange={(e) => setGuestBillingInfo({ ...guestBillingInfo, city: e.target.value })} />
                      </Field>
                      <Field label="İlçe">
                        <Input required value={guestBillingInfo.district} onChange={(e) => setGuestBillingInfo({ ...guestBillingInfo, district: e.target.value })} />
                      </Field>
                      <Field label="Posta Kodu">
                        <Input
                          required
                          value={guestBillingInfo.postalCode}
                          onChange={(e) => setGuestBillingInfo({ ...guestBillingInfo, postalCode: e.target.value })}
                        />
                      </Field>
                      <Field label="Adres Satırı 1" className="sm:col-span-2">
                        <Input
                          required
                          value={guestBillingInfo.addressLine1}
                          onChange={(e) => setGuestBillingInfo({ ...guestBillingInfo, addressLine1: e.target.value })}
                        />
                      </Field>
                      <Field label="Adres Satırı 2 (opsiyonel)" className="sm:col-span-2">
                        <Input
                          value={guestBillingInfo.addressLine2}
                          onChange={(e) => setGuestBillingInfo({ ...guestBillingInfo, addressLine2: e.target.value })}
                        />
                      </Field>
                    </div>
                  )}
                </section>
              )}

              {isAuthenticated && (
                <section>
                  <h2 className="text-xs font-medium uppercase tracking-widest">Teslimat Adresi</h2>
                  {addresses.length === 0 ? (
                    <FormMessage tone="error">
                      Kayıtlı adresiniz yok.{" "}
                      <Link href="/account?tab=addresses" className="underline underline-offset-4">
                        Hesabım
                      </Link>{" "}
                      sayfasından adres ekleyin.
                    </FormMessage>
                  ) : (
                    <div className="mt-4 grid grid-cols-1 gap-3 sm:grid-cols-2">
                      {addresses.map((addr) => (
                        <button
                          key={addr.id}
                          type="button"
                          onClick={() => setAddressId(addr.id)}
                          className={`border p-4 text-left text-sm transition-colors ${
                            addressId === addr.id ? "border-foreground" : "border-border hover:border-foreground/50"
                          }`}
                        >
                          <p className="font-medium">{addr.label}</p>
                          <p className="mt-1 text-muted-foreground">{addr.recipientName}</p>
                          <p className="text-muted-foreground">
                            {addr.addressLine1}, {addr.district}/{addr.city}
                          </p>
                        </button>
                      ))}
                    </div>
                  )}
                </section>
              )}

              {isAuthenticated && (
                <section>
                  <h2 className="text-xs font-medium uppercase tracking-widest">Fatura Adresi</h2>
                  <label className="mt-4 flex items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={sameBillingAddress}
                      onChange={(e) => setSameBillingAddress(e.target.checked)}
                      className="accent-foreground"
                    />
                    Fatura adresim teslimat adresimle aynı
                  </label>
                  {!sameBillingAddress && (
                    <>
                      {addresses.filter((a) => a.isBillingAddress).length === 0 ? (
                        <FormMessage tone="error">
                          Fatura adresi olarak işaretlenmiş kayıtlı adresiniz yok.{" "}
                          <Link href="/account?tab=addresses" className="underline underline-offset-4">
                            Hesabım
                          </Link>{" "}
                          sayfasından işaretleyin.
                        </FormMessage>
                      ) : (
                        <div className="mt-4 grid grid-cols-1 gap-3 sm:grid-cols-2">
                          {addresses
                            .filter((a) => a.isBillingAddress)
                            .map((addr) => (
                              <button
                                key={addr.id}
                                type="button"
                                onClick={() => setBillingAddressId(addr.id)}
                                className={`border p-4 text-left text-sm transition-colors ${
                                  billingAddressId === addr.id ? "border-foreground" : "border-border hover:border-foreground/50"
                                }`}
                              >
                                <p className="font-medium">{addr.label}</p>
                                <p className="mt-1 text-muted-foreground">{addr.recipientName}</p>
                                <p className="text-muted-foreground">
                                  {addr.addressLine1}, {addr.district}/{addr.city}
                                </p>
                              </button>
                            ))}
                        </div>
                      )}
                    </>
                  )}
                </section>
              )}

              <section>
                <h2 className="text-xs font-medium uppercase tracking-widest">Kargo Firması</h2>
                <div className="mt-4 max-w-sm">
                  <Select value={shippingCompanyId} onChange={(e) => setShippingCompanyId(e.target.value)}>
                    {companies.length === 0 && <option value="">Kargo firması bulunamadı</option>}
                    {companies.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.name} — {formatPrice(c.fee)}
                      </option>
                    ))}
                  </Select>
                </div>
              </section>

              <section>
                <h2 className="text-xs font-medium uppercase tracking-widest">Kupon Kodu</h2>
                <div className="mt-4 flex max-w-sm gap-2">
                  <Input
                    value={couponInput}
                    onChange={(e) => setCouponInput(e.target.value)}
                    placeholder="KUPON10"
                    onKeyDown={(e) => {
                      if (e.key === "Enter") {
                        e.preventDefault()
                        addCoupon()
                      }
                    }}
                  />
                  <button type="button" onClick={addCoupon} className="shrink-0 border border-foreground px-4 text-xs font-medium uppercase tracking-widest hover:bg-foreground hover:text-background">
                    Ekle
                  </button>
                </div>
                {couponCodes.length > 0 && (
                  <div className="mt-3 flex flex-wrap gap-2">
                    {couponCodes.map((code) => (
                      <span key={code} className="flex items-center gap-1.5 border border-border px-2.5 py-1 text-xs">
                        {code}
                        <button type="button" onClick={() => removeCoupon(code)} aria-label={`${code} kuponunu kaldır`}>
                          <X className="h-3 w-3" strokeWidth={1.5} />
                        </button>
                      </span>
                    ))}
                  </div>
                )}
              </section>

              {error && <FormMessage tone="error">{error}</FormMessage>}

              <button
                type="button"
                onClick={handleContinue}
                disabled={submitting}
                className="w-full bg-foreground py-4 text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-80 disabled:opacity-50 sm:w-auto sm:px-10"
              >
                {submitting ? "Hesaplanıyor…" : "Devam Et"}
              </button>
            </div>
          ) : (
            <form onSubmit={handlePlaceOrder} className="space-y-8">
              <section>
                <div className="flex items-center justify-between">
                  <h2 className="text-xs font-medium uppercase tracking-widest">Ödeme Bilgileri</h2>
                  <button type="button" onClick={() => setStep(1)} className="text-xs text-muted-foreground underline underline-offset-4">
                    Teslimat bilgilerini düzenle
                  </button>
                </div>
                <p className="mt-1 text-xs text-muted-foreground">Test ortamı — gerçek kart bilgisi girmeyin (sandbox ödeme sağlayıcısı).</p>
                <div className="mt-4 grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Field label="Kart Üzerindeki İsim" className="sm:col-span-2">
                    <Input required value={card.cardHolderName} onChange={(e) => setCard({ ...card, cardHolderName: e.target.value })} />
                  </Field>
                  <Field label="Kart Numarası" className="sm:col-span-2">
                    <Input required inputMode="numeric" placeholder="5528790000000008" value={card.cardNumber} onChange={(e) => setCard({ ...card, cardNumber: e.target.value })} />
                  </Field>
                  <Field label="Son Kullanma Ay (MM)">
                    <Input required inputMode="numeric" placeholder="12" maxLength={2} value={card.cardExpireMonth} onChange={(e) => setCard({ ...card, cardExpireMonth: e.target.value })} />
                  </Field>
                  <Field label="Son Kullanma Yıl (YYYY)">
                    <Input required inputMode="numeric" placeholder="2030" maxLength={4} value={card.cardExpireYear} onChange={(e) => setCard({ ...card, cardExpireYear: e.target.value })} />
                  </Field>
                  <Field label="CVC">
                    <Input required inputMode="numeric" placeholder="123" maxLength={4} value={card.cardCvc} onChange={(e) => setCard({ ...card, cardCvc: e.target.value })} />
                  </Field>
                  <Field label="TC Kimlik No (Alıcı)">
                    <Input required inputMode="numeric" maxLength={11} value={card.buyerIdentityNumber} onChange={(e) => setCard({ ...card, buyerIdentityNumber: e.target.value })} />
                  </Field>
                </div>
              </section>

              {error && <FormMessage tone="error">{error}</FormMessage>}

              <button
                type="submit"
                disabled={submitting}
                className="w-full bg-foreground py-4 text-xs font-medium uppercase tracking-widest text-background transition-opacity hover:opacity-80 disabled:opacity-50 sm:w-auto sm:px-10"
              >
                {submitting ? "Sipariş oluşturuluyor…" : "Siparişi Tamamla"}
              </button>
            </form>
          )}
        </div>

        <aside className="h-fit border border-border p-6 lg:sticky lg:top-24">
          <h2 className="text-sm font-medium uppercase tracking-widest">Sipariş Özeti</h2>
          <ul className="mt-6 space-y-3 border-b border-border pb-6">
            {items.map((item) => (
              <li key={item.id} className="flex justify-between gap-3 text-sm">
                <span className="text-muted-foreground">
                  {item.name}
                  {item.variantLabel && ` (${item.variantLabel})`} × {item.quantity}
                </span>
                <span className="font-mono">{formatPrice((item.unitPrice ?? 0) * item.quantity)}</span>
              </li>
            ))}
          </ul>
          <dl className="mt-4 space-y-3 border-b border-border pb-6 text-sm">
            <div className="flex justify-between">
              <dt className="text-muted-foreground">Ara Toplam</dt>
              <dd className="font-mono">{formatPrice(calculation?.subtotal ?? subtotal)}</dd>
            </div>
            {calculation && calculation.totalDiscount > 0 && (
              <div className="flex justify-between">
                <dt className="text-muted-foreground">İndirim</dt>
                <dd className="font-mono">−{formatPrice(calculation.totalDiscount)}</dd>
              </div>
            )}
            {calculation && (
              <div className="flex justify-between">
                <dt className="text-muted-foreground">Vergi</dt>
                <dd className="font-mono">{formatPrice(calculation.taxAmount)}</dd>
              </div>
            )}
            <div className="flex justify-between">
              <dt className="text-muted-foreground">Kargo</dt>
              <dd className="font-mono">{calculation ? formatPrice(shippingFee) : "—"}</dd>
            </div>
          </dl>
          <div className="mt-4 flex items-baseline justify-between">
            <span className="text-sm font-medium uppercase tracking-widest">Toplam</span>
            <span className="font-mono text-xl">{formatPrice(grandTotal)}</span>
          </div>
        </aside>
      </div>
    </div>
  )
}

function Field({ label, children, className = "" }: { label: string; children: React.ReactNode; className?: string }) {
  return (
    <div className={className}>
      <Label>{label}</Label>
      {children}
    </div>
  )
}
