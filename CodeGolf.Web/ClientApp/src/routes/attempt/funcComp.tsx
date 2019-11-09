import { ControlledEditor } from "@monaco-editor/react";
import { FunctionalComponent, h } from "preact";

import Loading from "../../components/loading";
import { ifLoaded, LoadingState } from "../../types/appTypes";
import { AttemptWithCode } from "../../types/types";

interface Props {
    readonly result: LoadingState<AttemptWithCode>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ result }) => ifLoaded(result, data => (
    <section class="section is-fluid">
        <h1 class="title">Attempt</h1>
        <div class="columns">
            <figure class="image is-48x48">
                <img src={data.avatar} />
            </figure>
            <span>
                {data.loginName} {data.score} {data.timeStamp}
            </span>
        </div>
        <ControlledEditor
            value={data.code}
            height="60vh"
            language="csharp"
            options={{ minimap: { enabled: false }, readOnly: true }}
        />
    </section>), () => <Loading />);

export default FuncComp;
