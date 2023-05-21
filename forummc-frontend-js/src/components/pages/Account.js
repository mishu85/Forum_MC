import Auth from "../../auth";
import Layout from "../Layout";

function Account() {
  const user = Auth.getInstance().getMyUser();

  return (
    <Layout>
      <h1>{user.userName}</h1>
    </Layout>
  );
}

export default Account;
