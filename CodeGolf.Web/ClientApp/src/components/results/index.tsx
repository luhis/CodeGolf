import { FunctionalComponent, h } from "preact";

import { RunResult } from "../../types/types";
import ErrorsView from "./errors";
import Success from "./success";

interface Props {
    readonly errors?: RunResult;
    readonly returnType: string;
}

const Comp: FunctionalComponent<Props> = ({ errors, returnType }) => {
    if (!errors)
    {
        return null;
    }
    switch (errors.type) {
        case "RunError":
        case "CompileError":
            return <ErrorsView errors={errors} returnType={returnType} />;
        case "Score":
            return <Success score={errors} />;
    }
};

export default Comp;
