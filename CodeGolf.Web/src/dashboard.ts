import Vue from 'vue';
import App from './DashboardVue.ts.vue';
import * as signalR from '@aspnet/signalr';

const connection = new signalR.HubConnectionBuilder().withUrl("/refreshHub").build();

interface Result {Rank: number, Id: string, LoginName: string , Avatar: string, Score: number, TimeStamp: string}

new Vue({
    el:"#dashboard",
    data: {
      data: [] as ReadonlyArray<Result>
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
              response.json().then((data: ReadonlyArray<Result>) => {
                this.data = [...data];
              });
          });
      };
      connection.on("newAnswer", updateUI);
      
      connection.start().then(updateUI).catch(err => {
          return console.error(err.toString());
      });
    }
});