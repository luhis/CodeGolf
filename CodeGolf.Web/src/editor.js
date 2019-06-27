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
}

const count = document.getElementById("Count");
editor.on("change", a => count.innerText = a.getDoc().getValue().replace(/\s/g, "").length);
