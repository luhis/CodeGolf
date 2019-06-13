import Vue from 'vue';
import App from './Dashboard.vue';

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
          window.fetch('./api/Results/Results').then( (response) => {
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