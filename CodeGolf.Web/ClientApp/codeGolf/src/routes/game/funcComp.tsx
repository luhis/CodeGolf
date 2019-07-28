import { FunctionalComponent, h } from "preact";
import { Circular } from "styled-loaders";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import ErrorsComp from "../../components/results";
import { Hole, LoadingState, RunResult } from "../../types/types";

interface Props {
  code: string;
  errors: LoadingState<RunResult | undefined>;
  challenge: LoadingState<Hole>;
  codeChanged: ((s: string) => Promise<void>);
  onCodeClick: (() => void);
  submitCode: ((code: string) => void);
}

const PleaseWait: FunctionalComponent = () => (<div class="notification is-info">
  Please wait for the hole to begin
</div>);

const valueOr = <T extends any>(l: LoadingState<T>, f: (() => T)): T => {
  if (l.type === "Loaded") {
    return l.data;
  }
  
    return f();
  
};

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ code, errors, challenge, codeChanged, onCodeClick, submitCode }) => {
  return (<section class="section">
    <h1 class="title">Game</h1>
    {challenge.type === "Loaded" ?
      <div class="columns">
        <div class="column">
          <CodeEditor code={code} codeChanged={codeChanged} errors={valueOr(errors, () => undefined)} submitCode={submitCode} />
        </div>
        <div class="column">
          <ChallengeComp
            challenge={challenge.data.challengeSet}
            onCodeClick={onCodeClick}
          />

          {challenge.type === "Loaded" && errors.type === "Loaded" ?
            <ErrorsComp errors={errors.data} returnType={challenge.data.challengeSet.returnType} /> : <Circular/>}
        </div>
      </div> : <PleaseWait />}
  </section>);
};

export default FuncComp;
