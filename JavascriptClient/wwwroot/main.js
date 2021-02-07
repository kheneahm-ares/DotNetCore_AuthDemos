
var config = {
    authority: "https://localhost:44336/",
    client_id: "client_id_js",
    response_type: "id_token token",
    redirect_uri: "https://localhost:44321/Home/SignIn",
    scope: "openid scope_one:read"
};


var userManager = new Oidc.userManager(config);

var signIn = function () {
    userManager.signInRedirect();
}