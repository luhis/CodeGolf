import { Editor, EditorConfiguration } from "codemirror";
import { FunctionalComponent, h } from "preact";
import { Controlled as CodeMirror, IControlledCodeMirror } from "react-codemirror2";
import { Circular } from "styled-loaders";

import { LoadingState } from "../../types/types";

import "codemirror/lib/codemirror.css";
import "codemirror/mode/clike/clike";

const CM = CodeMirror as unknown as FunctionalComponent<IControlledCodeMirror>;
interface Props {
    readonly result: LoadingState<string>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ result }) => result.type === "Loaded" ? (
    <section class="section is-fluid">
        <CM value={result.data}
            className="editor"
            onBeforeChange={() => undefined}
            options={{ lineNumbers: true, mode: "text/x-csharp" } as EditorConfiguration}
            editorDidMount={(e: Editor) => {
                setTimeout(() => {
                    e.refresh();
                }, 250);
            }} />
    </section>) : <Circular />;

export default FuncComp;
