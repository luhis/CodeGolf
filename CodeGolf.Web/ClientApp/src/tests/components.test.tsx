import { render } from "enzyme";
import { h } from "preact";

import Attempts from "../components/attempts";
import { AttemptId } from "../types/types";

it("renders Attempts without crashing", () => {
    render(<Attempts attempts={[]}/>);
});

it("renders Attempts with content without crashing", () => {
    render(<Attempts attempts={[{rank: 1, id: "avatar" as AttemptId, loginName: "login", avatar: "aaaa", score: 1, timeStamp: new Date(1) }]}/>);
});
