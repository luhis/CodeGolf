import * as codeMirror from "codemirror";
import "codemirror/mode/clike/clike";
import "codemirror/theme/bespin.css";

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
const codeErrorLocations = document.getElementById("CodeErrorLocations");
if (codeErrorLocations && codeErrorLocations.value) {
    const locs = codeErrorLocations.value.split(",");
    const t = locs.map(a => {
        var s = a.split(":");
        return { line: parseInt(s[0]), ch: parseInt(s[1]) };
    });
    t.map(error =>  editor.addLineClass(error.line - 1, "gutter", "line-error"));
}

const count = document.getElementById("Count");
if (count) {
    const updateCount = a => count.innerText = a.getDoc().getValue().replace(/\s/g, "").length;
    editor.on("changes", updateCount);
}
updateCount(editor);
