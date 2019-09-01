import { Component, h, RenderableProps } from "preact";

import { getMyGames, resetGame } from "../../api/adminApi";
import { Game, LoadingState, Round } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
    readonly myGames: LoadingState<ReadonlyArray<Game>>;
    readonly allChallenges: LoadingState<ReadonlyArray<Round>>;
    readonly showCreate: boolean;
}

export default class Comp extends Component<{}, State> {

    constructor() {
        super();
        this.state = { myGames: { type: "Loading" }, allChallenges: { type: "Loading" }, showCreate: false };
    }

    public readonly componentDidMount = async () => {
        const games = await getMyGames();
        // const challenges = await getAllChallenges();
        this.setState(s => ({ ...s, myGames: { type: "Loaded", data: games } }));
    }

    public readonly render = (_: RenderableProps<{}>, state: Readonly<State>) =>
        <FuncComp {...state} toggleCreate={(show: boolean) => this.setState(s => ({ ...s, showCreate: show }))} resetGame={resetGame} />

}
