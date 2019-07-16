<template>
<div class="columns">
  <div class="column">
        <codemirror v-model="code" :options="cmOption"></codemirror>
        <div class="field is-grouped">
                <div class="control">
                    <button class="button is-primary" type="submit">Submit</button>
                </div>

                <div class="buttons has-addons">
                    <button class="button" type="submit" asp-page-handler="ViewSource" formtarget="_blank">View .CS file</button>

                    <button class="button" type="submit" asp-page-handler="ViewDebugSource" formtarget="_blank">View debug .CS file</button>
                </div>
            </div>
  </div>
  <div class="column">
    <div class="panel">
    <div class="panel-heading">
        <p>Title</p>
    </div>
    <div class="panel-block">
        <div class="content">Description</div>
    </div>


        <div class="panel-block">Challenge here</div>


        <div class="panel-block">
            <i class="fas fa-info-circle has-text-info"></i>&nbsp;
            Create a function like&nbsp;<code id="codeSample">() => string</code>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="tsx">
  // language
  import 'codemirror/mode/clike/clike.js'

  // theme css
  import 'codemirror/theme/monokai.css'

  // require active-line.js
  import'codemirror/addon/selection/active-line.js'

  // styleSelectedText
  import'codemirror/addon/selection/mark-selection.js'
  import'codemirror/addon/search/searchcursor.js'

  // highlightSelectionMatches
  import'codemirror/addon/scroll/annotatescrollbar.js'
  import'codemirror/addon/search/matchesonscrollbar.js'
  import'codemirror/addon/search/searchcursor.js'
  import'codemirror/addon/search/match-highlighter.js'

  // keyMap
  import'codemirror/mode/clike/clike.js'
  import'codemirror/addon/edit/matchbrackets.js'
  import'codemirror/addon/comment/comment.js'
  import'codemirror/keymap/sublime.js'

  import { codemirror } from 'vue-codemirror';
  
  export default {
    data(): {code: string, cmOption: Object} {
      return {
        code :
`string Main() => "Hello World";`,
        cmOption: {
          tabSize: 4,
          styleActiveLine: false,
          lineNumbers: true,
          styleSelectedText: false,
          line: true,
          gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
          highlightSelectionMatches: { showToken: /\w/, annotateScrollbar: true },
          mode: 'text/x-csharp',
          keyMap: "sublime",
          matchBrackets: true,
          showCursorWhenSelecting: true,
          theme: "monokai",
        }
      }
    },
    mounted() {
      setTimeout(() => {
        (this as any).styleSelectedText = true,
        (this as any).cmOption.styleActiveLine = true
      }, 1800)
    },
components: {
    'codemirror': codemirror,
},
  }
</script>