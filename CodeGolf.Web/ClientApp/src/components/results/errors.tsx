import { FunctionalComponent, h } from "preact";

import { getInput } from "../../funcDeclaration";
import { CodeError, CompileError, RunError, RunErrorSet } from "../../types/types";
import Icon from "../icons";

interface Props { readonly errors: RunErrorSet | CompileError; }

const CompileErrorView: FunctionalComponent<{ readonly error: CodeError }> = ({ error }) => (<p>
    <Icon icon="exclamation-triangle" />
    <strong>{`(${error.line},${error.col}) ${error.message}`}</strong>
</p>);

const RunErrorView: FunctionalComponent<{ readonly error: RunError }> = ({ error }) => error.error ? (<p>
    <Icon icon="exclamation-triangle" />&nbsp;
    Input: {getInput(error.challenge)}&nbsp;
    <strong>{error.error.message} Expected: <pre class="result">{error.error.expected}</pre> Received: <pre class="result">{error.error.found}</pre></strong>
</p>) : (<p class="has-text-success has-background-white">
    <Icon icon="check-circle" />&nbsp;
    Input: {getInput(error.challenge)}&nbsp;
    <strong>Pass</strong>
</p>);

const hasError = (e: RunErrorSet | CompileError) => (e.type === "RunError" && e.errors.length) || (e.type === "CompileError" && e.errors.length);

const getError = (e: RunErrorSet | CompileError) => {
    switch (e.type) {
        case "RunError":
            return e.errors.map(a => <RunErrorView error={a} key={a.error} />);
        case "CompileError":
            return e.errors.map((a, i) => <CompileErrorView error={a} key={a.message + i} />);
    }
};

const Comp: FunctionalComponent<Props> = ({ errors }) => hasError(errors) ? (
    <article class="message is-danger">
        <div class="message-header">
            <p>Error</p>
        </div>
        <div class="message-body">
            {getError(errors)}
        </div>
    </article>) : null;

export default Comp;
