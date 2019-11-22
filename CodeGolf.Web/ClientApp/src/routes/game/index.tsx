import { HubConnection } from "@aspnet/signalr";
import debounce from "lodash.debounce";
import { Component, h, RenderableProps } from "preact";

import { getCurrentHole, submitChallenge, tryCompile } from "../../api/playerApi";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { CompileError, GameId, Hole, RunResultSet, Score } from "../../types/types";
import FuncComp from "./funcComp";
import Notification from "./notification";

interface State {
  readonly gameId: GameId | undefined;
  readonly challenge: LoadingState<Hole | undefined>;
  readonly code: string;
  readonly errors: LoadingState<Score | CompileError | undefined>;
  readonly runErrors: RunResultSet | undefined;
  readonly connection: HubConnection;
}

export default class Comp extends Component<{}, State> {
  private readonly tryCompile = debounce(async () => {
    this.setState(s => ({ ...s, errors: { type: "Loading" } }));
    const errors = {
      type: "Loaded",
      data: {
        type: "CompileError",
        errors: await tryCompile(this.state.code)
      }
    } as LoadingState<CompileError>;
    this.setState(s => ({ ...s, errors }));
  }, 1000);

  constructor() {
    super();
    const connection = Notification(async () => {
      this.setState(s => ({ ...s, challenge: { type: "Loading" } }));
      const challenge = await getCurrentHole();
      this.setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge }, errors: { type: "Loaded", data: undefined }, code: "" }));
    });
    this.state = { challenge: { type: "Loading" }, code: "", errors: { type: "Loaded", data: undefined }, runErrors: undefined, connection, gameId: undefined };
  }
  public readonly componentDidMount = async () => {
    const challenge = await getCurrentHole();
    this.setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge } }));
  }
  public readonly componentWillUnmount = async () => {
    await this.state.connection.stop();
  }
  public readonly render = (_: RenderableProps<{}>, { errors, runErrors, challenge }: State) =>
    <FuncComp errors={errors} runErrors={runErrors} challenge={challenge} codeChanged={this.codeChanged} submitCode={this.submitCode} />

  private readonly codeChanged = async (code: string) => {
    this.setState(s => ({ ...s, code }));
    this.tryCompile();
  }
  private readonly submitCode = async (code: string) => {
    ifLoaded(this.state.challenge, async c => {
      if (c) {
        this.setState(s => ({ ...s, code, errors: { type: "Loading" } }));
        const r = await submitChallenge(code, c.hole.holeId);
        if (r.type === "RunResultSet") {
          this.setState(s => ({ ...s, errors: { type: "Loaded", data: undefined }, runErrors: r }));
        }
        else if (r.type === "Score") {
          const passedChallenges = c.challengeSet.challenges.map(_ => ({ error: undefined }));
          this.setState(s => ({ ...s, errors: { type: "Loaded", data: r }, runErrors: { type: "RunResultSet", errors: passedChallenges } }));
        }
        else {
          this.setState(s => ({ ...s, errors: { type: "Loaded", data: r } }));
        }
      }
    }, () => undefined);
  }
}
