import { Editor, EditorConfiguration } from "codemirror";
import { AnyComponent, FunctionalComponent, h } from "preact";
import { Controlled as CodeMirror, IControlledCodeMirror } from "react-codemirror2";
import { Circular } from "styled-loaders";

import { ifLoaded, LoadingState } from "../../types/types";

import "codemirror/lib/codemirror.css";
import "codemirror/mode/clike/clike";

const CM = CodeMirror as unknown as AnyComponent<IControlledCodeMirror, any>;
interface Props {
    readonly result: LoadingState<string>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ result }) => ifLoaded(result, data => (
    <section class="section is-fluid">
        <CM value={data}
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
