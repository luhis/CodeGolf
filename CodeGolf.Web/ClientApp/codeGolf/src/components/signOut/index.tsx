import { Component, h, RenderableProps } from "preact";

import { signOut } from "../../api/accessApi";
import FuncComp from "./funcComp";

interface State {
    readonly showModal: boolean;
}

const signOutAndRedirect = async () => {
    await signOut();
    window.location.replace("/");
};

export default class Comp extends Component<{}, State> {

    constructor() {
        super();
        this.state = { showModal: false };
    }

    public readonly render = (_: RenderableProps<{}>, state: Readonly<State>) =>
        <FuncComp {...state} signOutFunc={signOutAndRedirect} toggleModal={this.toggleModal} />

    private readonly toggleModal = () => this.setState(s => ({ ...s, showModal: !s.showModal }));

}
