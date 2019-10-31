import { debounce } from "micro-dash";
import { Component, h, RenderableProps } from "preact";

import { getDemoChallenge, submitDemo, tryCompile } from "../../api/playerApi";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { ChallengeSet, ifLoaded, LoadingState, RunResult } from "../../types/types";
import FuncComp from "./funcComp";

interface State { readonly challenge: LoadingState<ChallengeSet>; readonly code: string; readonly errors: LoadingState<RunResult | undefined>; }

const compile = async (c: ChallengeSet | undefined, code: string) => c ? await tryCompile(c.id, code) : [];

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
    this.state = { challenge: { type: "Loading" }, code: "", errors: { type: "Loaded", data: undefined } };
  }
  public readonly componentDidMount = async () => {
    const challenge = await getDemoChallenge();
    this.setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge } }));
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
        const funcDec = getFunctionDeclaration(c);
        this.setState(s => ({ ...s, code: funcDec }));
      }, () => undefined);
    }
  }
  private readonly submitCode = async (code: string, reCaptcha: string) => {
    this.setState(s => ({ ...s, errors: { type: "Loading" } }));
    const res = { type: "Loaded", data: await submitDemo(code, reCaptcha) } as LoadingState<RunResult>;
    this.setState(s => ({ ...s, errors: res }));
  }
}
