import { configure } from "enzyme";
import Adapter from "enzyme-adapter-preact-pure";

configure({
  adapter: new Adapter()
});

// eslint-disable-next-line functional/immutable-data
console.error = (message: string) => {
  throw new Error(message);
};
