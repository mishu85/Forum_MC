import { Router, Routes, Route } from "react-router-dom";
import { createBrowserHistory } from "history";
import Home from "./components/pages/Home";

const history = createBrowserHistory();

function App() {
  return (
    <Router location={history.location} navigator={history}>
      <Routes>
        <Route path="/" element={<Home />} />
      </Routes>
    </Router>
  );
}

export default App;
