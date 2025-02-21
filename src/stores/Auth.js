import axios from "@/plugins/axios";
import { defineStore } from "pinia";
import { LOCAL_STORAGE_TOKEN_KEY, LOCAL_STORAGE_USER_KEY } from "@/config";
import { saveToStorage, getFromStorage } from "@/@core/utils/helpers";

export const useAuthStore = defineStore("AuthStore", {
  state: () => {
    return {
      token: getFromStorage(LOCAL_STORAGE_TOKEN_KEY),
      user: getFromStorage(LOCAL_STORAGE_USER_KEY),
    };
  },
  getters: {
    currentUser: (state) => {
      if (state.user) return state.user;

      return getFromStorage(LOCAL_STORAGE_USER_KEY);
    },
  },
  actions: {
    login(data) {
      return axios.post("auth/web/login", data);
    },
    logOut() {
      return axios.post("auth/web/logout");
    },
    resetPassword({ phone }) {
      return axios.post("auth/app/reset-password", { phone });
    },
    verifyOTP(data) {
      return axios.post("auth/app/verify-otp", data);
    },
    updatePassword(data) {
      return axios.post("auth/app/update-password", data);
    },
    updateUser(user) {
      this.user = user;
      saveToStorage(LOCAL_STORAGE_USER_KEY, user);
    },
    saveToken(token) {
      this.token = token;
      saveToStorage(LOCAL_STORAGE_TOKEN_KEY, token);
    },
  },
});
