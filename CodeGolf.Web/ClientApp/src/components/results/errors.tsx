import { FunctionalComponent, h } from "preact";

import { CodeError, CompileError } from "../../types/types";
import Icon from "../icons";

interface Props { readonly errors: CompileError; }

const CompileErrorView: FunctionalComponent<{ readonly error: CodeError }> = ({ error }) => (<p>
    <Icon icon="exclamation-triangle" />
    <strong>{`(${error.line},${error.col}) ${error.message}`}</strong>
</p>);

const hasError = (e: CompileError) => e.errors.length > 0;

const getError = (e: CompileError) => {
    return e.errors.map((a, i) => <CompileErrorView error={a} key={a.message + i} />);
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
