import React, { useState } from "react";
import "./Sidebar.css";
import { Paper, TextField, Button } from "@mui/material";
import { login } from "../../api/accountsApi";
import Auth from "../../auth";

export default function LoginSection(props) {
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (event) => {
    event.preventDefault();
    const response = await login(userName, password);
    if (response) {
      let myUser = {
        userName: userName,
        token: response,
      };
      Auth.getInstance().login(myUser);
      console.log("logged in!");
      console.log(response);
      window.open("/", "_self");
    }
  };

  return (
    <section id="login" className={`${props.isExpanded ? "expanded" : ""}`}>
      <Paper elevation={10}>
        <form onSubmit={handleSubmit}>
          <TextField
            label="UserName"
            placeholder="Type UserName"
            required
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
            SIGN IN
          </Button>
        </form>
      </Paper>
    </section>
  );
}
