import { Routes, Route } from "react-router-dom";
import Home from "./components/pages/Home";
import Account from "./components/pages/Account";
import { GuardedRouteAuth } from "./GuardedRoute";
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
      <Route path="/" element={<Home />} />
    </Routes>
  );
}

export default App;
