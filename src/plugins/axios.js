import axios from "axios";
import { API_BASE_URL, LOCAL_STORAGE_TOKEN_KEY } from "@/config";
import { getFromStorage } from "@/@core/utils/helpers";

const axiosIns = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    Accept: "application/json",
    Authorization: `Bearer ${getFromStorage(LOCAL_STORAGE_TOKEN_KEY)}`,
  },
});

axiosIns.interceptors.response.use(
  (response) => {
    axiosIns.defaults.headers.Authorization = `Bearer ${getFromStorage(
      LOCAL_STORAGE_TOKEN_KEY
    )}`;

    return response;
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      return Promise.reject("Unauthorized");
    }

    return Promise.reject(error);
  }
);

export default axiosIns;
