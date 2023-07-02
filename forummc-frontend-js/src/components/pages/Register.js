import Layout from "../Layout";
import React, { useState } from "react";
import "./Register.css";
import { TextField, Button } from "@mui/material";
import { login, register } from "../../api/accountsApi";
import Auth from "../../auth";
import { useNavigate } from "react-router-dom";

function Register() {
  const navigate = useNavigate();
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError("");
    const responseRegister = await register(userName, password);
    if (responseRegister) {
      const responseLogin = await login(userName, password);
      if (responseLogin) {
        let myUser = {
          userName: userName,
          token: responseLogin,
        };
        Auth.getInstance().login(myUser);
        console.log("logged in!");
        console.log(responseLogin);
        navigate("/");
      }
    } else {
      setError("Register was unsuccesful!");
    }
  };

  return (
    <Layout>
      <h1>Register</h1>
      <form class="register" onSubmit={handleSubmit}>
        <TextField
          label="UserName"
          placeholder="Type UserName"
          required
          fullWidth
          value={userName}
          onChange={(e) => {
            setUserName(e.target.value);
          }}
        />
        <TextField
          label="Password"
          placeholder="Enter code"
          type="password"
          required
          value={password}
          fullWidth
          onChange={(e) => {
            setPassword(e.target.value);
          }}
        />
        <Button
          // id = "signIn-style"
          fullWidth
          variant="contained"
          type="submit"
        >
          REGISTER
        </Button>
        {error !== "" ? <p>{error}</p> : null}
      </form>
    </Layout>
  );
}

export default Register;
