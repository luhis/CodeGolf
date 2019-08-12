import { Editor, TextMarker } from "codemirror";
import { FunctionalComponent, h } from "preact";
import { Controlled as CodeMirror, IControlledCodeMirror } from "react-codemirror2";

import { RunResult } from "../../types/types";
import * as style from "./style.css";

import "codemirror/lib/codemirror.css";
import "codemirror/mode/clike/clike";

interface Props {
    readonly code: string;
    readonly codeChanged: ((s: string) => void);
    readonly submitCode: ((s: string) => void);
    readonly errors?: RunResult;
}

const openInAction = (actionName: string, code: string) =>
    window.open(`./api/code/${actionName}?Code=${code}`, "_blank");

const getScore = (code: string) => code
    .replace(/\s/g, "")
    .length.toString();

let editor: (Editor | undefined);

const setErrors = (errors?: RunResult) => {
    if (editor) {
        const doc = editor.getDoc();
        const clear = () => doc.getAllMarks().map((a: TextMarker) => a.clear());

        clear();
        if (errors && errors.type === "CompileError" && errors.errors.length > 0) {
            errors.errors.map(e => doc.markText({ line: e.line - 1, ch: e.col }, { line: e.line - 1, ch: e.endCol },
                { className: "underline" }));
        }
    }
};

const CM = CodeMirror as any as FunctionalComponent<IControlledCodeMirror>;

const Comp: FunctionalComponent<Readonly<Props>> = ({ code, codeChanged, submitCode, errors }) => {
    setErrors(errors);

    return (<div>
        <div class="field">
            <label class="label">Code</label>
            <div class="control">
                <CM
                    value={code}
                    className={style.editor}
                    options={{ lineNumbers: true, mode: "text/x-csharp" }}
                    editorDidMount={(e: Editor) => {
                        editor = e;
                        setTimeout(() => {
                            e.refresh();
                        }, 250);
                    }}

                    onBeforeChange={(_: unknown, __: unknown, s: string) => codeChanged(s)} />
            </div>
        </div>
        <div class="field">
            <label class="label">Count</label>
            <div class="control">
                <label id="Count">{getScore(code)}</label>
            </div>
        </div>
        <div class="field is-grouped">
            <div class="control">
                <button class="button is-primary" onClick={() => submitCode(code)}>Submit</button>
            </div>

            <div class="buttons has-addons">
                <button class="button" onClick={() => openInAction("preview", code)}>View .CS file</button>

                <button class="button" onClick={() => openInAction("debug", code)}>View debug .CS file</button>
            </div>
        </div>
    </div>);
};

export default Comp;