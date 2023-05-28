import React from "react";
import { Navigate } from "react-router-dom";

const GuardedRouteAuth = ({ component: Component, auth }) =>
  auth === true ? <Component /> : <Navigate to="/" />;

const GuardedRouteAuthReversed = ({ component: Component, auth }) =>
  auth === false ? <Component /> : <Navigate to="/" />;

export { GuardedRouteAuth, GuardedRouteAuthReversed };
