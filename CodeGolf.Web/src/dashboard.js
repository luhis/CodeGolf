import Vue from 'vue';
import App from './Dashboard.vue';
import * as signalR from '@aspnet/signalr';

const connection = new signalR.HubConnectionBuilder().withUrl("/refreshHub").build();

new Vue({
    el:"#dashboard",
    data: {
      data: []
    },
    render(h) {
        return h(App, { props: { appData: this.data } });
    },
    mounted(){
      const  updateUI = () => {
          window.fetch('./api/Results/Results').then(r => {
              if (!r.ok) {
                  throw Error(r.statusText);
              }
              return r;
          }).then( (response) => {
              response.json().then((data) => {
                this.data = data;
              });
          });
      };
      connection.on("newAnswer", updateUI);
      
      connection.start().then(updateUI).catch(function (err) {
          return console.error(err.toString());
      });
    }
});