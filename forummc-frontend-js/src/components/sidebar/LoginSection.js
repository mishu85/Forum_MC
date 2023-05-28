import React, { useState } from "react";
import "./Sidebar.css";
import { Paper, TextField, Button } from "@mui/material";
import { login } from "../../api/accountsApi";
import Auth from "../../auth";
import { createTheme } from "@mui/material/styles";
import { ThemeProvider } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function LoginSection(props) {
  const navigate = useNavigate();
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
      navigate("/");
    }
  };

  const theme = createTheme({
    palette: {
      primary: {
        main: "#6699aa",
      },
      secondary: {
        main: "#ff0000",
      },
    },
  });

  return (
    <ThemeProvider theme={theme}>
      <section id="login" className={`${props.isExpanded ? "expanded" : ""}`}>
        <Paper elevation={10}>
          <form onSubmit={handleSubmit}>
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
              SIGN IN
            </Button>
          </form>
        </Paper>
      </section>
    </ThemeProvider>
  );
}
