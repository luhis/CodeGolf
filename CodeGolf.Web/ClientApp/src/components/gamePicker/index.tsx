import { FunctionalComponent, h } from "preact";
import { useState } from "preact/hooks";

interface Props {
    readonly verifyCode: (passcode: string) => Promise<void>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ verifyCode }) => {
    const [state, setState] = useState("");
    return (<div class="modal is-active">
        <div class="modal-background" />
        <div class="modal-content">
            <header class="modal-card-head">
                <p class="modal-card-title">Join Game</p>
            </header>
            <section class="modal-card-body">
                What is your game passcode?
                <input class="input" type="text" value={state} onChange={a => a.target ? setState((a.target as HTMLInputElement).value) : undefined} />
            </section>
            <footer class="modal-card-foot">
                <button class="button is-success" onClick={() => verifyCode(state)}>Join</button>
            </footer>
        </div>
    </div>);
};

export default FuncComp;
