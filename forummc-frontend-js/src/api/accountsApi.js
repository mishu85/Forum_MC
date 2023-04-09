import { ApiHttpClient } from "../HttpClient";

async function login(userName, password) {
  try {
    var request = {
      userName: userName,
      password: password,
    };
    const response = await ApiHttpClient().post("/accounts/login", request);
    return response.data;
  } catch (error) {
    console.log(error.message);
  }
  return null;
}

export { login };
