import Vue from 'vue';
import * as signalR from '@aspnet/signalr';

import App from './DemoVue.ts.vue';
import ResultType from './types/result';

const connection = new signalR.HubConnectionBuilder().withUrl("/refreshHub").build();

new Vue({
    el:"#demo",
    data: {
      data: [] as ReadonlyArray<ResultType>
    },
    render(h) {
        return h(App, { props: { appData: this.data } });
    },
    mounted(){
      const updateUI = () => {
          window.fetch('./api/Results/Results').then(r => {
              if (!r.ok) {
                  throw Error(r.statusText);
              }
              return r;
          }).then( (response) => {
              response.json().then((data: ReadonlyArray<ResultType>) => {
                this.data = [...data];
              });
          });
      };
      connection.on("newRound", updateUI);
      
      connection.start().then(updateUI).catch(err => {
          return console.error(err.toString());
      });
    }
});