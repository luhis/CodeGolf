import { Editor, EditorConfiguration } from "codemirror";
import { FunctionalComponent, h } from "preact";
import { Controlled as CodeMirror, IControlledCodeMirror } from "react-codemirror2";
import { Circular } from "styled-loaders";

import { AttemptWithCode, ifLoaded, LoadingState } from "../../types/types";

import "codemirror/lib/codemirror.css";
import "codemirror/mode/clike/clike";

const CM = CodeMirror as unknown as FunctionalComponent<IControlledCodeMirror>;
interface Props {
    readonly result: LoadingState<AttemptWithCode>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ result }) => ifLoaded(result, data => (
    <section class="section is-fluid">
        <h1 class="title">Attempt</h1>
        <div class="columns">
            <figure class="image is-48x48">
                <img src={data.avatar} />
            </figure>
            <span>
                {data.loginName} {data.score} {data.timeStamp}
            </span>
        </div>
        <CM value={data.code}
            className="editor"
            onBeforeChange={() => undefined}
            options={{ lineNumbers: true, mode: "text/x-csharp" } as EditorConfiguration}
            editorDidMount={(e: Editor) => {
                setTimeout(() => {
                    e.refresh();
                }, 250);
            }} />
    </section>), () => <Circular />);

export default FuncComp;
