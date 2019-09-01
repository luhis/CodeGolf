import { FunctionalComponent, h } from "preact";
import { Circular } from "styled-loaders";

import { Game, Guid, ifLoaded, LoadingState, Round } from "../../types/types";

const Modal: FunctionalComponent<{ readonly hide: () => void, readonly challenges: ReadonlyArray<Round> }> = ({ hide }) =>
    (<div class="modal is-active">
        <div class="modal-background" />
        <div class="modal-content">
            <header class="modal-card-head">
                <p class="modal-card-title">Create New Game</p>
                <button class="delete" aria-label="close" onClick={hide} />
            </header>
            <section class="modal-card-body">
                <div class="field">
                    <label class="label">Access Code</label>
                    <div class="control">
                        <input class="input is-primary" type="text" />
                    </div>
                </div>
            </section>
            <footer class="modal-card-foot">
                <button class="button is-success">Save changes</button>
                <button class="button" onClick={hide}>Cancel</button>
            </footer>
        </div>
    </div>);

const Row: FunctionalComponent<{ readonly g: Game, readonly resetGame: ((g: Guid) => void) }> = ({ g, resetGame }) =>
  (<article class="accordion is-active">
        <div class="accordion-header toggle">
            <p>Code: {g.accessKey}</p>
        </div>
        <div class="accordion-body">
            <div class="accordion-content">
                Rounds:
                <ul>
                    {g.rounds.map(b => <li key={b.id}>{b.name}</li>)}
                </ul>
                <button class="button" onClick={() => resetGame(g.id)}>Reset Game</button>
            </div>
        </div>
    </article>);

type Props = ({
    readonly myGames: LoadingState<ReadonlyArray<Game>>;
    readonly allChallenges: LoadingState<ReadonlyArray<Round>>;
    readonly showCreate: boolean
    readonly toggleCreate: (state: boolean) => void;
    readonly resetGame: ((g: Guid) => void);
});

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ myGames, allChallenges, showCreate, toggleCreate, resetGame }) =>
    ifLoaded(myGames, g =>
        (<div><section class="accordions">
            {g.map((a: Game) => <Row g={a} resetGame={resetGame} key={a.id} />)}
        </section>
            {ifLoaded(allChallenges, c => (showCreate ? <Modal hide={() => toggleCreate(false)} challenges={c} /> : null), () => null)}
            <button className="button" onClick={() => toggleCreate(!showCreate)}>Create New</button>
        </div>),
        () =>
            <Circular />);

export default FuncComp;
