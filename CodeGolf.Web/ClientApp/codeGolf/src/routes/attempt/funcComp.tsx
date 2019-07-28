import { Editor, EditorConfiguration } from "codemirror";
import { FunctionalComponent, h } from "preact";
import { Controlled as CodeMirror, IControlledCodeMirror } from "react-codemirror2";
import { Circular } from "styled-loaders";

import { AttemptWithCode, LoadingState } from "../../types/types";

import "codemirror/lib/codemirror.css";
import "codemirror/mode/clike/clike";

const CM = CodeMirror as any as FunctionalComponent<IControlledCodeMirror>;
interface Props {
    result: LoadingState<AttemptWithCode>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ result }) => result.type === "Loaded" ? (
    <section class="section is-fluid">
        <h1 class="title">Attempt</h1>
        <div class="columns">
            <figure class="image is-48x48">
                <img src={result.data.avatar} />
            </figure>
            <span>
                {result.data.loginName} {result.data.score} {result.data.timeStamp}
            </span>
        </div>
        <CM value={result.data.code}
            // className={style.editor}
            onBeforeChange={() => undefined}
            options={{ lineNumbers: true, mode: "text/x-csharp" } as EditorConfiguration}
            editorDidMount={(e: Editor) => {
                setTimeout(() => {
                    e.refresh();
                }, 250);
            }} />
    </section>) : <Circular/>;

export default FuncComp;
