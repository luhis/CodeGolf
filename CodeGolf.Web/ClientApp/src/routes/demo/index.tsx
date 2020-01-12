import debounce from "lodash.debounce";
import { Component, h, RenderableProps } from "preact";

import { getDemoChallenge, submitDemo, tryCompile } from "../../api/playerApi";
import { getFunctionDeclaration } from "../../funcDeclaration";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { ChallengeSet, CompileError, RunResultSet, Score } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
  readonly challenge: LoadingState<ChallengeSet>;
  readonly code: string;
  readonly runErrors: RunResultSet | undefined;
  readonly runResult: LoadingState<Score | CompileError | undefined>;
}

export default class Comp extends Component<{}, State> {
  private readonly tryCompile = debounce(async () => {
    const runResult = {
      type: "Loaded",
      data: {
        type: "CompileError",
        errors: await tryCompile(this.state.code)
      }
    } as LoadingState<CompileError>;
    this.setState(s => ({ ...s, runResult }));
  }, 1000);

  private readonly codeChanged = debounce(async (code: string) => {
    this.setState(s => ({ ...s, code, runResult: { type: "Loading" } }));
    await this.tryCompile();
  }, 250);

  constructor() {
    super();
    this.state = { challenge: { type: "Loading" }, code: "", runResult: { type: "Loaded", data: undefined }, runErrors: undefined };
  }
  public readonly componentDidMount = async () => {
    const challenge = await getDemoChallenge();
    this.setState(s => ({ ...s, challenge: { type: "Loaded", data: challenge } }));
  }
  public readonly render = (_: RenderableProps<{}>, { runResult, runErrors, code, challenge }: State) =>
    <FuncComp code={code} runErrors={runErrors} runResult={runResult} challenge={challenge} codeChanged={this.codeChanged} submitCode={this.submitCode} onCodeClick={this.onCodeClick} />
  private readonly onCodeClick = () => {
    if (this.state.code === "") {
      ifLoaded(this.state.challenge, c => {
        const funcDec = getFunctionDeclaration(c);
        this.setState(s => ({ ...s, code: funcDec }));
      }, () => undefined);
    }
  }
  private readonly submitCode = async (code: string, reCaptcha: string) => {
    this.setState(s => ({ ...s, runResult: { type: "Loading" } }));
    const r = await submitDemo(code, reCaptcha);
    if (r.type === "RunResultSet") {
      this.setState(s => ({ ...s, runResult: { type: "Loaded", data: undefined }, runErrors: r }));
    }
    else if (r.type === "Score") {
      const passedChallenges = ifLoaded(this.state.challenge, c => c.challenges.map(_ => ({ error: undefined })), () => []);
      this.setState(s => ({ ...s, runResult: { type: "Loaded", data: r }, runErrors: { type: "RunResultSet", errors: passedChallenges } }));
    }
    else {
      this.setState(s => ({ ...s, runResult: { type: "Loaded", data: r } }));
    }
  }
}
