import axios from "axios";
import { API_BASE_URL } from "@/config";
import { authService } from "@/services/auth.service";

const axiosIns = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    Accept: "application/json",
  },
});

// Request interceptor for API calls
axiosIns.interceptors.request.use(
  (config) => {
    const token = authService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for API calls
axiosIns.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Handle 401 Unauthorized errors
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      // Handle token refresh or logout here
      authService.logout();
      // window.location.href = "/auth/login";

      return Promise.reject("Unauthorized");
    }

    return Promise.reject(error);
  }
);

export default axiosIns;
