import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
import { FunctionComponent, h } from "preact";
import { route } from "preact-router";
import { useEffect, useState } from "preact/hooks";

import { endHole, getCurrentChallenge, getResults, nextHole } from "../../api/adminApi";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { Attempt, Hole } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
  readonly currentHole: LoadingState<Hole | undefined>;
  readonly attempts: LoadingState<ReadonlyArray<Attempt>>;
}

const connection = new HubConnectionBuilder().withUrl("/refreshHub").configureLogging(LogLevel.Error).build();
connection.on("newAnswer", getResults);

const Comp: FunctionComponent = () => {
  const [state, setState] = useState<State>({ currentHole: { type: "Loading" }, attempts: { type: "Loaded", data: [] } });
  const getResultsX = async () => {
    return ifLoaded(state.currentHole, async hole => {
      if (hole) {
        const results = await getResults(hole.hole.holeId);
        setState(s => ({ ...s, attempts: { type: "Loaded", data: results } }));
      }

    }, () => Promise.resolve());
  };

  const getHole = async () => {
    try {
      const hole = await getCurrentChallenge();
      setState(s => ({ ...s, currentHole: { type: "Loaded", data: hole } }));
    }
    catch {
      setState(s => ({ ...s, currentHole: { type: "Loaded", data: undefined } }));
    }
  };

  const doThenUpdateHole = (f: () => Promise<void>) => async () => {
    await f();
    await getHole();
  };

  useEffect(() => {
    const a = async () => {
      await connection.start().catch(console.error);
      await getHole();
    };
    // tslint:disable-next-line: no-floating-promises
    a();
    return () => {
      return connection.stop();
    };
  }, []);
  useEffect(() => {
    // tslint:disable-next-line: no-floating-promises
    getResultsX();
  }, [state.currentHole]);
  return ifLoaded(state.currentHole, x => {
    const f = x ? endHole : async () => { await endHole(); route("/results"); };
    return (<FuncComp
      current={state.currentHole}
      attempts={state.attempts}
      nextHole={doThenUpdateHole(nextHole)}
      endHole={doThenUpdateHole(f)} />);
  },
    () => null);
};

export default Comp;
