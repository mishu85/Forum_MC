import axios from "axios";
import Auth from "./auth";

const serverAddress = "http://localhost:6600";

const ApiHttpClient = () => {
  let client = axios.create({ baseURL: serverAddress + "/api" });
  if (Auth.getInstance().isAuthenticated()) {
    client.defaults.headers.common["Authorization"] =
      "Bearer " + Auth.getInstance().getMyUser().token;
  }
  return client;
};

export { ApiHttpClient, serverAddress };
