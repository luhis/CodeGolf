import { FunctionComponent, h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { getFinalResults } from "../../api/adminApi";
import { LoadingState } from "../../types/appTypes";
import { Result } from "../../types/types";
import FuncComp from "./funcComp";

type State = LoadingState<ReadonlyArray<Result>>;

const Comp: FunctionComponent = () => {
  const [state, setState] = useState<State>({ type: "Loading" });
  useEffect(() => {
    const a = async () => {
      const results = await getFinalResults();
      setState(() => ({ type: "Loaded", data: results }));
    };
    // tslint:disable-next-line: no-floating-promises
    a();
  }, []);
  return <FuncComp results={state} />;
};

export default Comp;
