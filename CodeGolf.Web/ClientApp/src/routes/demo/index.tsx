import { debounce } from "micro-dash";
import { Component, h, RenderableProps } from "preact";

import { getDemoChallenge, submitDemo, tryCompile } from "../../api/playerApi";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { ChallengeSet, CompileError, ifLoaded, LoadingState, RunResultSet, Score } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
  readonly challenge: LoadingState<ChallengeSet>;
  readonly code: string;
  readonly runErrors: RunResultSet | undefined;
  readonly errors: LoadingState<Score | CompileError | undefined>;
}

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
    } as LoadingState<CompileError>;
    this.setState(s => ({ ...s, errors }));
  }, 1000);
  constructor() {
    super();
    this.state = { challenge: { type: "Loading" }, code: "", errors: { type: "Loaded", data: undefined }, runErrors: undefined };
  }
  public readonly componentDidMount = async () => {
    const challenge = await getDemoChallenge();
    this.setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge } }));
  }
  public readonly render = (_: RenderableProps<{}>, { errors, runErrors, code, challenge }: State) =>
    <FuncComp code={code} runErrors={runErrors} errors={errors} challenge={challenge} codeChanged={this.codeChanged} submitCode={this.submitCode} onCodeClick={this.onCodeClick} />

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
    const r = await submitDemo(code, reCaptcha);
    if (r.type === "RunResultSet") {
      this.setState(s => ({ ...s, errors: { type: "Loaded", data: undefined }, runErrors: r }));
    }
    else if (r.type === "Score") {
      const passedChallenges = ifLoaded(this.state.challenge, c => c.challenges.map(_ => ({error: undefined})), () => []);
      this.setState(s => ({ ...s, errors: { type: "Loaded", data: r }, runErrors: { type: "RunResultSet", errors: passedChallenges } }));
    }
    else {
      this.setState(s => ({ ...s, errors: { type: "Loaded", data: r } }));
    }
  }
}
