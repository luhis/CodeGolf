"use strict";
window.grecaptcha.ready(function () {
    var key = document.getElementById('g-recaptcha-site-key').value;
    window.grecaptcha.execute(key, { action: 'demo' }).then(function (token) {
        document.getElementById('g-recaptcha-response').value = token;
    });
});
