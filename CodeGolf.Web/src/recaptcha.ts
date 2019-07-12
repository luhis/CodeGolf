"use strict";

window.grecaptcha.ready(() => {
    const key = (document.getElementById('g-recaptcha-site-key') as HTMLInputElement).value;
    window.grecaptcha.execute(key, { action: 'demo' }).then((token: string) => {
        (document.getElementById('g-recaptcha-response') as HTMLInputElement).value = token;
    });
});