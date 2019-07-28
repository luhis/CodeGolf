import { faCheckCircle, faExclamationTriangle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionalComponent, h } from "preact";

import { getChallengeOverView } from "../../funcDeclaration";
import { CompileError, Error, RunError, RunErrorSet } from "../../types/types";

interface Props { errors: RunErrorSet | CompileError; returnType: string; }

const CompileErrorView: FunctionalComponent<{ error: Error }> = ({ error }) => (<p>
    <FontAwesomeIcon icon={faExclamationTriangle} />
    <strong>{`(${error.line},${error.col}) ${error.message}`}</strong>
</p>);

const RunErrorView: FunctionalComponent<{ error: RunError, returnType: string }> = ({ error, returnType }) => error.error ? (<p>
    <FontAwesomeIcon icon={faExclamationTriangle} />
    {getChallengeOverView(error.challenge, returnType)}&nbsp;
    <strong>{`${error.error}`}</strong>
</p>) : (<p class="has-text-success has-background-white">
    <FontAwesomeIcon icon={faCheckCircle} />
    {getChallengeOverView(error.challenge, returnType)}&nbsp;
    <strong>Pass</strong>
</p>);

const hasError = (e: RunErrorSet | CompileError) => (e.type === "RunError" && e.errors.length) || (e.type === "CompileError" && e.errors.length);

const getError = (e: RunErrorSet | CompileError, returnType: string) => {
    switch (e.type) {
        case "RunError":
            return e.errors.map(a => <RunErrorView error={a} key={a.error} returnType={returnType} />);
        case "CompileError":
            return e.errors.map((a, i) => <CompileErrorView error={a} key={a.message + i} />);
    }
};

const Comp: FunctionalComponent<Props> = ({ errors, returnType }) => hasError(errors) ? (
    <article class="message is-danger">
        <div class="message-header">
            <p>Error</p>
        </div>
        <div class="message-body">
            {getError(errors, returnType)}
        </div>
    </article>) : null;

export default Comp;