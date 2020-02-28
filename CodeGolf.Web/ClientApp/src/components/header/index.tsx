import { Component, Fragment, FunctionalComponent, FunctionComponent, h, RenderableProps } from "preact";
import { Link } from "preact-router";

import { getAccess } from "../../api/accessApi";
import { Access } from "../../types/types";
import SignOut from "../signOut";

const AdminSection: FunctionComponent<{ readonly admin: boolean }> = ({ admin }) => admin ?
  (<Link class="navbar-item" href="/admin">Admin</Link>) : null;

const DashboardSection: FunctionComponent<{ readonly showDashboard: boolean }> = ({ showDashboard }) => showDashboard ?
  (<Link class="navbar-item" href="/dashboard">Dashboard</Link>) : null;

const LoggedInSection: FunctionComponent<{ readonly loggedIn: boolean }> = ({ loggedIn }) => loggedIn ?
  (<Fragment>
    <Link class="navbar-item" href="/game">Game</Link>
    <SignOut />
  </Fragment>) : null;

const SignIn: FunctionalComponent<{ readonly loggedIn: boolean }> =
  ({ loggedIn }) => !loggedIn ? <a href="/account/signin" class="navbar-item">Sign In with GitHub</a> : null;

const FuncComp: FunctionalComponent<State & { readonly toggleMenu: (() => void) }> = ({ access, toggleMenu, showMenu }) =>
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
        <LoggedInSection loggedIn={access.isLoggedIn} />
        <SignIn loggedIn={access.isLoggedIn} />
        <DashboardSection showDashboard={access.showDashboard} />
        <AdminSection admin={access.isAdmin} />
        <a class="navbar-item" target="_blank" rel="noopener" href="https://codegolf.stackexchange.com/questions/173/tips-for-code-golfing-in-c">Tips</a>
      </div>
    </div>
  </nav >);

interface State { readonly access: Access; readonly showMenu: boolean; }

export default class Comp extends Component<{}, State> {
  constructor() {
    super();
    this.state = { access: { isAdmin: false, isLoggedIn: false, showDashboard: false }, showMenu: false };
  }
  public readonly componentDidMount = async () => {
    const access = await getAccess();
    this.setState(s => ({ ...s, access }));
  }
  public readonly render = (_: RenderableProps<{}>, s: Readonly<State>) =>
    <FuncComp {...s} toggleMenu={this.ToggleMenu} />
  private readonly ToggleMenu = () => this.setState(s => ({ ...s, showMenu: !s.showMenu }));
}
