import Image from "next/image"
import Link from "next/link"
import { getCategoryTree } from "@/lib/api/catalog"

export async function CategoryShowcase() {
  const tree = await getCategoryTree()
  const categories = tree.filter((category) => category.isActive).slice(0, 4)

  if (categories.length === 0) return null

  // Repeated enough times that the track stays well wider than any viewport — otherwise the
  // 2x-duplicated loop runs out of content mid-scroll and the restart reads as a visible snap
  // back to the start instead of a continuous flow.
  const repeated = Array.from({ length: Math.max(1, Math.ceil(12 / categories.length)) }, () => categories).flat()
  const track = [...repeated, ...repeated]

  return (
    <section className="border-b border-border">
      <div className="px-6 py-8 md:px-10">
        <p className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">Keşfet</p>
        <h2 className="mt-2 font-serif text-3xl font-medium tracking-tight md:text-4xl">Kategoriler</h2>
      </div>

      <div className="overflow-hidden pb-16">
        <div className="flex w-max gap-6 px-6 animate-[marquee-reverse_60s_linear_infinite] md:px-10">
          {track.map((category, i) => (
            <Link
              key={`${category.id}-${i}`}
              href={`/shop?categoryId=${category.id}`}
              aria-hidden={i >= repeated.length}
              tabIndex={i >= repeated.length ? -1 : undefined}
              className="group block w-56 shrink-0 md:w-64"
            >
              <div className="relative flex aspect-[3/4] flex-col justify-between overflow-hidden border border-border bg-secondary p-5 transition-colors group-hover:border-foreground">
                {category.imageUrl && (
                  <>
                    <Image
                      src={category.imageUrl}
                      alt={category.name}
                      fill
                      sizes="(max-width: 768px) 224px, 256px"
                      className="object-cover transition-transform duration-500 group-hover:scale-105"
                    />
                    <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-black/10 to-transparent" />
                  </>
                )}
                <span className={`relative font-mono text-xs ${category.imageUrl ? "text-white/80" : "text-muted-foreground"}`}>
                  {String((i % categories.length) + 1).padStart(2, "0")}
                </span>
                <div className="relative">
                  <h3 className={`font-serif text-xl font-medium tracking-tight ${category.imageUrl ? "text-white" : "text-foreground"}`}>
                    {category.name}
                  </h3>
                  <div
                    className={`mt-3 flex items-center gap-2 text-[10px] font-medium uppercase tracking-widest transition-colors ${
                      category.imageUrl ? "text-white/70 group-hover:text-white" : "text-muted-foreground group-hover:text-foreground"
                    }`}
                  >
                    Koleksiyonu Gör
                    <span className="h-px w-4 bg-current transition-all duration-300 group-hover:w-8" />
                  </div>
                </div>
              </div>
            </Link>
          ))}
        </div>
      </div>
    </section>
  )
}
