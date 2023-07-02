import { Routes, Route } from "react-router-dom";
import Home from "./components/pages/Home";
import Register from "./components/pages/Register";
import Account from "./components/pages/Account";
import { GuardedRouteAuth, GuardedRouteAuthReversed } from "./GuardedRoute";
import Auth from "./auth";

function App() {
  return (
    <Routes>
      <Route
        path="/profile"
        element={
          <GuardedRouteAuth
            auth={Auth.getInstance().isAuthenticated()}
            component={Account}
          />
        }
      />
      <Route
        path="/register"
        element={
          <GuardedRouteAuthReversed
            auth={Auth.getInstance().isAuthenticated()}
            component={Register}
          />
        }
      />
      <Route path="/" element={<Home />} />
    </Routes>
  );
}

export default App;
