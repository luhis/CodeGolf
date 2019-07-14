import * as signalR from '@aspnet/signalr';
import * as bulmaToast from 'bulma-toast';
var connection = new signalR.HubConnectionBuilder().withUrl("/refreshHub").build();
connection.on("newRound", function () {
    window.location.href = window.location.href;
});
var template = function (name, score, avatarUri) {
    return "<div>\n        <p>New Top Score!</p>\n        <figure class=\"image container is-48x48\"><img src=\"" + avatarUri + "\"><img/></figure>\n        <p>" + name + ", " + score + " strokes</p>\n    </div>";
};
connection.on("newTopScore", function (name, score, avatarUri) {
    bulmaToast.toast({
        message: template(name, score, avatarUri),
        type: "is-info",
        dismissible: true,
        pauseOnHover: true,
        duration: 4000,
    });
});
connection.start().catch(function (err) {
    return console.error(err.toString());
});
