import { Component, h, RenderableProps } from "preact";

import { getCsFile } from "../../api/playerApi";
import { LoadingState } from "../../types/appTypes";
import FuncComp from "./funcComp";

type State = LoadingState<string>;

interface Props { readonly type: ("debug" | "preview"); readonly code: string; }

export default class Comp extends Component<Props, State> {
    constructor() {
        super();
        this.state = { type: "Loading" };
    }
    public readonly componentDidMount = async () => {
        const results = await getCsFile(this.props.type, this.props.code);
        this.setState(() => ({ type: "Loaded", data: results }));
    }
    public readonly render = (_: RenderableProps<Props>, res: Readonly<State>) =>
        <FuncComp result={res} />
}
