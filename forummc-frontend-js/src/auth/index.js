export default class Auth {
  // singleton design pattern

  static instance = null; //???

  constructor() {
    if (Auth.instance) {
      return Auth.instance;
    }
    this.init();
    Auth.instance = this;
  }

  static getInstance = () => {
    if (Auth.instance) {
      return Auth.instance;
    }
    Auth.instance = new Auth();
    return Auth.instance;
  };

  init = () => {
    let data = localStorage.getItem("myUser");
    if (data) {
      let myUser = JSON.parse(data);
      this.myUser = myUser;
      this.authenticated = true;
      return;
    }
    this.authenticated = false;
    this.myUser = null;
  };

  updateMyUser = (updatedUser) => {
    this.myUser = updatedUser;
    localStorage.setItem("myUser", JSON.stringify(this.myUser));
  };

  getMyUser = () => {
    return this.myUser;
  };

  isAuthenticated = () => {
    return this.authenticated;
  };

  login = (data) => {
    localStorage.setItem("myUser", JSON.stringify(data));
    this.authenticated = true;
    this.myUser = data;
  };

  logout = () => {
    localStorage.removeItem("myUser");
    this.authenticated = false;
    this.myUser = null;
  };
}
