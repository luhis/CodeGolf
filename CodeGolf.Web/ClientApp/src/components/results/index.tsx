import { FunctionalComponent, h } from "preact";

import { CompileError, Score } from "../../types/types";
import ErrorsView from "./errors";
import Success from "./success";

interface Props {
  readonly runResult: Score | CompileError | undefined;
}

const Comp: FunctionalComponent<Props> = ({ runResult }) => {
  if (!runResult) {
    return null;
  }
  switch (runResult.type) {
    case "CompileError":
      return <ErrorsView errors={runResult} />;
    case "Score":
      return <Success score={runResult} />;
  }
};

export default Comp;
