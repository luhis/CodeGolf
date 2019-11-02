import { FunctionalComponent, h } from "preact";

import { CompileError, Score } from "../../types/types";
import ErrorsView from "./errors";
import Success from "./success";

interface Props {
    readonly errors: Score | CompileError | undefined;
}

const Comp: FunctionalComponent<Props> = ({ errors }) => {
    if (!errors)
    {
        return null;
    }
    switch (errors.type) {
        case "CompileError":
            return <ErrorsView errors={errors} />;
        case "Score":
            return <Success score={errors} />;
    }
};

export default Comp;
