import Image from "next/image"

// Purely decorative — fills the empty gutters that appear on very wide screens once content is
// capped at max-w-7xl/6xl, with a tall grayscale editorial photo strip. Hidden below 2xl, where
// the gutter is too narrow to hold it without crowding the content. Absolutely positioned within
// whatever (relative) wrapper it's rendered in, so it spans exactly that section's height rather
// than the full page (avoids overlapping the header/footer).
export function SideRails() {
  return (
    <div aria-hidden className="pointer-events-none absolute inset-y-0 inset-x-0 z-0 hidden 2xl:block">
      <div className="absolute inset-y-0 left-0 w-28 overflow-hidden border-r border-border">
        <Image src="/editorial.png" alt="" fill sizes="112px" className="object-cover grayscale opacity-40" />
      </div>
      <div className="absolute inset-y-0 right-0 w-28 overflow-hidden border-l border-border">
        <Image src="/editorial.png" alt="" fill sizes="112px" className="object-cover grayscale opacity-40" />
      </div>
    </div>
  )
}
