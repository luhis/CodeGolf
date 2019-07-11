import * as codeMirror from "codemirror";
import "codemirror/mode/clike/clike";
import "codemirror/theme/bespin.css";
import { debounce } from "lodash";

const editor = codeMirror.fromTextArea(document.getElementById("Code"),
    {
        lineNumbers: true,
        matchBrackets: true,
        mode: "text/x-csharp"
    });

const codeSamples = document.getElementById("codeSample");
const codeInputs = document.getElementById("Code");

if (codeSamples && codeInputs) {
    codeSamples.addEventListener("click", () => {
        const doc = editor.getDoc();
        if (doc.getValue() === "") {
            doc.setValue(codeSamples.innerText);
        }
    });
    codeSamples.classList.add("is-clickable");
}

const setCodeErrors = errors => {
    const clear = () => editor.getDoc().getAllMarks().map(a => a.clear());
    
    if (errors.length > 0) {
        clear();
        errors.map(error => editor.getDoc().markText({ line: error.line - 1, ch: error.ch }, { line: error.line - 1, ch: 50 },
            { css: "text-decoration: underline; text-decoration-style: wavy" }));
    } else {
        clear();
    }
};

const codeErrorLocations = document.getElementById("CodeErrorLocations");
if (codeErrorLocations && codeErrorLocations.value) {
    setCodeErrors(JSON.parse(codeErrorLocations.value));
}

const count = document.getElementById("Count");
const challengeSetId = document.getElementById("ChallengeSetId");
if (count) {
    const updateCount = a => count.innerText = a.getDoc().getValue().replace(/\s/g, "").length;
    editor.on("changes", debounce(updateCount, 500));
    const updateCodeErrors = a => window.fetch(`./api/code/TryCompile/${challengeSetId.value}`,
        {
            method: "POST",
            body: JSON.stringify(a.getDoc().getValue()),
            headers: {
                'Content-Type': "application/json"
            }
        }).then(r => {
        if (!r.ok) {
            throw Error(r.statusText);
        }
        return r;
    }).then(response => {
        response.json().then(data => {
            setCodeErrors(data);
        });
    });
    editor.on("changes", debounce(updateCodeErrors, 500));
    updateCount(editor);
}
