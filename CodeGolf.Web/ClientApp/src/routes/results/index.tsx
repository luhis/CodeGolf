import { Component, h, RenderableProps } from "preact";

import { getFinalResults } from "../../api/adminApi";
import { LoadingState, Result } from "../../types/types";
import FuncComp from "./funcComp";

type State = LoadingState<ReadonlyArray<Result>>;

export default class Comp extends Component<{}, State> {
  constructor() {
    super();
    this.state = { type: "Loading" };
  }
  public readonly componentDidMount = async () => {
    const results = await getFinalResults();
    this.setState(() => ({ type: "Loaded", data: results }));
  }
  public readonly render = (_: RenderableProps<{}>, res: State) =>
    <FuncComp results={res} />
}
