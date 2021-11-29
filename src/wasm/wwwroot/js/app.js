function getHeightToBottom() {
    return document.documentElement.scrollHeight - window.innerHeight - document.documentElement.scrollTop;
}

function showToast(id) {
    new bootstrap.Toast(document.getElementById(id)).show();
}

function hideToast(id) {
    new bootstrap.Toast(document.getElementById(id)).hide();
}