import { FunctionComponent, h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { getCsFile } from "../../api/playerApi";
import { LoadingState } from "../../types/appTypes";
import FuncComp from "./funcComp";

type State = LoadingState<string>;

interface Props { readonly type: ("debug" | "preview"); readonly code: string; }

const Comp: FunctionComponent<Props> = ({ type, code }) => {
    const [state, setState] = useState<State>({ type: "Loading" });
    useEffect(() => {
        const a = async () => {
            const results = await getCsFile(type, code);
            setState(() => ({ type: "Loaded", data: results }));
        };
        // tslint:disable-next-line: no-floating-promises
        a();
    });
    return <FuncComp result={state} />;
};

export default Comp;
