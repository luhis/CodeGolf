import debounce from "lodash.debounce";
import { FunctionComponent, h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { getDemoChallenge, submitDemo, tryCompile } from "../../api/playerApi";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { ChallengeSet, CompileError, RunResultSet, Score } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
  readonly challenge: LoadingState<ChallengeSet>;
  readonly code: string;
  readonly runErrors: RunResultSet | undefined;
  readonly runResult: LoadingState<Score | CompileError | undefined>;
}

const Comp: FunctionComponent = () => {
  const [state, setState] = useState<State>({ challenge: { type: "Loading" }, code: "", runResult: { type: "Loaded", data: undefined }, runErrors: undefined });

  useEffect(() => {
    const f = async () => {
      const challenge = await getDemoChallenge();
      setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge } }));
    };
    // tslint:disable-next-line: no-floating-promises
    f();
  }, []);

  const tryCompileX = debounce(async (code) => {
    const runResult = {
      type: "Loaded",
      data: {
        type: "CompileError",
        errors: await tryCompile(code)
      }
    } as LoadingState<CompileError>;
    setState(s => ({ ...s, runResult }));
  }, 1000);

  const codeChanged = debounce(async (code: string) => {
    setState(s => ({ ...s, code, runResult: { type: "Loading" } }));
    await tryCompileX(code);
  }, 250);

  const onCodeClick = () => {
    if (state.code === "") {
      ifLoaded(state.challenge, c => {
        const funcDec = getFunctionDeclaration(c);
        setState(s => ({ ...s, code: funcDec }));
      }, () => undefined);
    }
  };

  const submitCode = async (code: string, reCaptcha: string) => {
    setState(s => ({ ...s, runResult: { type: "Loading" } }));
    const r = await submitDemo(code, reCaptcha);
    if (r.type === "RunResultSet") {
      setState(s => ({ ...s, runResult: { type: "Loaded", data: undefined }, runErrors: r }));
    }
    else if (r.type === "Score") {
      const passedChallenges = ifLoaded(state.challenge, c => c.challenges.map(_ => ({ error: undefined })), () => []);
      setState(s => ({ ...s, runResult: { type: "Loaded", data: r }, runErrors: { type: "RunResultSet", errors: passedChallenges } }));
    }
    else {
      setState(s => ({ ...s, runResult: { type: "Loaded", data: r } }));
    }
  };
  return (<FuncComp code={state.code}
    runErrors={state.runErrors}
    runResult={state.runResult}
    challenge={state.challenge}
    codeChanged={codeChanged}
    submitCode={submitCode}
    onCodeClick={onCodeClick} />
  );
};

export default Comp;
