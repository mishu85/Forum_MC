import React, { useState } from "react";
import "./Sidebar.css";
import MenuIcon from "@mui/icons-material/Menu";
import HomeIcon from "@mui/icons-material/Home";
import FingerprintIcon from "@mui/icons-material/Home";
import {
  Grid,
  Paper,
  Avatar,
  TextField,
  Checkbox,
  Typography,
  Link,
  FormControlLabel,
  Button,
} from "@mui/material";
import { login } from "../api/accountsApi";
import Auth from "../auth";

function SidebarItem(props) {
  return (
    <li>
      <a href="/">
        {props.icon}
        <span className={`${props.isExpanded ? "expanded" : ""}`}>
          {props.title}
        </span>
      </a>
    </li>
  );
}

function LoginSection(props) {
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

export default function Sidebar() {
  const [isExpanded, setIsExpanded] = useState(false);

  function handleMouseEnter() {
    setIsExpanded(true);
  }

  function handleMouseLeave() {
    setIsExpanded(false);
  }

  return (
    <div
      className={`sidebar ${isExpanded ? "expanded" : ""}`}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      <ul>
        <li>
          <a href="#" onClick={() => setIsExpanded(!isExpanded)}>
            <MenuIcon />
          </a>
        </li>
        <SidebarItem title="Home" icon={<HomeIcon />} isExpanded={isExpanded} />
      </ul>
      <LoginSection isExpanded={isExpanded} />
    </div>
  );
}
