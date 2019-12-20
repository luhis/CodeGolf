import { FunctionalComponent, h } from "preact";
import { useRef } from "preact/hooks";
import ReCAPTCHA from "react-google-recaptcha";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import Loading from "../../components/loading";
import ErrorsComp from "../../components/results";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { ChallengeSet, CompileError, RunResultSet, Score } from "../../types/types";

interface Props {
  readonly code: string;
  readonly runResult: LoadingState<Score | CompileError | undefined>;
  readonly runErrors: RunResultSet | undefined;
  readonly challenge: LoadingState<ChallengeSet>;
  readonly codeChanged: (s: string) => Promise<void>;
  readonly onCodeClick: () => void;
  readonly submitCode: (code: string, recaptcha: string) => void;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ code, runResult, runErrors, challenge, codeChanged, onCodeClick, submitCode }) => {
  const verifyCallback = (response: string) => {
    submitCode(code, response);
  };
  const recaptchaInstance = useRef<any | undefined>(undefined);

  const executeCaptcha = () => {
    recaptchaInstance.current.execute();
  };

  return (<section class="section">
    <h1 class="title">Demo</h1>
    <div class="columns">
      <div class="column is-half">
        <CodeEditor code={code} codeChanged={codeChanged} runResult={ifLoaded(runResult, e => e, () => undefined)} submitCode={executeCaptcha} />
      </div>
      <div class="column is-half">
        {ifLoaded(challenge, c => <ChallengeComp
          challengeSet={c}
          onCodeClick={onCodeClick}
          errors={runErrors}
        />, () => <Loading />)}
        {ifLoaded(runResult, some =>
          some ? <ErrorsComp runResult={some} /> : null, () => <Loading />)}
      </div>
    </div>
    <ReCAPTCHA
      // eslint-disable-next-line functional/immutable-data
      ref={e => recaptchaInstance.current = e}
      sitekey={process.env.PREACT_APP_RECAPTCHA_SITEKEY as string}
      size="invisible"
      onChange={verifyCallback}
    />
  </section>);
};

export default FuncComp;
