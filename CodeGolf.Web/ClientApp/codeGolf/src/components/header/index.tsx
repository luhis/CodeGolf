import { Component, FunctionalComponent, h, RenderableProps } from "preact";
import { isAdmin, isLoggedIn } from "../../api";

const FuncComp: FunctionalComponent<State & { readonly toggleMenu: (() => void) }> = ({ admin, loggedIn, toggleMenu, showMenu }) =>
    (<nav class="navbar" role="navigation" aria-label="main navigation">
        <div class="navbar-brand">
            <a role="button" class={"navbar-burger burger " + (showMenu ? "is-active" : "")} aria-label="menu" aria-expanded="false" onClick={toggleMenu}>
                <span aria-hidden="true" />
                <span aria-hidden="true" />
                <span aria-hidden="true" />
            </a>
        </div><div class={"navbar-menu " + (showMenu ? "is-active" : "")}>
            <div class="navbar-start">
                <a class="navbar-item" href="/">Home</a>
                <a class="navbar-item" href="/demo">Demo</a>
                {loggedIn ? <a class="navbar-item" href="/game">Game</a> : null}
                {loggedIn ? <a class="navbar-item" href="/SignOut">Sign Out</a> : null}
                {!loggedIn ? <a href="/account/signin" class="navbar-item">Sign In with GitHub</a> : null}

                {admin ? <a class="navbar-item" href="/dashboard">Dashboard</a> : null}
                {admin ? <a class="navbar-item" href="/admin">Admin</a> : null}
                <a class="navbar-item" target="_blank" rel="noopener" href="https://codegolf.stackexchange.com/questions/173/tips-for-code-golfing-in-c">Tips</a>
            </div>
        </div>
    </nav >);

interface State { readonly admin: boolean; readonly loggedIn: boolean; readonly showMenu: boolean; }

export default class Comp extends Component<{}, State> {
    constructor() {
        super();
        this.state = { loggedIn: false, admin: false, showMenu: false };
    }
    public readonly componentDidMount = async () => {
        const admin = await isAdmin();
        const loggedIn = await isLoggedIn();
        this.setState(s => ({ ...s, admin, loggedIn }));
    }
    public readonly render = (_: RenderableProps<{}>, s: Readonly<State>) =>
        <FuncComp {...s} toggleMenu={this.ToggleMenu} />
    private readonly ToggleMenu = () => this.setState(s => ({ ...s, showMenu: !s.showMenu }));
}
