import { Router, Routes, Route } from "react-router-dom";
import { createBrowserHistory } from "history";
import Home from "./components/pages/Home";
import Account from "./components/pages/Account";

const history = createBrowserHistory();

function App() {
  return (
    <Router location={history.location} navigator={history}>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/profile" element={<Account />} />
      </Routes>
    </Router>
  );
}

export default App;
