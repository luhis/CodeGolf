import { HubConnection } from "@aspnet/signalr";
import { debounce } from "lodash";
import { Component, h, RenderableProps } from "preact";

import { getCurrentHole, submitChallenge, tryCompile } from "../../api";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { Hole, ifLoaded, LoadingState, RunResult } from "../../types/types";
import FuncComp from "./funcComp";
import Notification from "./notification";

interface State {
  readonly challenge: LoadingState<Hole | undefined>;
  readonly code: string;
  readonly errors: LoadingState<RunResult | undefined>;
  readonly connection: HubConnection;
}

export default class Comp extends Component<{}, State> {
  private readonly tryCompile = debounce(async () => {
    this.setState(s => ({ ...s, errors: { type: "Loading" } }));
    const errors = {
      type: "Loaded",
      data: { type: "CompileError", errors: await tryCompile(this.state.challenge.type === "Loaded" && this.state.challenge.data ? this.state.challenge.data.challengeSet.id : "", this.state.code) } as RunResult
    } as LoadingState<RunResult>;
    this.setState(s => ({ ...s, errors }));
  }, 1000);

  constructor() {
    super();
    const connection = Notification(async () => {
      this.setState(s => ({ ...s, challenge: { type: "Loading" } }));
      const challenge = await getCurrentHole();
      this.setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge }, errors: { type: "Loaded", data: undefined }, code: "" }));
    });
    this.state = { challenge: { type: "Loading" }, code: "", errors: { type: "Loaded", data: undefined }, connection };
  }
  public readonly componentDidMount = async () => {
    const challenge = await getCurrentHole();
    this.setState((s => ({ ...s, challenge: { type: "Loaded", data: challenge } })));
  }
  public readonly componentWillUnmount = () => {
    this.state.connection.stop();
  }
  public readonly render = (_: RenderableProps<{}>, { errors, code, challenge }: Readonly<State>) =>
    <FuncComp code={code} errors={errors} challenge={challenge} codeChanged={this.codeChanged} submitCode={this.submitCode} onCodeClick={this.onCodeClick} />

  public readonly codeChanged = async (code: string) => {
    this.setState(s => ({ ...s, code }));
    this.tryCompile();
  }
  private readonly onCodeClick = () => {
    if (this.state.code === "" && this.state.challenge.type === "Loaded" && this.state.challenge.data) {
      const funcDec = getFunctionDeclaration(this.state.challenge.data.challengeSet);
      this.setState(s => ({ ...s, code: funcDec }));
    }
  }
  private readonly submitCode = async (code: string) => {
    ifLoaded(this.state.challenge, async c => {
      if (c) {
        this.setState(s => ({ ...s, code, errors: { type: "Loading" } }));
        const res = { type: "Loaded", data: await submitChallenge(code, c.hole.holeId) } as LoadingState<RunResult>;
        this.setState(_ => ({ errors: res }));
      }
    }, () => undefined);
  }
}
