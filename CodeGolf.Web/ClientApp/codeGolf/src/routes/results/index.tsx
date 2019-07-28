import { Component, h, RenderableProps } from "preact";

import { getFinalResults } from "../../api";
import { LoadingState, Result } from "../../types/types";
import FuncComp from "./funcComp";

type State = LoadingState<ReadonlyArray<Result>>;

export default class Comp extends Component<{}, State> {
  constructor() {
    super();
    this.state = { type: "Loading" };
  }
  public async componentDidMount() {
    const results = await getFinalResults();
    this.setState(() => ({ type: "Loaded", data: results }));
  }
  public render = (_: RenderableProps<{}>, res: Readonly<State>) =>
    <FuncComp results={res} />
}
