import Vue from 'vue';
import * as signalR from '@aspnet/signalr';
import App from './DashboardVue.ts.vue';
var connection = new signalR.HubConnectionBuilder().withUrl("/refreshHub").build();
new Vue({
    el: "#dashboard",
    data: {
        data: []
    },
    render: function (h) {
        return h(App, { props: { appData: this.data } });
    },
    mounted: function () {
        var _this = this;
        var updateUI = function () {
            window.fetch('./api/Results/Results').then(function (r) {
                if (!r.ok) {
                    throw Error(r.statusText);
                }
                return r;
            }).then(function (response) {
                response.json().then(function (data) {
                    _this.data = data.slice();
                });
            });
        };
        connection.on("newAnswer", updateUI);
        connection.start().then(updateUI).catch(function (err) {
            return console.error(err.toString());
        });
    }
});
