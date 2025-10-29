import { BrowserRouter, Routes, Route } from "react-router";
import "./styles/App.css";
import AppNav from "./pages/AppNav";
import Home from "./pages/Home";
import History from "./pages/History";

function App() {
  return (
    <BrowserRouter>
      <AppNav homeRef="/" historyRef="/history" />
      <Routes>
        <Route index element={<Home />} />
        <Route path="history" element={<History />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
