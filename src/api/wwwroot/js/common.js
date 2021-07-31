function getUser(isIndex = false) {
    var user = window.localStorage.getItem("user");
    if (!user && !isIndex) {
        window.location.href = "./index.html";
        return;
    }

    return JSON.parse(user);
}