import { HubConnection } from "@aspnet/signalr";
import { debounce } from "micro-dash";
import { Component, h, RenderableProps } from "preact";

import { getCurrentHole, submitChallenge, tryCompile } from "../../api/playerApi";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { GameId, Hole, ifLoaded, LoadingState, RunResult } from "../../types/types";
import FuncComp from "./funcComp";
import Notification from "./notification";

interface State {
  readonly gameId: GameId | undefined ;
  readonly challenge: LoadingState<Hole | undefined>;
  readonly code: string;
  readonly errors: LoadingState<RunResult | undefined>;
  readonly connection: HubConnection;
}

const compile = async (c: Hole | undefined, code: string) => c ? await tryCompile(c.challengeSet.id, code) : [];

export default class Comp extends Component<{}, State> {
  private readonly tryCompile = debounce(async () => {
    this.setState(s => ({ ...s, errors: { type: "Loading" } }));
    const errors = {
      type: "Loaded",
      data: {
        type: "CompileError",
        errors: await ifLoaded(this.state.challenge, c => compile(c, this.state.code), () => Promise.resolve([]))
      }
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
    this.state = { challenge: { type: "Loading" }, code: "", errors: { type: "Loaded", data: undefined }, connection, gameId: undefined };
  }
  public readonly componentDidMount = async () => {
    const challenge = await getCurrentHole();
    this.setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge } }));
  }
  public readonly componentWillUnmount = () => {
    this.state.connection.stop();
  }
  public readonly render = (_: RenderableProps<{}>, { errors, code, challenge }: Readonly<State>) =>
    <FuncComp code={code} errors={errors} challenge={challenge} codeChanged={this.codeChanged} submitCode={this.submitCode} onCodeClick={this.onCodeClick} />

  private readonly codeChanged = async (code: string) => {
    this.setState(s => ({ ...s, code }));
    this.tryCompile();
  }
  private readonly onCodeClick = () => {
    if (this.state.code === "") {
      ifLoaded(this.state.challenge, c => {
        if (c) {
          const funcDec = getFunctionDeclaration(c.challengeSet);
          this.setState(s => ({ ...s, code: funcDec }));
        }
      }, () => undefined);
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
