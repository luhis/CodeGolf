import { FunctionalComponent, h } from "preact";
import { useState } from "preact/hooks";

import { addGame } from "../../api/adminApi";
import { GameId, Round } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
  readonly accessKey: string;
}

interface Props {
  readonly hide: () => void;
  readonly challenges: ReadonlyArray<Round>;
}

const Comp: FunctionalComponent<Props> = (props) => {
  const [state] = useState<State>({ accessKey: "" });
  const addGameX = () => addGame({ id: "" as GameId, accessKey: state.accessKey, rounds: [] });
  return <FuncComp {...props} {...state} save={addGameX} />;
};

export default Comp;
