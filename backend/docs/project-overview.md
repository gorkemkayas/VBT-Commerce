# Project Overview

## 1. Project Description

An **e-commerce platform** where a single store (single-store) sells its products, and users (registered or guest) can shop.

The store manages its products with category, attributes, images, pricing type, and stock information. Users browse products, add them to the cart, and create an order by entering address/shipping/payment details. The system covers end-to-end e-commerce flows including inventory management, tax calculation, coupons/discounts, shipping integration, payment (iyzico), and notifications.

See `architecture.md` for the architectural approach (Modular Monolith + DDD + CQRS).

---

## 2. Scope

### Included
- Product catalog management (category, attributes, variants, images, pricing type)
- Inventory management and reservation
- User registration/login + guest checkout
- Customer profile and multiple address management
- Cart management
- Order creation and status tracking
- Tax calculation
- Coupon / discount system
- Shipping company integration and delivery tracking
- Payment infrastructure (iyzico sandbox)
- Email notifications
- Product reviews and ratings (purchasers only)

### Not Included For Now (May Be Considered Later)
- Multi-vendor / marketplace structure (multiple sellers)
- SMS / push notification
- Guest → Registered account conversion flow
- Review moderation/approval mechanism
- Multi-currency / multi-language support

---

## 3. Modules

| # | Module | Responsibility |
|---|--------|-----------------|
| 1 | **Catalog** | Product, category, variant, image, pricing type management |
| 2 | **Inventory** | Stock tracking, reservation (synchronous check, safety net via `ExpiresAt`) |
| 3 | **Identity** | Authentication, registration/login (details in `auth-and-authorization.md`) |
| 4 | **Customer** | Customer profile, address management, guest customer record |
| 5 | **Cart** | Cart and cart item management |
| 6 | **Order** | Order creation, status management (state machine) |
| 7 | **Pricing & Tax** | Tax calculation, coupons/discounts (details in Section 4.1), price calculation engine |
| 8 | **Shipping** | Shipping company management (manual/static — no API integration in MVP), fee calculation, delivery status (manual update) |
| 9 | **Payment** | iyzico integration, payment/refund operations |
| 10 | **Notification** | Email notifications (triggered via Outbox Pattern + Event) |
| 11 | **Review** | Product reviews and ratings (purchasers only, published instantly without moderation) |

---

## 4. Key Product Decisions

| Topic | Decision |
|-------|----------|
| Cart | Treated as a separate module (not part of Order) |
| Coupon / discount | Yes — see Section 4.1 for details |
| Guest checkout | Yes — registration is not mandatory, guest customer flow is supported |
| Who can write a product review | Only users who have purchased the product (the Review module queries purchase information from the Order module via Integration) |
| Review moderation | None — reviews are published instantly |
| Shipping integration (MVP) | **Manual / static** — the shipping company is selected during checkout (informational only), no real-time API integration (label creation, automatic tracking, etc.) is implemented. Delivery status is updated manually by the relevant person through the system. Rationale: access to shipping company APIs requires a prior commercial agreement, which is not needed at the MVP stage |

### 4.1. Coupon System Rules

| Rule | Decision |
|------|----------|
| Discount type | Percentage **or** fixed amount — determined per coupon |
| Applicability scope | Entire cart / specific category / specific product (selected per coupon) |
| Minimum cart amount | Optional, can be defined per coupon |
| Maximum discount amount | Optional (especially as an upper bound for percentage-based coupons) |
| Usage limit | Both **total usage limit** and **per-user usage limit** are supported |
| Validity period | Limited by a start and end date |
| Stacking | Multiple coupons can be used together on the same order |

> **Note:** Since stacking is enabled, the price calculation engine must include an upper bound/validation rule to ensure the total discount does not exceed the cart amount (e.g. total discount cannot exceed 100% of the product total).

**Possible Entities:**
```
Coupon
  - Code, DiscountType (Percentage / FixedAmount), DiscountValue
  - MaxDiscountAmount, MinCartAmount
  - ScopeType (Cart / Category / Product), ScopeReferenceId
  - StartDate, EndDate
  - TotalUsageLimit, PerUserUsageLimit

CouponUsage
  - CouponId, CustomerId/GuestId, OrderId, UsedAt
  (for usage limit tracking)
```

---

## 5. Example End-to-End Flow

```
1. User browses products (Catalog)
2. Adds a product to the cart (Cart)
3. Checkout begins:
   a. Address is entered/selected (Customer) — temporary info is collected if guest
   b. Shipping option is selected (Shipping)
   c. Coupon is applied, total including tax + shipping is calculated (Pricing & Tax)
4. Order is created (Order)
   → Synchronous: Stock check/reservation (Inventory, via Integration)
5. Payment is taken (Payment → iyzico sandbox)
   → Success: Order moves to "Confirmed" status
   → Failure: Stock reservation is released (Compensating Action)
6. Once the order is confirmed (asynchronous, via Outbox + Event):
   → Email notification is sent (Notification)
   → Shipping company info is recorded on the order (Shipping — manual, no API integration)
7. As delivery progresses, the order status is updated manually by the relevant person
8. After delivery, the purchasing user can leave a review/rating for the product (Review)
```

---

## 6. Related Documents

- `architecture.md` — Backend architecture decisions (Modular Monolith, DDD, CQRS, Pipeline Behaviors, Transaction strategy, Outbox Pattern)
- `auth-and-authorization.md` — Authentication and authorization strategy
- Frontend/Mobile teams' own documents (managed outside/separately from this repository)

---

## 7. Open Items / TODO

- [ ] Other topics to be discussed will be added here

---

*This document will continue to be updated as the project evolves.*
