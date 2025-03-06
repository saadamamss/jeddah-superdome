import { defineStore } from "pinia";
import { LOCAL_STORAGE_USER_KEY } from "@/config";
import {
  getFromStorage,
  saveToStorage,
  removeFromStorage,
} from "@/@core/utils/helpers";
import { authService } from "@/services/auth.service";
import axiosIns from "@/plugins/axios";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: authService.getToken(),
    user: getFromStorage(LOCAL_STORAGE_USER_KEY),
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  actions: {
    setToken(token) {
      this.token = token;
      const currentUser = authService.getStoredUser() || {};
      authService.setUser({ ...currentUser, token });
    },

    setUser(user) {
      this.user = user;
      authService.setUser(user);
    },

    logout() {
      this.token = null;
      this.user = null;
      authService.logout();
    },

    async login(credentials) {
      try {
        const response = await axiosIns.post("/login", credentials);
        const data = response?.data;

        if (!data) {
          throw new Error("No data received from server");
        }

        // Handle both possible response structures
        const token = data.token || data.data?.token;
        const user = data.user || data.data;

        if (!token) {
          throw new Error("No token received from server");
        }

        this.setToken(token);
        this.setUser(user);

        return { token, user };
      } catch (error) {
        console.error("Login error:", error);
        throw error;
      }
    },

    resetPassword({ phone }) {
      return axiosIns.post("auth/app/reset-password", { phone });
    },

    verifyOTP(data) {
      return axiosIns.post("auth/app/verify-otp", data);
    },

    updatePassword(data) {
      return axiosIns.post("auth/app/update-password", data);
    },
  },
});
