import * as CodeMirror from 'codemirror';
import 'codemirror/mode/clike/clike';
import 'codemirror/theme/bespin.css';

CodeMirror.fromTextArea(document.getElementById("Code"),
    {
        lineNumbers: true,
        matchBrackets: true,
        mode: "text/x-csharp"
    });