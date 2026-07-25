export function HeroSection() {
  return (
    <section className="relative h-[calc(100svh-40px)] min-h-[560px] w-full overflow-hidden border-b border-border">
      <video
        src="/hero.mp4"
        autoPlay
        muted
        loop
        playsInline
        className="absolute inset-0 h-full w-full object-cover"
      />
      <div className="absolute inset-0 bg-gradient-to-t from-black/55 via-black/0 to-black/0" />

      <div className="absolute inset-x-0 top-0 p-6 text-[11px] font-medium uppercase tracking-[0.25em] text-white/70 md:p-10">
        Sonbahar / Kış 2026
      </div>

      <div className="absolute inset-x-0 bottom-0 flex flex-col gap-5 p-6 md:p-10 lg:p-14">
        <h1 className="max-w-3xl text-balance font-serif text-5xl font-medium leading-[0.95] text-white sm:text-6xl lg:text-7xl">
          Siyah. Beyaz. <span className="italic">Zamansız.</span>
        </h1>
        <a
          href="#koleksiyon"
          className="group inline-flex w-fit items-center gap-2 text-xs font-medium uppercase tracking-[0.2em] text-white"
        >
          Koleksiyonu Keşfet
          <span className="h-px w-6 bg-white transition-all duration-300 group-hover:w-10" />
        </a>
      </div>
    </section>
  )
}
