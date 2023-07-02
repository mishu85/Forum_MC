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

async function register(userName, password) {
  try {
    var request = {
      userName: userName,
      password: password,
    };
    const response = await ApiHttpClient().post("/accounts/register", request);
    if (response.status === 201) {
      return true;
    }
  } catch (error) {
    console.log(error.message);
  }
  return false;
}

export { login, register };
