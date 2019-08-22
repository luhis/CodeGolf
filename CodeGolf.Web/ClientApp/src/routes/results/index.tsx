import { FunctionComponent, h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { getFinalResults } from "../../api/adminApi";
import { LoadingState } from "../../types/appTypes";
import { GameId, Result } from "../../types/types";
import FuncComp from "./funcComp";

type State = LoadingState<ReadonlyArray<Result>>;
interface Props { readonly gameId: GameId; }

const Comp: FunctionComponent<Props> = ({ gameId }) => {
  const [state, setState] = useState<State>({ type: "Loading" });
  useEffect(() => {
    const a = async () => {
        const results = await getFinalResults(gameId);
      setState(() => ({ type: "Loaded", data: results }));
    };
    // eslint-disable-next-line @typescript-eslint/no-floating-promises
    a();
  }, []);
  return <FuncComp results={state} />;
};

export default Comp;
