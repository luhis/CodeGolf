import { FunctionalComponent, h } from "preact";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import Loading from "../../components/loading";
import ErrorsComp from "../../components/results";
import { Hole, ifLoaded, LoadingState, RunResult, RunResultSet } from "../../types/types";

interface Funcs {
  readonly codeChanged: (s: string) => Promise<void>;
  readonly onCodeClick: () => void;
  readonly submitCode: (code: string) => void;
}

type Props = {
  readonly code: string;
  readonly errors: LoadingState<RunResult | undefined>;
  readonly challenge: LoadingState<Hole | undefined>;
} & Funcs;

const PleaseWait: FunctionalComponent = () => (<div class="notification is-info">
  Please wait for the hole to begin
</div>);

const onlyRunErrors = (l: LoadingState<RunResult | undefined>): LoadingState<RunResultSet | undefined> =>
  ifLoaded<RunResult | undefined, LoadingState<RunResultSet | undefined>>(
    l,
    some => ({ type: "Loaded", data: (some && some.type === "RunResultSet" ? some : undefined) }),
    () => ({ type: "Loading" }));

const HasChallenge: FunctionalComponent<{
  readonly code: string;
  readonly challenge: Hole | undefined;
  readonly errors: LoadingState<RunResult | undefined>;
} & Funcs> = ({ challenge, code, errors, codeChanged, submitCode, onCodeClick }) => (
  challenge ?
    (<div class="columns">
      <div class="column is-half">
        <CodeEditor code={code} codeChanged={codeChanged} errors={ifLoaded(errors, e => e, () => undefined)} submitCode={submitCode} />
      </div>
      <div class="column is-half">
        <ChallengeComp
          challengeSet={challenge.challengeSet}
          errors={onlyRunErrors(errors)}
          onCodeClick={onCodeClick}
        />

        {ifLoaded(errors, e =>
          e && (e.type === "Score" || e.type === "CompileError") ? <ErrorsComp errors={e} /> : null, () => <Loading />)}
      </div>
    </div>) : <PleaseWait />
);

const FuncComp: FunctionalComponent<Props> = ({ code, errors, challenge, codeChanged, onCodeClick, submitCode }) => {
  return (<section class="section">
    <h1 class="title">Game</h1>
    {ifLoaded(challenge, c =>
      <HasChallenge challenge={c} code={code} errors={errors} codeChanged={codeChanged} onCodeClick={onCodeClick} submitCode={submitCode} />,
      () => <Loading />)}
  </section>);
};

export default FuncComp;
