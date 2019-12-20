import { FunctionComponent, h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { getAttempt } from "../../api/adminApi";
import { LoadingState } from "../../types/appTypes";
import { AttemptId, AttemptWithCode } from "../../types/types";
import FuncComp from "./funcComp";

type State = LoadingState<AttemptWithCode>;

interface Props { readonly attemptId: AttemptId }

const Comp: FunctionComponent<Props> = ({ attemptId }) => {
  const [state, setState] = useState<State>({ type: "Loading" });
  useEffect(() => {
    const a = async () => {
      const results = await getAttempt(attemptId);
      setState(() => ({ type: "Loaded", data: results }));
    };
    // eslint-disable-next-line @typescript-eslint/no-floating-promises
    a();
  }, []);
  return <FuncComp result={state} />;
};

export default Comp;
