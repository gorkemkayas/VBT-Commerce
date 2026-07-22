import axios from 'axios';

const API_BASE_URL = 'https://intern-api.kayas.dev';

// Access token JS Memory'de tutulur
let inMemoryToken: string | null = null;

export function getAccessToken(): string | null {
  return inMemoryToken;
}

export function setAccessToken(token: string | null): void {
  if (token) {
    inMemoryToken = token.startsWith('Bearer ') ? token : `Bearer ${token}`;
  } else {
    inMemoryToken = null;
  }
}

// 🟢 İŞTE EKSİK OLAN FONKSİYON:
export function getOrCreateAnonymousId(): string {
  if (typeof window === 'undefined') return '';
  let anonId = localStorage.getItem('anonymous_id');
  if (!anonId) {
    anonId = crypto.randomUUID();
    localStorage.setItem('anonymous_id', anonId);
  }
  return anonId;
}

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor
apiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = token;
  }
  return config;
});

// Response Interceptor (Otomatik Refresh)
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url?.includes('/api/auth/refresh')
    ) {
      originalRequest._retry = true;

      try {
        const res = await axios.post(
          `${API_BASE_URL}/api/auth/refresh`,
          { platform: 0 }, // Web platform enum (0)
          { withCredentials: true }
        );

        const newAccessToken = res.data?.accessToken;
        if (newAccessToken) {
          setAccessToken(newAccessToken);
          originalRequest.headers.Authorization = getAccessToken();
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        setAccessToken(null);
      }
    }

    return Promise.reject(error);
  }
);