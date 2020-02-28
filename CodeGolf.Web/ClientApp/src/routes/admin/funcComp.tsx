import { FunctionalComponent, h } from "preact";

import CreateGame from "../../components/createGame";
import Loading from "../../components/loading";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { Game, GameId, Round } from "../../types/types";

const Row: FunctionalComponent<{ readonly g: Game, readonly resetGame: ((g: GameId) => void) }> = ({ g, resetGame }) =>
  (<article class="message">
    <div class="message-header">
      <p>Code: {g.accessKey}</p>
    </div>
    <div class="message-body">
      <div class="message-content">
        Rounds:
                <ul>
          {g.rounds.map(b => <li key={b.id}>{b.name}</li>)}
        </ul>
        <button class="button" onClick={() => resetGame(g.id)}>Reset Game</button>
      </div>
    </div>
  </article>);

interface Props {
  readonly myGames: LoadingState<ReadonlyArray<Game>>;
  readonly allChallenges: LoadingState<ReadonlyArray<Round>>;
  readonly showCreate: boolean;
  readonly toggleCreate: (state: boolean) => void;
  readonly resetGame: (g: GameId) => void;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ myGames, allChallenges, showCreate, toggleCreate, resetGame }) =>
  ifLoaded(myGames, g =>
    (<div><section class="accordions">
      {g.map((a: Game) => <Row g={a} resetGame={resetGame} key={a.id} />)}
    </section>
      {ifLoaded(allChallenges, c => (showCreate ? <CreateGame hide={() => toggleCreate(false)} challenges={c} /> : null), () => null)}
      <button className="button" onClick={() => toggleCreate(!showCreate)}>Create New</button>
    </div>),
    () =>
      <Loading />);

export default FuncComp;
