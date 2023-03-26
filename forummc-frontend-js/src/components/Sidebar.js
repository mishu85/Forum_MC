import React, { useState } from "react";
import "./Sidebar.css";
import MenuIcon from "@mui/icons-material/Menu";
import HomeIcon from "@mui/icons-material/Home";

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
    </div>
  );
}
