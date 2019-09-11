import { Component, h, RenderableProps } from "preact";

import { AddGame } from "../../api/adminApi";
import { Round } from "../../types/types";
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
        <FuncComp {...props} {...state} save={() => AddGame({ id: "", accessKey: state.accessKey, rounds: [] })} />

}
