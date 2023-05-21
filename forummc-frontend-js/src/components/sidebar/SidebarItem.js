import React from "react";
import "./Sidebar.css";

export default function SidebarItem(props) {
  const onClick = props.onClick ?? (() => {});

  return (
    <li>
      <a href={props.href ?? "#"} onClick={() => onClick()}>
        {props.icon}
        <span className={`${props.isExpanded ? "expanded" : ""}`}>
          {props.title}
        </span>
      </a>
    </li>
  );
}
