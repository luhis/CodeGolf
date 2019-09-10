import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";
import { Component, h, RenderableProps } from "preact";
import { route } from "preact-router";

import { endHole, getCurrentChallenge, getResults, nextHole } from "../../api/adminApi";
import { Attempt, Hole, ifLoaded, LoadingState } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
  readonly currentHole: LoadingState<Hole | undefined>;
  readonly attempts: LoadingState<ReadonlyArray<Attempt>>;
  readonly connection: HubConnection;
}

export default class Comp extends Component<{}, State> {
  constructor() {
    super();
    const connection = new HubConnectionBuilder().withUrl("/refreshHub").build();
    connection.on("newAnswer", this.getResults);
    this.state = { currentHole: { type: "Loading" }, attempts: { type: "Loaded", data: [] }, connection };
  }
  public readonly componentDidMount = async () => {
    await this.state.connection.start().catch(err => console.error(err.toString()));
    await this.getHole();
    await this.getResults();
  }
  public readonly componentWillUnmount = () => {
    return this.state.connection.stop();
  }
  public readonly render = (_: RenderableProps<{}>, { currentHole, attempts }: Readonly<State>) =>
    ifLoaded(currentHole, x => {
      const f = x ? endHole : async () => { await endHole(); route("/results"); };
      return (<FuncComp
        current={currentHole}
        attempts={attempts}
        nextHole={this.doThenUpdateHole(nextHole)}
        endHole={this.doThenUpdateHole(f)} />);
    },
      () => null)

  private readonly getResults = async () => {
    return ifLoaded(this.state.currentHole, async hole => {
      if (hole) {
        const results = await getResults(hole.hole.holeId);
        this.setState(s => ({ ...s, attempts: { type: "Loaded", data: results } }));
      }

    }, () => Promise.resolve());
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

  private readonly doThenUpdateHole = (f: () => Promise<unknown>) => async () => {
    await f();
    await this.getHole();
    await this.getResults();
  }
}
