const COLOR_ATTRIBUTE_NAMES = ["renk", "color", "colour"]

export function isColorAttributeName(name: string) {
  return COLOR_ATTRIBUTE_NAMES.includes(name.trim().toLowerCase())
}

// input[type=color] sadece #rrggbb formatını kabul eder — admin panelinde renk seçiciyle
// atanan değerler zaten bu formatta gelir, ama elle girilmiş/eksik bir değer varsa güvenli varsayılana düşürüyoruz.
export function toHexColorOrDefault(value: string, fallback = "#000000") {
  return /^#[0-9a-fA-F]{6}$/.test(value) ? value : fallback
}
