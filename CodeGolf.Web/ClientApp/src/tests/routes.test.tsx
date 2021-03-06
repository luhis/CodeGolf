import { render, shallow } from "enzyme";
import { h } from "preact";
//import * as Hooks from "preact/hooks";
import { newValidDateOrThrow } from "ts-date";

import AdminContainer from "../routes/admin";
import Admin from "../routes/admin/funcComp";
import AttemptContainer from "../routes/attempt";
import Attempt from "../routes/attempt/funcComp";
import CodeFileContainer from "../routes/codefile";
import CodeFile from "../routes/codefile/funcComp";
//import DashboardIndex from "../routes/dashboard";
import Dashboard from "../routes/dashboard/funcComp";
import Demo from "../routes/demo/funcComp";
import GameComp from "../routes/game/funcComp";
//import GameContainer from "../routes/game";
import Home from "../routes/home";
import ResultsContainer from "../routes/results";
import Results from "../routes/results/funcComp";
import { Game, GameId, Round, RoundId, ChallengeSetId, Hole, HoleId, AttemptId } from "../types/types";
import { LoadingState } from "../types/appTypes";

const games = [{ id: "" as GameId, accessKey: "", rounds: [{ id: "id" as RoundId, name: "name" }] }] as ReadonlyArray<Game>;
const challenges = [{ id: "" as RoundId, name: "" }] as ReadonlyArray<Round>;

describe("Admin", () => {
  it("renders Admin without crashing", () => {
    render(<Admin myGames={{ type: "Loading" }} allChallenges={{ type: "Loading" }} showCreate={false} toggleCreate={((_: boolean) => undefined)} resetGame={(() => undefined)} />);
  });

  it("renders Admin with content without crashing", () => {
    render(<Admin myGames={{ type: "Loaded", data: games }} allChallenges={{ type: "Loaded", data: challenges }} showCreate={false} toggleCreate={((_: boolean) => undefined)} resetGame={(() => undefined)} />);
  });
  it("renders Admin Index without crashing", async () => {
    render(<AdminContainer />);
  });
});

it("renders Home without crashing", () => {
  render(<Home />);
});

describe("Attempt", () => {
  it("renders Attempt without crashing", () => {
    render(<Attempt result={{ type: "Loading" }} />);
  });

  it("renders Attempt Container without crashing", () => {
    render(<AttemptContainer attemptId={"" as AttemptId}/>);
  });

  // const attemptWithCode = {avatar: "ava.png", code:"return hello world", loginName: "", score:1, timeStamp: new Date()};
  // it("renders Attempt with data without crashing", () => {
  //   shallow(<Attempt result={{type: "Loaded", data: attemptWithCode}} />);
  // });
});
describe("CodeFile should", () => {
  it("renders CodeFile without crashing", () => {
    render(<CodeFile result={{ type: "Loading" }} />);
  });
  it("renders CodeFileContainer without crashing", () => {
    render(<CodeFileContainer code="a" type="debug" />);
  });
});

describe("Dashboard Should should", () => {
  it("renders Dashboard without crashing", () => {
    render(<Dashboard current={{ type: "Loading" }} attempts={{ type: "Loading" }} nextHole={() => Promise.resolve()} endHole={() => Promise.resolve()} />);
  });
  // it("renders Dashboard without crashing", () => {
  //   render(<DashboardIndex />);
  // });
});
describe("Demo should", () => {
  it("renders Demo without crashing", () => {
    shallow(<Demo
      code="aaa"
      runResult={{ type: "Loading" }}
      runErrors={undefined}
      challenge={{ type: "Loading" }}
      codeChanged={_ => Promise.resolve()}
      onCodeClick={() => undefined}
      submitCode={(_, __) => undefined} />);
  });

  it("renders Demo with errors without crashing", () => {
    shallow(<Demo
      code="aaa"
      runResult={{ type: "Loaded", data: { type: "CompileError", errors: [{ line: 1, col: 2, endCol: 3, message: "error" }] } }}
      runErrors={undefined}
      challenge={{ type: "Loading" }}
      codeChanged={_ => Promise.resolve()}
      onCodeClick={() => undefined}
      submitCode={(_, __) => undefined} />);
  });

  it("renders Demo with score without crashing", () => {
    shallow(<Demo
      code="aaa"
      runResult={{ type: "Loaded", data: { type: "Score", val: 30 } }}
      runErrors={undefined}
      challenge={{ type: "Loading" }}
      codeChanged={_ => Promise.resolve()}
      onCodeClick={() => undefined}
      submitCode={(_, __) => undefined} />);
  });

  it("renders Demo without crashing", () => {
    shallow(<Demo
      code="aaa"
      runResult={{ type: "Loading" }}
      runErrors={{ type: "RunResultSet", errors: [{ error: { message: "aaa", found: "" } }] }}
      challenge={{ type: "Loading" }}
      codeChanged={_ => Promise.resolve()}
      onCodeClick={() => undefined}
      submitCode={(_, __) => undefined} />);
  });
});
describe("Game should", () => {
  it("renders GameComp without crashing", () => {
    shallow(<GameComp
      code="aaa"
      runResult={{ type: "Loading" }}
      runErrors={{ type: "RunResultSet", errors: [] }}
      challenge={{ type: "Loading" }} codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={(_) => undefined} />);
  });

  // it("renders GameContainer without crashing", () => {
  //   shallow(<GameContainer />);
  // });

  it("renders GameComp with errors without crashing", () => {
    shallow(<GameComp
      code="aaa"
      runResult={{ type: "Loaded", data: { type: "CompileError", errors: [{ line: 1, col: 2, endCol: 3, message: "error" }] } }}
      runErrors={{ type: "RunResultSet", errors: [] }}
      challenge={{ type: "Loading" }} codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={(_) => undefined} />);
  });

  it("renders Game with score without crashing", () => {
    shallow(<GameComp
      code="aaa"
      runResult={{ type: "Loaded", data: { type: "Score", val: 30 } }}
      runErrors={undefined}
      challenge={{ type: "Loading" }}
      codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={(_) => undefined} />);
  });

  it("renders GameComp with n challenge without crashing", () => {
    shallow(<GameComp
      code="aaa"
      runResult={{ type: "Loading" }}
      runErrors={{ type: "RunResultSet", errors: [] }}
      challenge={{ type: "Loaded", data: undefined }}
      codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={(_) => undefined} />);
  });
  it("renders GameComp with challenge without crashing", () => {

    const x: LoadingState<Hole> = {
      type: "Loaded", data: {
        challengeSet: {
          id: "id" as ChallengeSetId,
          title: "title",
          description: "description",
          challenges: [],
          returnType: "rtn",
          params: []
        }, start: newValidDateOrThrow("2000-1-1"), end: newValidDateOrThrow("2000-1-1"), closedAt: undefined, hasNext: true, hole: { holeId: "aaa" as HoleId }
      }
    };
    shallow(<GameComp
      code="aaa"
      runResult={{ type: "Loading" }}
      runErrors={{ type: "RunResultSet", errors: [] }}
      challenge={x}
      codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={(_) => undefined} />);
  });
});

describe("Demo should", () => {
  it("renders ResultsContainer without crashing", () => {
    render(<ResultsContainer />);
  });

  it("renders Results without crashing", () => {
    render(<Results results={{ type: "Loading" }} />);
  });

  it("renders Results with items without crashing", () => {
    render(<Results results={{
      type: "Loaded", data: [{
        score: 10,
        rank: 1,
        loginName: "matt",
        avatarUri: "avatar.png"
      }]
    }} />);
  });
});
