const ANON_KEY = 'luxe_anonymous_id';

export const getOrCreateAnonymousId = (): string => {
  if (typeof window === 'undefined') return '';

  let anonId = localStorage.getItem(ANON_KEY);
  if (!anonId) {
    anonId = crypto.randomUUID();
    localStorage.setItem(ANON_KEY, anonId);
  }
  return anonId;
};

export const clearAnonymousId = () => {
  if (typeof window !== 'undefined') {
    localStorage.removeItem(ANON_KEY);
  }
};