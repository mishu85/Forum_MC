import React from "react";
import "./Sidebar.css";

export default function SidebarItem(props) {
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
