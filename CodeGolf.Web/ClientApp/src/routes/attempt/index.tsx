import { Component, h, RenderableProps } from "preact";

import { getAttempt } from "../../api/adminApi";
import { AttemptId, AttemptWithCode, LoadingState } from "../../types/types";
import FuncComp from "./funcComp";

type State = LoadingState<AttemptWithCode>;

interface Props { readonly attemptId: AttemptId; }

export default class Comp extends Component<Props, State> {
  constructor() {
    super();
    this.state = { type: "Loading" };
  }
  public readonly componentDidMount = async () => {
    const results = await getAttempt(this.props.attemptId);
    this.setState(() => ({ type: "Loaded", data: results }));
  }
  public readonly render = (_: RenderableProps<Props>, res: Readonly<State>) =>
    <FuncComp result={res} />
}
