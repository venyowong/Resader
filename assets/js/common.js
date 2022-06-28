function getUser(isLoginPage = false) {
    var user = window.localStorage.getItem("user");
    if (!user) {
        if (!isLoginPage) {
            window.location.href = "./login.html";
        }
        return;
    }

    return JSON.parse(user);
}

function setUser(user) {
    window.localStorage.setItem("user", JSON.stringify(user));
}

const base_url = "..";