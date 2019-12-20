import { render } from "enzyme";
import { h } from "preact";
import { newValidDateOrThrow } from "ts-date";

import Attempts from "../components/attempts";
import Challenge from "../components/challenge";
import CookieConsent from "../components/cookieConsent";
import CreateGame from "../components/createGame";
import Footer from "../components/footer";
import Header from "../components/header";
import Icons from "../components/icons";
import Loading from "../components/loading";
import Privacy from "../components/privacy";
import Results from "../components/results";
import SignOut from "../components/signOut";
import { AttemptId, ChallengeSetId } from "../types/types";

describe("Attempts", () => {
  it("renders without crashing", () => {
    render(<Attempts attempts={[]} />);
  });

  it("renders with content without crashing", () => {
    render(<Attempts attempts={[{ rank: 1, id: "avatar" as AttemptId, loginName: "login", avatar: "aaaa", score: 1, timeStamp: newValidDateOrThrow('2000-1-1') }]} />);
  });
});

describe("Challenge", () => {
  it("renders without crashing", () => {
    render(
      <Challenge
        challengeSet={
          {
            id: "id" as ChallengeSetId,
            title: "title",
            description: "description",
            challenges: [],
            returnType: "rtn",
            params: []
          }}
        errors={undefined}
      />);
  });

  it("renders with content without crashing", () => {
    render(<Challenge challengeSet={{
      id: "id" as ChallengeSetId,
      title: "title",
      description: "description",
      challenges: [{ args: ["int", "string"], expectedResult: 1 as unknown as object }],
      returnType: "rtn",
      params: [{ suggestedName: "s", type: "string" }]
    }}
    errors={undefined}
    />);
  });
});

describe("CookieConsent", () => {
  it("renders without crashing", () => {
    render(<CookieConsent />);
  });
});

describe("SignOut", () => {
  it("renders without crashing", () => {
    render(<SignOut />);
  });
});

describe("Loading", () => {
  it("renders without crashing", () => {
    render(<Loading />);
  });
});

describe("Icons", () => {
  it("renders without crashing", () => {
    render(<Icons icon="code" />);
  });
});

describe("Header", () => {
  it("renders without crashing", () => {
    render(<Header />);
  });
});

describe("Footer", () => {
  it("renders without crashing", () => {
    render(<Footer />);
  });
});

describe("CreateGame", () => {
  it("renders without crashing", () => {
    render(<CreateGame challenges={[]} hide={() => undefined} />);
  });
});

describe("Privacy", () => {
  it("renders without crashing", () => {
    render(<Privacy />);
  });
});

describe("Results", () => {
  it("renders without crashing", () => {
    render(<Results runResult={undefined} />);
  });
  it("renders without crashing", () => {
    render(<Results runResult={{ type: "CompileError", errors: [{ col: 1, endCol: 2, line: 1, message: "message" }] }} />);
  });
  it("renders without crashing", () => {
    render(<Results runResult={{ type: "Score", val: 99 }} />);
  });
});
