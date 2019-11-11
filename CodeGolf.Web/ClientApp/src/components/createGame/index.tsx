import { Component, h, RenderableProps } from "preact";

import { addGame } from "../../api/adminApi";
import { GameId, Round } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
    readonly accessKey: string;
}

interface Props {
    readonly hide: () => void;
    readonly challenges: ReadonlyArray<Round>;
}

export default class Comp extends Component<Props, State> {

    constructor() {
        super();
        this.state = { accessKey: "" };
    }

    public readonly render = (props: RenderableProps<Props>, state: Readonly<State>) =>
        <FuncComp {...props} {...state} save={this.addGame} />

    private readonly addGame = () => addGame({ id: "" as GameId, accessKey: this.state.accessKey, rounds: [] });

}
