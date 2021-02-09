
var config = {
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }),
    authority: "https://localhost:44336/",
    client_id: "client_id_js",
    response_type: "id_token token",
    redirect_uri: "https://localhost:44321/Home/SignIn",
    scope: "openid scope_one:read",
    post_logout_redirect_uri: "https://localhost:44321/Home/Index"
};


var userManager = new Oidc.UserManager(config);

var signIn = function () {
    userManager.signinRedirect();
}


var signOut = function () {
    userManager.signoutRedirect();
}

userManager.getUser().then(user => {
    console.log("user:", user);
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    }
});

var callApi = function () {
    axios.get("https://localhost:44339/secret")
        .then(res => {
            console.log(res);
        });
};

axios.interceptors.response.use(succ => { return succ }, err => {
    console.log(err.response);

    return Promise.reject(error);
})