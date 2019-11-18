import { configure } from "enzyme";
import Adapter from "enzyme-adapter-preact-pure";

configure({
    adapter: new Adapter()
});

console.error = message => {
    throw new Error(message);
};
