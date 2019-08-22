import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
import { FunctionComponent, h } from "preact";
import { route } from "preact-router";
import { useEffect, useState } from "preact/hooks";

import { endHole, getCurrentChallenge, getResults, nextHole } from "../../api/adminApi";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { Attempt, GameId, Hole } from "../../types/types";
import FuncComp from "./funcComp";
import { verifyGameCode } from "../../api/playerApi";

interface State {
  readonly gameId: GameId | undefined;
  readonly currentHole: LoadingState<Hole | undefined>;
  readonly attempts: LoadingState<ReadonlyArray<Attempt>>;
}

const connection = new HubConnectionBuilder().withUrl("/refreshHub").configureLogging(LogLevel.Error).build();

const Comp: FunctionComponent = () => {
  const [state, setState] = useState<State>({ currentHole: { type: "Loading" }, attempts: { type: "Loaded", data: [] }, gameId: undefined });
  const getResultsX = async (currentHole: LoadingState<Hole | undefined>) => ifLoaded(currentHole, async hole => {
    if (hole) {
      const results = await getResults(hole.hole.holeId);
      setState(s => ({ ...s, attempts: { type: "Loaded", data: results } }));
    }

  }, () => Promise.resolve());

  const getHole = async (gameId: GameId) => {
    try {
      const hole = await getCurrentChallenge(gameId);
      setState(s => ({ ...s, currentHole: { type: "Loaded", data: hole } }));
    }
    catch {
      setState(s => ({ ...s, currentHole: { type: "Loaded", data: undefined } }));
    }
  };

  const verCode = async (s: string) => {
    try {
      const gameId = await verifyGameCode(s);
      setState(s => ({ ...s, gameId }));
    }
    finally {
    }
  };

  const doThenUpdateHole = (f: (g: GameId) => Promise<void>) => async (g: GameId) => {
    await f(g);
    await getHole(g);
  };

  useEffect(() => {
    const a = async () => {
      await connection.start().catch(console.error);
      if (state.gameId) {
        await getHole(state.gameId);
      }
    };
    // eslint-disable-next-line @typescript-eslint/no-floating-promises
    a();
    return () => connection.stop();
  }, []);
  useEffect(() => {
    // eslint-disable-next-line @typescript-eslint/no-floating-promises
    getResultsX(state.currentHole);
    connection.on("newAnswer", () => getResultsX(state.currentHole));
  }, [state.currentHole]);
  return ifLoaded(state.currentHole, x => {
    const f = x ? endHole : async (id: GameId) => { await endHole(id); route("/results"); };
    return (<FuncComp
      setGameCode={verCode}
      gameId={state.gameId}
      current={state.currentHole}
      attempts={state.attempts}
      nextHole={doThenUpdateHole(nextHole)}
      endHole={doThenUpdateHole(f)} />);
  },
    () => null);
};

export default Comp;
