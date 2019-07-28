import { Component, FunctionalComponent, h, RenderableProps } from "preact";
import { isAdmin, isLoggedIn } from "../../api";

const FuncComp: FunctionalComponent<State> = ({ admin, loggedIn }) => (<div class="navbar-menu">
    <div class="navbar-start">
        <a class="navbar-item" href="/">Home</a>
        <a class="navbar-item" href="/demo">Demo</a>
        {loggedIn ? <a class="navbar-item" href="/game">Game</a> : null}
        {loggedIn ? <a class="navbar-item" href="/SignOut">Sign Out</a> : null}
        {!loggedIn ? <a href="/account/signin" class="navbar-item">Sign In with GitHub</a> : null}

        {admin ? <a class="navbar-item" href="/dashboard">Dashboard</a> : null}
        {admin ? <a class="navbar-item" href="/admin">Admin</a> : null}
        <a class="navbar-item" target="_blank" href="https://codegolf.stackexchange.com/questions/173/tips-for-code-golfing-in-c">Tips</a>
    </div>
</div>);

interface State { admin: boolean; loggedIn: boolean; }

export default class Comp extends Component<{}, State> {
    constructor() {
        super();
        this.state = { loggedIn: false, admin: false };
    }
    public async componentDidMount() {
        const admin = await isAdmin();
        const loggedIn = await isLoggedIn();
        this.setState({ admin, loggedIn });
      }
    public render = (_: RenderableProps<{}>, s: Readonly<State>) =>
        <FuncComp {...s} />
}
