
import { FunctionalComponent, h } from "preact";

import Editor from "@monaco-editor/react";

import Loading from "../../components/loading";
import { ifLoaded, LoadingState } from "../../types/types";

interface Props {
    readonly result: LoadingState<string>;
}

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ result }) => ifLoaded(result, data => (
    <section class="section is-fluid">
        <Editor value={data}
            height="40vh"
            language="csharp" />
    </section>), () => <Loading/>);

export default FuncComp;
