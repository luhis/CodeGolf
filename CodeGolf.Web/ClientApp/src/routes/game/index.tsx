import { HubConnection } from "@aspnet/signalr";
import debounce from "lodash.debounce";
import { FunctionComponent, h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { getCurrentHole, submitChallenge, tryCompile } from "../../api/playerApi";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { CompileError, GameId, Hole, RunResultSet, Score } from "../../types/types";
import FuncComp from "./funcComp";
import Notification from "./notification";

interface State {
  readonly gameId: GameId | undefined;
  readonly challenge: LoadingState<Hole | undefined>;
  readonly code: string;
  readonly runResult: LoadingState<Score | CompileError | undefined>;
  readonly runErrors: RunResultSet | undefined;
  readonly connection: HubConnection | undefined;
}

const Comp: FunctionComponent = () => {
  const [state, setState] = useState<State>(
    { challenge: { type: "Loading" }, code: "", runResult: { type: "Loaded", data: undefined }, runErrors: undefined, connection: undefined, gameId: undefined });
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
        if (c) {
          const funcDec = getFunctionDeclaration(c.challengeSet);
          setState(s => ({ ...s, code: funcDec }));
        }
      }, () => undefined);
    }
  }
  const submitCode = async (code: string) => {
    ifLoaded(state.challenge, async c => {
      if (c) {
        setState(s => ({ ...s, code, runResult: { type: "Loading" } }));
        const r = await submitChallenge(code, c.hole.holeId);
        if (r.type === "RunResultSet") {
          setState(s => ({ ...s, runResult: { type: "Loaded", data: undefined }, runErrors: r }));
        }
        else if (r.type === "Score") {
          const passedChallenges = c.challengeSet.challenges.map(_ => ({ error: undefined }));
          setState(s => ({ ...s, runResult: { type: "Loaded", data: r }, runErrors: { type: "RunResultSet", errors: passedChallenges } }));
        }
        else {
          setState(s => ({ ...s, runResult: { type: "Loaded", data: r } }));
        }
      }
    }, () => undefined);
  }

  useEffect(() => {
    const f = async () => {
      const connection = Notification(async () => {
        setState(s => ({ ...s, challenge: { type: "Loading" } }));
        const c = await getCurrentHole();
        setState(s => ({ ...s, challenge: { type: "Loaded", data: c }, runResult: { type: "Loaded", data: undefined }, code: "" }));
      });
      const challenge = await getCurrentHole();
      setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge }, connection }));
    };
    // tslint:disable-next-line: no-floating-promises
    f();
    return async () => {
      if (state.connection) {
        await state.connection.stop();
      }
    };
  }, []);
  return (<FuncComp code={state.code}
    onCodeClick={onCodeClick}
    runResult={state.runResult} runErrors={state.runErrors}
    challenge={state.challenge} codeChanged={codeChanged} submitCode={submitCode} />);
};

export default  Comp;
