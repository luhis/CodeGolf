import { ControlledEditor } from "@monaco-editor/react";
import { FunctionalComponent, h } from "preact";
import { useRef } from "preact/hooks";

import { RunResult } from "../../types/types";

interface Props {
    readonly code: string;
    readonly errors?: RunResult;
    readonly codeChanged: (s: string) => void;
    readonly submitCode: (s: string) => void;
}

const openInAction = (actionName: ("debug" | "preview"), code: string) =>
    window.open(`/codefile/${actionName}?code=${encodeURI(code)}`, "_blank");

const getScore = (code: string) => code
    .replace(/\s/g, "")
    .length;

const setErrors = (editorComp: any, errors?: RunResult) => {
    const doc = editorComp.getModel();
    const monaco = (window as any).monaco;
    if (errors && errors.type === "CompileError" && errors.errors.length > 0) {
        monaco.editor.setModelMarkers(doc, "error", errors.errors.map(e => ({
            startLineNumber: e.line - 1,
            startColumn: e.col + 1,
            endLineNumber: e.line - 1,
            endColumn: e.endCol + 1,
            message: "error"
        })));
    }
    else {
        monaco.editor.setModelMarkers(doc, "error", []);
    }
};

const Comp: FunctionalComponent<Readonly<Props>> = ({ code, codeChanged, submitCode, errors }) => {
    const editor = useRef<any | undefined>(undefined);
    if (editor.current) {
        setErrors(editor.current, errors);
    }
    return (<div>
        <div class="field">
            <label class="label">Code</label>
            <div class="control">
                <ControlledEditor
                    value={code}
                    height="40vh"
                    language="csharp"
                    // tslint:disable-next-line: no-object-mutation
                    editorDidMount={(_: () => string, e: any) => editor.current = e}
                    onChange={(_: unknown, s: string) => codeChanged(s)}
                    options={{minimap: {enabled: false}}}
                />
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
