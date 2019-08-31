import { FunctionalComponent, h } from "preact";
import { Circular } from "styled-loaders";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import ErrorsComp from "../../components/results";
import { Hole, ifLoaded, LoadingState, RunResult } from "../../types/types";

interface Funcs {
  readonly codeChanged: ((s: string) => Promise<void>);
  readonly onCodeClick: (() => void);
  readonly submitCode: ((code: string) => void);
}

type Props = ({
  readonly code: string;
  readonly errors: LoadingState<RunResult | undefined>;
  readonly challenge: LoadingState<Hole | undefined>;
} & Funcs);

const PleaseWait: FunctionalComponent = () => (<div class="notification is-info">
  Please wait for the hole to begin
</div>);

const valueOr = <T extends any>(l: LoadingState<T>, f: (() => T)): T => {
  if (l.type === "Loaded") {
    return l.data;
  }

  return f();
};

const HasChallenge: FunctionalComponent<{
  readonly code: string;
  readonly challenge: Hole | undefined;
  readonly errors: LoadingState<RunResult | undefined>;
} & Funcs> = ({ challenge, code, errors, codeChanged, submitCode, onCodeClick }) => (
  challenge ?
    (<div class="columns">
      <div class="column is-half">
        <CodeEditor code={code} codeChanged={codeChanged} errors={valueOr(errors, () => undefined)} submitCode={submitCode} />
      </div>
      <div class="column is-half">
        <ChallengeComp
          challenge={challenge.challengeSet}
          onCodeClick={onCodeClick}
        />

        {ifLoaded(errors, e =>
          <ErrorsComp errors={e} returnType={challenge.challengeSet.returnType} />, () => <Circular />)}
      </div>
    </div>) : <PleaseWait />
);

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ code, errors, challenge, codeChanged, onCodeClick, submitCode }) => {
  return (<section class="section">
    <h1 class="title">Game</h1>
    {ifLoaded(challenge, c =>
      <HasChallenge challenge={c} code={code} errors={errors} codeChanged={codeChanged} onCodeClick={onCodeClick} submitCode={submitCode} />,
      () => <Circular />)}
  </section>);
};

export default FuncComp;
