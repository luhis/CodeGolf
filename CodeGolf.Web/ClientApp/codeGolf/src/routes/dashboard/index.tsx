import { HubConnectionBuilder } from "@aspnet/signalr";
import { Component, h, RenderableProps } from "preact";
import { route } from "preact-router";

import { endHole, getCurrentChallenge, getResults, nextHole } from "../../api";
import { Attempt, Hole, LoadingState } from "../../types/types";
import FuncComp from "./funcComp";

interface State { readonly currentHole: LoadingState<Hole | undefined>; readonly attempts: LoadingState<ReadonlyArray<Attempt>>; }

export default class Comp extends Component<{}, State> {
  constructor() {
    super();
    const connection = new HubConnectionBuilder().withUrl("/refreshHub").build();
    connection.on("newAnswer", this.getResults);
    connection.start().catch(err => console.error(err.toString()));
    this.state = { currentHole: { type: "Loading" }, attempts: { type: "Loaded", data: [] } };
  }
  public readonly componentDidMount = async () => {
    await this.getHole();
    await this.getResults();
  }
  public readonly render = (_: RenderableProps<{}>, { currentHole, attempts }: Readonly<State>) => {
    if (currentHole.type === "Loaded") {
      const f = currentHole.data && currentHole.data.hasNext ? endHole : async () => { await endHole(); route("/results"); };
      return (<FuncComp
        current={currentHole}
        attempts={attempts}
        nextHole={this.doThenUpdateHole(nextHole)}
        endHole={this.doThenUpdateHole(f)} />);
    }
    return null;
  }

  private readonly getResults = async () => {
    if (this.state.currentHole.type === "Loaded" && this.state.currentHole.data) {
      const results = await getResults(this.state.currentHole.data.hole.holeId);
      this.setState(s => ({ ...s, attempts: { type: "Loaded", data: results } }));
    }
  }

  private readonly getHole = async () => {
    try {
      const hole = await getCurrentChallenge();
      this.setState(s => ({ ...s, currentHole: { type: "Loaded", data: hole } }));
    }
    catch {
      this.setState(s => ({ ...s, currentHole: { type: "Loaded", data: undefined } }));
    }
  }

  private readonly doThenUpdateHole = (f: () => Promise<any>) => async () => {
    await f();
    await this.getHole();
    await this.getResults();
  }
}
