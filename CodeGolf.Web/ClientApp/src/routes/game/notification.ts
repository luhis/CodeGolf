import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
import { toast } from "bulma-toast";
import { VNode } from "preact";
import render from "preact-render-to-string";

import NotificationTemplate from "./notificationTemplate";

const setup = (onUpdate: (() => void)) => {
    const connection = new HubConnectionBuilder().withUrl("/refreshHub").configureLogging(LogLevel.Error).build();
    connection.on("newTopScore", (name: string, score: number, avatarUri: string) => {
        toast({
            message: render(NotificationTemplate({name, score, avatarUri}) as VNode),
            type: "is-info",
            dismissible: true,
            pauseOnHover: true,
            duration: 5_000,
        });
    });
    connection.on("newRound", onUpdate);
    connection.start().catch(err => console.error(err.toString()));
    return connection;
};

export default setup;
