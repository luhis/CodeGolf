import { render, shallow } from "enzyme";
import { h } from "preact";

import Admin from "../routes/admin/funcComp";
import Attempt from "../routes/attempt/funcComp";
import CodeFile from "../routes/codefile/funcComp";
import Dashboard from "../routes/dashboard/funcComp";
import Demo from "../routes/demo/funcComp";
import Game from "../routes/game/funcComp";
import Home from "../routes/home";
import Results from "../routes/results/funcComp";

it("renders Admin without crashing", () => {
  render(<Admin myGames={{type: "Loading"}} allChallenges={{type: "Loading"}} showCreate={false} toggleCreate={((_: boolean) => undefined)} resetGame={(() => undefined)} />);
});

it("renders Home without crashing", () => {
  render(<Home />);
});

it("renders Attempt without crashing", () => {
  render(<Attempt result={{type: "Loading"}} />);
});

it("renders CodeFile without crashing", () => {
  render(<CodeFile result={{type: "Loading"}} />);
});

it("renders Dashboard without crashing", () => {
  render(<Dashboard current={{type: "Loading"}} attempts={{type: "Loading"}} nextHole={() => Promise.resolve()} endHole={() => Promise.resolve()} />);
});

it("renders Demo without crashing", () => {
  shallow(<Demo
    code="aaa"
    errors={{type: "Loading"}} challenge={{type: "Loading"}} codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={ (_, __) => undefined} />);
});

it("renders Game without crashing", () => {
  shallow(<Game
    code="aaa"
    errors={{type: "Loading"}} challenge={{type: "Loading"}} codeChanged={_ => Promise.resolve()} onCodeClick={() => undefined} submitCode={ (_) => undefined} />);
});

it("renders Results without crashing", () => {
  render(<Results results={{type: "Loading"}} />);
});
