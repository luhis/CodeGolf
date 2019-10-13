import { FunctionalComponent, h } from "preact";

import Attempts from "../../components/attempts";
import ChallengeComp from "../../components/challenge";
import Loading from "../../components/loading";
import { Attempt, Hole, ifLoaded, LoadingState } from "../../types/types";
import Controls from "./controls";
import Times from "./times";

interface Props {
    readonly current: LoadingState<Hole | undefined>;
    readonly attempts: LoadingState<ReadonlyArray<Attempt>>;
    readonly nextHole: () => Promise<void>;
    readonly endHole: () => Promise<void>;
}

const LeftCol: FunctionalComponent<{ readonly hole?: Hole }> = ({ hole }) => (hole ? <div>
    <Times start={hole.start} end={hole.end} />
    <ChallengeComp challenge={hole.challengeSet} onCodeClick={undefined} />
</div> : null);

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ current, attempts, nextHole, endHole }) =>
    (<section class="section">
        <span class="title">Attempts</span>
        <div class="columns">
            <div class="column">
                {ifLoaded(attempts, a=> <Attempts attempts={a} />, () => <Loading/>)}
            </div>
            <div class="column">
                <Controls hole={current} nextHole={nextHole} endHole={endHole} />
                {ifLoaded(current, c => <LeftCol hole={c} />, () => <Loading/>)}
            </div>
        </div>
    </section>);

export default FuncComp;
