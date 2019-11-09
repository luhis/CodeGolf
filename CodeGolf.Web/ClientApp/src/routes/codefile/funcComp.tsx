
import { ControlledEditor } from "@monaco-editor/react";
import { FunctionalComponent, h } from "preact";

import Loading from "../../components/loading";
import { ifLoaded, LoadingState } from "../../types/appTypes";

interface Props {
    readonly result: LoadingState<string>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ result }) => ifLoaded(result, data => (
    <section class="section is-fluid">
        <ControlledEditor value={data}
            height="80vh"
            language="csharp"
            options={{ minimap: { enabled: false }, readOnly: true }}
        />
    </section>), () => <Loading />);

export default FuncComp;
