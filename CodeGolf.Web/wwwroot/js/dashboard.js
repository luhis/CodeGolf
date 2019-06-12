"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/refreshHub").build();

var app = new Vue({
    el: '#app',
    data: function() {
        return { results: [] };
    },
});

function updateUI() {

    fetch('./api/Results/Results').then(function (response) {
        response.json().then(function(data) {
            app.results = data;
        });
    });
}

connection.on("newAnswer", updateUI);

connection.start().then(updateUI).catch(function (err) {
    return console.error(err.toString());
});