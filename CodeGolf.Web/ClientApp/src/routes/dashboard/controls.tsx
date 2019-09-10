import { FunctionalComponent, h } from "preact";

import { Hole, LoadingState } from "../../types/types";

interface Props { readonly hole: LoadingState<Hole | undefined>; readonly endHole: () => Promise<void>; readonly nextHole: () => Promise<void>; }

const GetButton: FunctionalComponent<Props> = ({ hole, endHole, nextHole }) => {
    if (hole.type === "Loaded" && hole.data) {
        if (hole.data.closedAt) {
            if (hole.data.hasNext) {
                return <button onClick={nextHole} class="button">Next Hole</button>;
            }

            return <button onClick={nextHole} class="button">End Game</button>;
        }

        return <button onClick={endHole} class="button">End Hole</button>;
    }

    return <button onClick={nextHole} class="button">Start Game</button>;
};

const comp: FunctionalComponent<Readonly<Props>> = ({ hole, endHole, nextHole }) => (
    <div class="field">
        <div class="control">
            <GetButton hole={hole} endHole={endHole} nextHole={nextHole} />
        </div>
    </div>);

export default comp;
