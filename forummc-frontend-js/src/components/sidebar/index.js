import React, { useState } from "react";
import "./Sidebar.css";
import MenuIcon from "@mui/icons-material/Menu";
import HomeIcon from "@mui/icons-material/Home";
import LoginSection from "./LoginSection";
import SidebarItem from "./SidebarItem";
import Auth from "../../auth";
import UserHeader from "./UserHeader";

export default function Sidebar() {
  const [isExpanded, setIsExpanded] = useState(false);

  function handleMouseEnter() {
    setIsExpanded(true);
  }

  function handleMouseLeave() {
    setIsExpanded(false);
  }

  var auth = Auth.getInstance();

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
      {auth.isAuthenticated() ? (
        <UserHeader
          username={auth.getMyUser().userName}
          isExpanded={isExpanded}
        />
      ) : (
        <LoginSection isExpanded={isExpanded} />
      )}
    </div>
  );
}
