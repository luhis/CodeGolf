import { h } from "preact";
import render from "preact-render-to-string";
import Header from "../components/header/index";

test("Link changes the class when hovered", () => {
    const component = render(
        <Header />,
    );
    expect(component).toMatchSnapshot();
});
