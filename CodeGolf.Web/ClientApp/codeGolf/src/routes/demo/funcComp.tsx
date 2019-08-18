import { FunctionalComponent, h } from "preact";
import ReCAPTCHA from "preact-google-recaptcha";
import { Circular } from "styled-loaders";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import ErrorsComp from "../../components/results";
import { ChallengeSet, LoadingState, RunResult } from "../../types/types";

interface Props {
  readonly code: string;
  readonly errors: LoadingState<RunResult | undefined>;
  readonly challenge: LoadingState<ChallengeSet>;
  readonly codeChanged: ((s: string) => Promise<void>);
  readonly onCodeClick: (() => void);
  readonly submitCode: ((code: string, recaptcha: string) => void);
}
// tslint:disable-next-line: no-let
let recaptchaInstance: any = null;

const executeCaptcha = () => {
  recaptchaInstance.execute();
};

const valueOr = <T extends any>(l: LoadingState<T>, f: (() => T)): T => {
  if (l.type === "Loaded") {
    return l.data;
  }

  return f();
};

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ code, errors, challenge, codeChanged, onCodeClick, submitCode }) => {
  const verifyCallback = (response: string) => {
    submitCode(code, response);
  };
  return (<section class="section">
    <h1 class="title">Demo</h1>
    <div class="columns">
      <div class="column is-half">
        <CodeEditor code={code} codeChanged={codeChanged} errors={valueOr(errors, () => undefined)} submitCode={executeCaptcha} />
      </div>
      <div class="column is-half">
        {challenge.type === "Loaded" ? <ChallengeComp
          challenge={challenge.data}
          onCodeClick={onCodeClick}
        /> : <Circular />}
        {challenge.type === "Loaded" && errors.type === "Loaded" ?
          errors.data ? <ErrorsComp errors={errors.data} returnType={challenge.data.returnType} /> : null
          : <Circular />}
      </div>
    </div>
    <ReCAPTCHA
      ref={e => recaptchaInstance = e}
      sitekey={process.env.PREACT_APP_RECAPTCHA_SITEKEY as string}
      size="invisible"
      onChange={verifyCallback}
    />
  </section>);
};

export default FuncComp;
