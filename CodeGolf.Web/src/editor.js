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
    const locs = errors.split(",").filter(a => a !== "");
    const t = locs.map(a => {
        var s = a.split(":");
        return { line: parseInt(s[0]), ch: parseInt(s[1]) };
    });
    
    if (t.length > 0) {
        [...Array(editor.lineCount()).keys()].map(i => {
            editor.removeLineClass(i, "gutter");
        });
        t.map(error => editor.addLineClass(error.line - 1, "gutter", "line-error"));
    } else {
        [...Array(editor.lineCount()).keys()].map(i => editor.removeLineClass(i, "gutter"));
    }
};

const codeErrorLocations = document.getElementById("CodeErrorLocations");
if (codeErrorLocations && codeErrorLocations.value) {
    setCodeErrors(codeErrorLocations.value);
}

const count = document.getElementById("Count");
const challengeSetId = document.getElementById("ChallengeSetId");
if (count) {
    const updateCount = a => count.innerText = a.getDoc().getValue().replace(/\s/g, "").length;
    editor.on("changes", updateCount);
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
        response.text().then(data => {
            setCodeErrors(data);
        });
    });
    editor.on("changes", debounce(updateCodeErrors, 500));
    updateCount(editor);
}
