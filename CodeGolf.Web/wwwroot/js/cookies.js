var element = document.getElementById("cookie-warning");
var cookieName = "accepted-cookies";

function hasCookie() {
    return document.cookie.split(';').filter(function(item) {
        return item.indexOf(cookieName + '=') >= 0;
    }).length;
}

if (hasCookie()) {
    element.hidden = true;
} else {
    document.cookie = cookieName + "=1";
}