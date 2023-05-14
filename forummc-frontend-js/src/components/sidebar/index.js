import React, { useState } from "react";
import "./Sidebar.css";
import MenuIcon from "@mui/icons-material/Menu";
import HomeIcon from "@mui/icons-material/Home";
import LoginSection from "./LoginSection";
import SidebarItem from "./SidebarItem";
import Auth from "../../auth";
import UserHeader from "./UserHeader";
import DeleteIcon from "@mui/icons-material/Delete";

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
      <section id="sidebar-main">
        <ul>
          <SidebarItem
            title="Home"
            icon={<MenuIcon />}
            onClick={() => setIsExpanded(!isExpanded)}
          />
          <SidebarItem
            title="Home"
            icon={<HomeIcon />}
            href="/"
            isExpanded={isExpanded}
          />
        </ul>
        {auth.isAuthenticated() ? (
          <UserHeader
            username={auth.getMyUser().userName}
            isExpanded={isExpanded}
          />
        ) : (
          <LoginSection isExpanded={isExpanded} />
        )}
      </section>
      <section id="sidebar-bottom">
        <ul>
          {auth.isAuthenticated() ? (
            <SidebarItem
              title="Log Out"
              icon={<DeleteIcon />}
              onClick={() => auth.logout()}
              isExpanded={isExpanded}
            />
          ) : null}
        </ul>
      </section>
    </div>
  );
}
