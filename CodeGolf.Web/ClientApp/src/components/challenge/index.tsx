import ClassNames from "classnames";
import Markdown from "markdown-to-jsx";
import { Fragment, FunctionalComponent, h } from "preact";

import { getFunctionDeclaration, getInput } from "../../funcDeclaration";
import { Challenge, ChallengeSet, RunError, RunResultSet } from "../../types/types";
import Icon from "../icons";

interface Props {
  readonly challengeSet: ChallengeSet;
  readonly errors: RunResultSet | undefined;
  readonly onCodeClick?: () => void;
}

const ResultsCell: FunctionalComponent<{ readonly challenge: Challenge, readonly runError?: RunError }> =
  ({ challenge, runError }) => (<td>
    {runError ?
      <Fragment>
        <pre class={ClassNames("result", runError.error ? "has-background-danger" : "has-background-success")}>
          {runError.error ? runError.error.found : challenge.expectedResult}
        </pre>
        {runError.error ? runError.error.message : null}
      </Fragment>
      : null}
  </td>);

const Row: FunctionalComponent<{ readonly challenge: Challenge, readonly runError?: RunError, readonly showResults: boolean }> =
  ({ challenge, runError, showResults }) =>
    (<tr>
      <td>{getInput(challenge)} =></td>
      <td><pre class="result">{challenge.expectedResult}</pre></td>
      {showResults ? <ResultsCell challenge={challenge} runError={runError} /> : null}
    </tr>);

const Comp: FunctionalComponent<Props> = ({ challengeSet, onCodeClick, errors }) => {
  const getError = (index: number) => errors && errors.type === "RunResultSet" ? errors.errors[index] : undefined;
  const showResults = !!errors;
  const pairs = challengeSet.challenges.map((challenge, index) => ({ challenge, result: getError(index) }));
  return (<div class="panel">
    <div class="panel-heading">
      <p>{challengeSet.title}</p>
    </div>
    <div class="panel-block">
      <div class="content">
        <Markdown>
          {challengeSet.description}
        </Markdown>
      </div>
    </div>
    <table class="table is-fullwidth">
      <thead>
        <tr>
          <th>Input</th>
          <th>Expected</th>
          {showResults ? <th>Received</th> : null}
        </tr>
      </thead>
      <tbody>
        {pairs.map(a => <Row key={a.challenge.expectedResult.toString()} challenge={a.challenge} runError={a.result} showResults={showResults} />)}
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
