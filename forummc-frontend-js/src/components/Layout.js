import Sidebar from "./sidebar";
import "./Layout.css";

export default function Layout(props) {
  return (
    <div id="layout">
      <Sidebar />
      <main>{props.children}</main>
    </div>
  );
}
