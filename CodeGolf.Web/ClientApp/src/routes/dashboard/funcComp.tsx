import { FunctionalComponent, h } from "preact";

import Attempts from "../../components/attempts";
import ChallengeComp from "../../components/challenge";
import GamePicker from "../../components/gamePicker";
import Loading from "../../components/loading";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { Attempt, Hole, GameId } from "../../types/types";
import Controls from "./controls";
import Times from "./times";

interface Props {
  readonly gameId: GameId | undefined;
  readonly current: LoadingState<Hole | undefined>;
  readonly attempts: LoadingState<ReadonlyArray<Attempt>>;
  readonly nextHole: (_: GameId) => Promise<void>;
  readonly endHole: (_: GameId) => Promise<void>;
  readonly setGameCode: (g: string) => Promise<void>;
}

const RightCol: FunctionalComponent<{ readonly hole?: Hole }> = ({ hole }) => (hole ? <div>
  <Times start={hole.start} end={hole.end} />
  <ChallengeComp challengeSet={hole.challengeSet} onCodeClick={undefined} errors={undefined} />
</div> : null);


const FuncComp: FunctionalComponent<Readonly<Props>> = ({ current, attempts, nextHole, endHole, setGameCode, gameId }) =>
  (<section class="section">
    <span class="title">Attempts</span>
    <div class="columns">
      <div class="column">
        {ifLoaded(attempts, a => <Attempts attempts={a} />, () => <Loading />)}
      </div>
      <div class="column">
        <Controls hole={current} nextHole={() => gameId ? nextHole(gameId) : Promise.resolve()} endHole={() => gameId ? endHole(gameId) : Promise.resolve()} />
        {ifLoaded(current, c => <RightCol hole={c} />, () => <Loading />)}
      </div>
    </div>
    {gameId ? <GamePicker verifyCode={setGameCode} /> : null}
  </section>);

export default FuncComp;
