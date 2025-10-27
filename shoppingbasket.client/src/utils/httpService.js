import axios from "axios";

const http = axios.create({
  baseURL: "https://localhost:51479",
  headers: {
    "Content-Type": "application/json",
  }
});

http.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handling of errors
    console.error("HTTP error:", error);
    return Promise.reject(error);
  }
);

// Export simple async/await wrappers
//TODO: remove config params
export const httpService = {
  get: async (url, config) => {
    const response = await http.get(url, config);
    return response.data;
  },
  post: async (url, data, config) => {
    const response = await http.post(url, data, config);
    return response.data;
  },
  put: async (url, data, config) => {
    const response = await http.put(url, data, config);
    return response.data;
  },
  delete: async (url, config) => {
    const response = await http.delete(url, config);
    return response.data;
  },
};

export default httpService;