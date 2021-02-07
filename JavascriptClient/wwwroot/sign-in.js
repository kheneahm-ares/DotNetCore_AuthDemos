

var signIn = function () {
    var redirectUri = encodeURIComponent("https://localhost:44321/Home/SignIn");
    var responseType = encodeURIComponent("id_token token");
    var scope = encodeURIComponent("openid scope_one:read");
    var authUrl = "/connect/authorize/callback"+
                    "?client_id=client_id_js"+
                    "&redirect_uri="+ redirectUri +
                    "&response_type="+ responseType +
        "&scope="+ scope +
        "&nonce=lsakdjfklasjdfsadjlfj"+
        "&state=jaslkdjfkasdlfalsdjfasdfjasdjfklasjdlfajsldfjaldfjaljfd";
    var returnUrl = encodeURIComponent(authUrl);

    window.location.href = "https://localhost:44336/Auth/Login?ReturnUrl=" + returnUrl;

}