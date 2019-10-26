import { render } from "enzyme";
import { h } from "preact";

import Attempts from "../components/attempts";

it("renders Attempts without crashing", () => {
    render(<Attempts attempts={[]}/>);
});

it("renders Attempts with content without crashing", () => {
    render(<Attempts attempts={[{rank: 1, id: "avatar", loginName: "login", avatar: "aaaa", score: 1, timeStamp: new Date(1) }]}/>);
});
