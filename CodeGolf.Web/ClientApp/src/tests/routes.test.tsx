import { render, shallow } from "enzyme";
import { h } from "preact";

import Admin from "../routes/admin/funcComp";
import Attempt from "../routes/attempt/funcComp";
import CodeFile from "../routes/codefile/funcComp";
import Dashboard from "../routes/dashboard/funcComp";
import Demo from "../routes/demo/funcComp";
import GameComp from "../routes/game/funcComp";
import Home from "../routes/home";
import Results from "../routes/results/funcComp";
import { Game, GameId, Round, RoundId } from "../types/types";

const games = [{ id: "" as GameId, accessKey: "", rounds: [{ id: "id" as RoundId, name: "name" }] }] as ReadonlyArray<Game>;
const challenges = [{ id: "" as RoundId, name: "" }] as ReadonlyArray<Round>;

describe("Admin", () => {
  it("renders Admin without crashing", () => {
    render(<Admin myGames={{ type: "Loading" }} allChallenges={{ type: "Loading" }} showCreate={false} toggleCreate={((_: boolean) => undefined)} resetGame={(() => undefined)} />);
  });

  it("renders Admin with content without crashing", () => {
    render(<Admin myGames={{ type: "Loaded", data: games }} allChallenges={{ type: "Loaded", data: challenges }} showCreate={false} toggleCreate={((_: boolean) => undefined)} resetGame={(() => undefined)} />);
  });
});

it("renders Home without crashing", () => {
  render(<Home />);
});

describe("Attempt", () => {
  it("renders Attempt without crashing", () => {
    render(<Attempt result={{ type: "Loading" }} />);
  });

  // const attemptWithCode = {avatar: "ava.png", code:"return hello world", loginName: "", score:1, timeStamp: new Date()};
  // it("renders Attempt with data without crashing", () => {
  //   shallow(<Attempt result={{type: "Loaded", data: attemptWithCode}} />);
  // });
});

it("renders CodeFile without crashing", () => {
  render(<CodeFile result={{ type: "Loading" }} />);
});

it("renders Dashboard without crashing", () => {
  render(<Dashboard current={{ type: "Loading" }} attempts={{ type: "Loading" }} nextHole={() => Promise.resolve()} endHole={() => Promise.resolve()} />);
});

it("renders Demo without crashing", () => {
  shallow(<Demo
    code="aaa"
    errors={{ type: "Loading" }}
    runErrors={undefined}
    challenge={{ type: "Loading" }} codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={(_, __) => undefined} />);
});

it("renders GameComp without crashing", () => {
  shallow(<GameComp
    code="aaa"
    errors={{ type: "Loading" }}
    runErrors={{type: "RunResultSet", errors: []}}
    challenge={{ type: "Loading" }} codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={(_) => undefined} />);
});

it("renders Results without crashing", () => {
  render(<Results results={{ type: "Loading" }} />);
});
