import { FunctionalComponent, h } from "preact";

import ChallengeComp from "../../components/challenge";
import CodeEditor from "../../components/codeEditor";
import GamePicker from "../../components/gamePicker";
import Loading from "../../components/loading";
import ErrorsComp from "../../components/results";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { CompileError, GameId, Hole, RunResultSet, Score } from "../../types/types";

interface Funcs {
  readonly codeChanged: (s: string) => Promise<void>;
  readonly onCodeClick: () => void;
  readonly submitCode: (code: string) => void;
}

type Props = {
  readonly code: string;
  readonly gameId: GameId | undefined;
  readonly runResult: LoadingState<Score | CompileError | undefined>;
  readonly runErrors: RunResultSet | undefined;
  readonly challenge: LoadingState<Hole | undefined>;
} & Funcs & { readonly verifyPasscode: (passcode: string) => Promise<void> };

const PleaseWait: FunctionalComponent = () => (<div class="notification is-info">
  Please wait for the hole to begin
</div>);

const HasChallenge: FunctionalComponent<{
  readonly code: string;
  readonly runResult: LoadingState<Score | CompileError | undefined>;
  readonly runErrors: RunResultSet | undefined;
  readonly challenge: Hole | undefined;
} & Funcs> = ({ challenge, code, runResult, runErrors, codeChanged, submitCode, onCodeClick }) => (
  challenge ?
    (<div class="columns">
      <div class="column is-half">
        <CodeEditor code={code} codeChanged={codeChanged} runResult={ifLoaded(runResult, e => e, () => undefined)} submitCode={submitCode} />
      </div>
      <div class="column is-half">
        <ChallengeComp
          challengeSet={challenge.challengeSet}
          errors={runErrors}
          onCodeClick={onCodeClick}
        />

        {ifLoaded(runResult, e =>
          e ? <ErrorsComp runResult={e} /> : null, () => <Loading />)}
      </div>
    </div>) : <PleaseWait />
);

const FuncComp: FunctionalComponent<Props> = ({ code, gameId, runResult, runErrors, challenge, codeChanged, onCodeClick, submitCode, verifyPasscode }) => {
  return (<section class="section">
    {gameId ? null : <GamePicker verifyCode={verifyPasscode} />}
    <h1 class="title">Game</h1>
    {ifLoaded(challenge, c =>
      <HasChallenge challenge={c} code={code} runResult={runResult} runErrors={runErrors} codeChanged={codeChanged} onCodeClick={onCodeClick} submitCode={submitCode} />,
      () => <Loading />)}
  </section>);
};

export default FuncComp;
