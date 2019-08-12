import { FunctionalComponent, h } from "preact";
import { Circular } from "styled-loaders";

import Attempts from "../../components/attempts";
import ChallengeComp from "../../components/challenge";
import { Attempt, Hole, LoadingState } from "../../types/types";
import Controls from "./controls";
import Times from "./times";

interface Props {
    readonly current: LoadingState<Hole | undefined>;
    readonly attempts: LoadingState<ReadonlyArray<Attempt>>;
    readonly nextHole: () => Promise<void>;
    readonly endHole: () => Promise<void>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ current, attempts, nextHole, endHole }) =>
    (<section class="section">
        <span class="title">Attempts</span>
        <div class="columns">
            <div class="column">
                {attempts.type === "Loaded" ? <Attempts attempts={attempts.data} /> : <Circular/>}
            </div>
            <div class="column">
                <Controls hole={current} nextHole={nextHole} endHole={endHole} />
                {current.type === "Loaded" && current.data ?
                    <div>
                        <Times start={current.data.start} end={current.data.end} />
                        <ChallengeComp challenge={current.data.challengeSet} onCodeClick={undefined} />
                    </div> : <Circular/>}
            </div>
        </div>
    </section>);

export default FuncComp;
