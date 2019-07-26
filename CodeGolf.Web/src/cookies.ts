"use strict";

const element = document.getElementById("cookie-warning");
const cookieName = "accepted-cookies";

const hasCookie = () => {
    return document.cookie.split(';').filter(item => item.indexOf(`${cookieName}=`) >= 0).length;
}

if (hasCookie() && element) {
    element.hidden = true;
} else {
    document.cookie = `${cookieName}=1`;
}