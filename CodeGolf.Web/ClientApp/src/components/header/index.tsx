import { Component, Fragment, FunctionalComponent, FunctionComponent, h, RenderableProps } from "preact";
import { Link } from "preact-router";

import { isAdmin, isLoggedIn } from "../../api/accessApi";
import SignOut from "../signOut";

const AdminSection: FunctionComponent<{ readonly admin: boolean }> = ({ admin }) => admin ?
    (<Fragment>
        <Link class="navbar-item" href="/dashboard">Dashboard</Link>
        <Link class="navbar-item" href="/admin">Admin</Link>
    </Fragment>) : null;

const LoggedInSection: FunctionComponent<{ readonly loggedIn: boolean }> = ({ loggedIn }) => loggedIn ?
    (<Fragment>
        <Link class="navbar-item" href="/game">Game</Link>
        <SignOut />
    </Fragment>) : null;

const SignIn: FunctionalComponent<{ readonly loggedIn: boolean }> =
    ({ loggedIn }) => !loggedIn ? <a href="/account/signin" class="navbar-item">Sign In with GitHub</a> : null;

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
                <Link class="navbar-item" href="/">Home</Link>
                <Link class="navbar-item" href="/demo">Demo</Link>
                <LoggedInSection loggedIn={loggedIn} />
                <SignIn loggedIn={loggedIn} />
                <AdminSection admin={admin} />
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
