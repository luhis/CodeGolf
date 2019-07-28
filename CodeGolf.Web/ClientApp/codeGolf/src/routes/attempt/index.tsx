import { Component, h, RenderableProps } from "preact";

import { getAttempt } from "../../api";
import { AttemptWithCode, Guid, LoadingState } from "../../types/types";
import FuncComp from "./funcComp";

type State = LoadingState<AttemptWithCode>;

export default class Comp extends Component<{attemptId: Guid}, State> {
  constructor() {
    super();
    this.state = { type: "Loading" };
  }
  public async componentDidMount() {
    const results = await getAttempt(this.props.attemptId);
    this.setState(() => ({ type: "Loaded", data: results }));
  }
  public render = (_: RenderableProps<{attemptId: Guid}>, res: Readonly<State>) =>
    <FuncComp result={res} />
}
