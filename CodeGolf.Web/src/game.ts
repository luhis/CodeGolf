import * as signalR from '@aspnet/signalr';
import * as bulmaToast from 'bulma-toast';

const connection = new signalR.HubConnectionBuilder().withUrl("/refreshHub").build();

connection.on("newRound", () => {
    window.location.href = window.location.href;
});

const template = (name: string, score: number, avatarUri: string) =>
    `<div>
        <p>New Top Score!</p>
        <figure class="image container is-48x48"><img src="${avatarUri}"><img/></figure>
        <p>${name}, ${score} strokes</p>
    </div>`;

connection.on("newTopScore", (name, score, avatarUri) => {
    bulmaToast.toast({
        message: template(name, score, avatarUri),
        type: "is-info",
        dismissible: true,
        pauseOnHover: true,
        duration: 2000,
    });
});

connection.start().catch((err) => {
    return console.error(err.toString());
});