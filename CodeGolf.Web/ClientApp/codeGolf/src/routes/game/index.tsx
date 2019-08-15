import { HubConnectionBuilder } from "@aspnet/signalr";
import { toast } from "bulma-toast";
import { debounce } from "lodash";
import { Component, h, RenderableProps } from "preact";

import { getCurrentHole, submitChallenge, tryCompile } from "../../api";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { Hole, LoadingState, RunResult } from "../../types/types";
import FuncComp from "./funcComp";

interface State { readonly challenge: LoadingState<Hole | undefined>; readonly code: string; readonly errors: LoadingState<RunResult | undefined>; }

const template = (name: string, score: number, avatarUri: string) =>
  `<div>
        <p>New Top Score!</p>
        <figure class="image container is-48x48"><img src="${avatarUri}"><img/></figure>
        <p>${name}, ${score} strokes</p>
    </div>`;

export default class Comp extends Component<{}, State> {
  private tryCompile = debounce(async () => {
    const errors = {
      type: "Loaded",
      data: { type: "CompileError", errors: await tryCompile(this.state.challenge.type === "Loaded" && this.state.challenge.data ? this.state.challenge.data.challengeSet.id : "", this.state.code) } as RunResult
    } as LoadingState<RunResult>;
    this.setState({ ...this.state, errors });
  }, 1000);
  constructor() {
    super();
    const connection = new HubConnectionBuilder().withUrl("/refreshHub").build();
    connection.on("newTopScore", (name: string, score: number, avatarUri: string) => {
      toast({
        message: template(name, score, avatarUri),
        type: "is-info",
        dismissible: true,
        pauseOnHover: true,
        duration: 2000,
      });
    });
    connection.on("newRound", async () => {
      this.setState({ ...this.state, challenge: { type: "Loading" } });
      const challenge = await getCurrentHole();
      this.setState({ ...this.state, challenge: { type: "Loaded", data: challenge }, errors: {type: "Loaded", data: undefined} });
    });
    connection.start().catch(err => console.error(err.toString()));
    this.state = { challenge: { type: "Loading" }, code: "", errors: { type: "Loaded", data: undefined } };
  }
  public async componentDidMount() {
    const challenge = await getCurrentHole();
    this.setState({ ...this.state, challenge: { type: "Loaded", data: challenge } });
  }
  public render = (_: RenderableProps<{}>, { errors, code, challenge }: Readonly<State>) =>
    <FuncComp code={code} errors={errors} challenge={challenge} codeChanged={this.codeChanged} submitCode={this.submitCode} onCodeClick={this.onCodeClick} />

  public codeChanged = async (code: string) => {
    this.setState({ ...this.state, code, errors: {type: "Loading"} });
    this.tryCompile();
  }
  private onCodeClick = () => {
    if (this.state.code === "" && this.state.challenge.type === "Loaded" && this.state.challenge.data) {
      this.setState({ code: getFunctionDeclaration(this.state.challenge.data.challengeSet) });
    }
  }
  private submitCode = async (code: string) => {
    if (this.state.challenge.type === "Loaded" && this.state.challenge.data) {
      this.setState({ ...this.state, code, errors: {type: "Loading"} });
      const res = { type: "Loaded", data: await submitChallenge(code, this.state.challenge.data.hole.holeId) } as LoadingState<RunResult>;
      this.setState({ errors: res });
    }
  }
}
