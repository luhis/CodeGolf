import { FunctionalComponent, h } from "preact";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import Loading from "../../components/loading";
import ErrorsComp from "../../components/results";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { CompileError, Hole, RunResultSet, Score } from "../../types/types";

interface Funcs {
  readonly codeChanged: (s: string) => Promise<void>;
  readonly submitCode: (code: string) => void;
}

type Props = {
  readonly errors: LoadingState<Score | CompileError | undefined>;
  readonly runErrors: RunResultSet | undefined;
  readonly challenge: LoadingState<Hole | undefined>;
} & Funcs;

const PleaseWait: FunctionalComponent = () => (<div class="notification is-info">
  Please wait for the hole to begin
</div>);

const HasChallenge: FunctionalComponent<{
  readonly errors: LoadingState<Score | CompileError | undefined>;
  readonly runErrors: RunResultSet | undefined;
  readonly challenge: Hole | undefined;
} & Funcs> = ({ challenge, errors, runErrors, codeChanged, submitCode }) => (
  challenge ?
    (<div class="columns">
      <div class="column is-half">
        <CodeEditor code={getFunctionDeclaration(challenge.challengeSet)} codeChanged={codeChanged} errors={ifLoaded(errors, e => e, () => undefined)} submitCode={submitCode} />
      </div>
      <div class="column is-half">
        <ChallengeComp
          challengeSet={challenge.challengeSet}
          errors={runErrors}
          onCodeClick={undefined}
        />

        {ifLoaded(errors, e =>
          e ? <ErrorsComp errors={e} /> : null, () => <Loading />)}
      </div>
    </div>) : <PleaseWait />
);

const FuncComp: FunctionalComponent<Props> = ({ errors, runErrors, challenge, codeChanged, submitCode }) => {
  return (<section class="section">
    <h1 class="title">Game</h1>
    {ifLoaded(challenge, c =>
      <HasChallenge challenge={c} errors={errors} runErrors={runErrors} codeChanged={codeChanged} submitCode={submitCode} />,
      () => <Loading />)}
  </section>);
};

export default FuncComp;
