import * as codeMirror from "codemirror";
import "codemirror/mode/clike/clike";
import "codemirror/theme/bespin.css";
import { debounce } from "lodash";
var editor = codeMirror.fromTextArea(document.getElementById("Code"), {
    lineNumbers: true,
    mode: "text/x-csharp"
});
var codeSamples = document.getElementById("codeSample");
var codeInputs = document.getElementById("Code");
if (codeSamples && codeInputs) {
    codeSamples.addEventListener("click", function () {
        var doc = editor.getDoc();
        if (doc.getValue() === "") {
            doc.setValue(codeSamples.innerText);
        }
    });
    codeSamples.classList.add("is-clickable");
}
var setCodeErrors = function (errors) {
    var clear = function () { return editor.getDoc().getAllMarks().map(function (a) { return a.clear(); }); };
    if (errors.length > 0) {
        clear();
        errors.map(function (error) { return editor.getDoc().markText({ line: error.line, ch: error.ch }, { line: error.line, ch: error.endCh }, { className: "underline" }); });
    }
    else {
        clear();
    }
};
var codeErrorLocations = document.getElementById("CodeErrorLocations");
if (codeErrorLocations && codeErrorLocations.value) {
    setCodeErrors(JSON.parse(codeErrorLocations.value));
}
;
var count = document.getElementById("Count");
var challengeSetId = document.getElementById("ChallengeSetId");
if (count) {
    var updateCount = function (a) { return count.innerText = a.getDoc().getValue().replace(/\s/g, "").length.toString(); };
    editor.on("changes", debounce(updateCount, 500));
    var updateCodeErrors = function (a) { return window.fetch("./api/code/TryCompile/" + challengeSetId.value, {
        method: "POST",
        body: JSON.stringify(a.getDoc().getValue()),
        headers: {
            'Content-Type': "application/json"
        }
    }).then(function (r) {
        if (!r.ok) {
            throw Error(r.statusText);
        }
        return r;
    }).then(function (response) {
        response.json().then(function (data) {
            setCodeErrors(data);
        });
    }); };
    editor.on("changes", debounce(updateCodeErrors, 500));
    updateCount(editor);
}
