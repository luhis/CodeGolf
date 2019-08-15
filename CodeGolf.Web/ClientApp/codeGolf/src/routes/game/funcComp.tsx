import { FunctionalComponent, h } from "preact";
import { Circular } from "styled-loaders";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import ErrorsComp from "../../components/results";
import { Hole, LoadingState, RunResult } from "../../types/types";

interface Funcs {

  codeChanged: ((s: string) => Promise<void>);
  onCodeClick: (() => void);
  submitCode: ((code: string) => void);
}

type Props = ({
  code: string;
  errors: LoadingState<RunResult | undefined>;
  challenge: LoadingState<Hole | null>;
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
  code: string;
  challenge: Hole | null;
  errors: LoadingState<RunResult | undefined>;
} & Funcs> = ({ challenge, code, errors, codeChanged, submitCode, onCodeClick }) => (
  challenge ?
    (<div class="columns">
      <div class="column">
        <CodeEditor code={code} codeChanged={codeChanged} errors={valueOr(errors, () => undefined)} submitCode={submitCode} />
      </div>
      <div class="column">
        <ChallengeComp
          challenge={challenge.challengeSet}
          onCodeClick={onCodeClick}
        />

        {errors.type === "Loaded" ?
          <ErrorsComp errors={errors.data} returnType={challenge.challengeSet.returnType} /> : <Circular />}
      </div>
    </div>) : <PleaseWait />
);

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ code, errors, challenge, codeChanged, onCodeClick, submitCode }) => {
  return (<section class="section">
    <h1 class="title">Game</h1>
    {challenge.type === "Loaded" ?
      <HasChallenge challenge={challenge.data} code={code} errors={errors} codeChanged={codeChanged} onCodeClick={onCodeClick} submitCode={submitCode} /> : <Circular />}
  </section>);
};

export default FuncComp;
