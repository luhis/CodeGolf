import ClassNames from "classnames";
import Markdown from "markdown-to-jsx";
import { FunctionalComponent, h } from "preact";

import { getFunctionDeclaration, getInput } from "../../funcDeclaration";
import { Challenge, ChallengeSet, ifLoaded, LoadingState, RunError, RunResult } from "../../types/types";
import Icon from "../icons";

interface Props {
    readonly challengeSet: ChallengeSet;
    readonly errors: LoadingState<RunResult | undefined>;
    readonly onCodeClick?: () => void;
}

const Row: FunctionalComponent<{ readonly challenge: Challenge, readonly runError?: RunError }> = ({ challenge, runError }) =>
    (<tr>
        <td>{getInput(challenge)} =></td>
        <td><pre class="result">{challenge.expectedResult}</pre></td>
        <td>
            {runError ?
                <pre class={ClassNames("result", runError.error ? "has-background-danger" : "has-background-success")}>
                    {runError.error ? runError.error.found : challenge.expectedResult}
                </pre>
                : null}
        </td>
    </tr>);

const Comp: FunctionalComponent<Readonly<Props>> = ({ challengeSet, onCodeClick, errors }) => {
    const getError = (index: number) => ifLoaded(errors, some => some && some.type === "RunResultSet" ? some.errors[index] : undefined, () => undefined);
    const pairs = challengeSet.challenges.map((challenge, index) => ({ challenge, result: getError(index) }));
    return (<div class="panel">
        <div class="panel-heading">
            <p>{challengeSet.title}</p>
        </div>
        <div class="panel-block">
            <div class="content">
                <Markdown>{challengeSet.description}
                </Markdown>
            </div>
        </div>
        <table class="table is-fullwidth">
            <thead>
                <tr>
                    <th>Input</th>
                    <th>Expected</th>
                    <th>Received</th>
                </tr>
            </thead>
            <tbody>
                {pairs.map(a => <Row key={a.challenge.expectedResult.toString()} challenge={a.challenge} runError={a.result} />)}
            </tbody>
        </table>
        <div class="panel-block">
            <Icon icon="info-circle" className="has-text-info" />&nbsp;
            Create a function like&nbsp;
            <code class={onCodeClick ? "is-clickable" : ""} onClick={onCodeClick}>{getFunctionDeclaration(challengeSet)}</code>
        </div>
    </div>);
};

export default Comp;
