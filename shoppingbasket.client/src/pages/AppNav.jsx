import { Link } from "react-router";
import "../styles/AppNav.css";

function AppNav({ homeRef, historyRef }) {
  return (
    <>
      <nav>
        <ul id="horizontal-navbar">
          <li className="navbar-ref">
            <Link to={homeRef} id="home-link">
              Home
            </Link>
          </li>
          <li className="navbar-ref">
            <Link to={historyRef} id="history-link">
              History
            </Link>
          </li>
        </ul>
      </nav>
    </>
  );
}

export default AppNav;
