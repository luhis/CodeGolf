"use strict";
var element = document.getElementById("cookie-warning");
var cookieName = "accepted-cookies";
var hasCookie = function () {
    return document.cookie.split(';').filter(function (item) {
        return item.indexOf(cookieName + "=") >= 0;
    }).length;
};
if (hasCookie() && element) {
    element.hidden = true;
}
else {
    document.cookie = cookieName + "=1";
}
