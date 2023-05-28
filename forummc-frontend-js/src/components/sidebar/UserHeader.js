import React from "react";
import Auth from "../../auth";
import Avatar from "@mui/material/Avatar";
import "./UserHeader.css";
import { useNavigate } from "react-router-dom";

// export default function UserHeader(props) {
//   return props.isExpanded ? (
//     <div className="personal">
//       <Avatar
//         alt={props.username}
//         src=""
//         sx={{ bgcolor: "#883", width: 24, height: 24 }}
//       >
//         {props.username[0]}
//       </Avatar>
//       <p>{props.username}</p>
//     </div>
//   ) : null;
// }
export default function UserHeader(props) {
  const navigate = useNavigate();

  return (
    <li>
      <a href="#" onClick={() => navigate("/profile")}>
        <Avatar
          alt={props.username}
          src=""
          sx={{ bgcolor: "#883", width: 24, height: 24 }}
        >
          {props.username[0]}
        </Avatar>
        <span className={`${props.isExpanded ? "expanded" : ""}`}>
          {props.username}
        </span>
      </a>
    </li>
  );
}

{
  /* <li>
      <a href="/">
        <Avatar
        alt={props.username}
        src=""
        sx={{ bgcolor: "#883", width: 24, height: 24 }}
      >
        {props.username[0]}
      </Avatar>
        <span className={`${props.isExpanded ? "expanded" : ""}`}>
          {props.username}
        </span>
      </a>
    </li> */
}
