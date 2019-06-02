grecaptcha.ready(function () {
    var key = document.getElementById('g-recaptcha-site-key').value;
    grecaptcha.execute(key, { action: 'demo' }).then(function (token) {
        document.getElementById('g-recaptcha-response').value = token;
    });
});