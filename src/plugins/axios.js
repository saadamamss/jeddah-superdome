import axios from "axios";

const token = localStorage.getItem("SAR_TOKEN") || "";

const axiosIns = axios.create({
  baseURL: "/api/v2", // test api
  headers: {
    Accept: "application/json",
    Authorization: `Bearer ${token}`,
  },
});

axiosIns.interceptors.response.use(
  (response) => {
    const token = localStorage.getItem("SAR_TOKEN") || "";

    axiosIns.defaults.headers.Authorization = `Bearer ${token}`;

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
