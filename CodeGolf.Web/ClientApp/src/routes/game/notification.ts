import { HubConnectionBuilder } from "@aspnet/signalr";
import { toast } from "bulma-toast";

const template = (name: string, score: number, avatarUri: string) =>
    `<div>
        <p>New Top Score!</p>
        <figure class="image container is-48x48"><img src="${avatarUri}"><img/></figure>
        <p>${name}, ${score} strokes</p>
    </div>`;

const setup = (onUpdate: (() => void)) => {
    const connection = new HubConnectionBuilder().withUrl("/refreshHub").build();
    connection.on("newTopScore", (name: string, score: number, avatarUri: string) => {
        toast({
            message: template(name, score, avatarUri),
            type: "is-info",
            dismissible: true,
            pauseOnHover: true,
            duration: 5000,
        });
    });
    connection.on("newRound", onUpdate);
    connection.start().catch(err => console.error(err.toString()));
    return connection;
};

export default setup;
