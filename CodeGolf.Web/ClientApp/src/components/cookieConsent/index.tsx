import { Component, FunctionalComponent, h, RenderableProps } from "preact";

const F: FunctionalComponent = () => (<p>This website uses cookies. By continuing to use our site, you agree to our use of cookies</p>);

interface Props { readonly showWarning: boolean; }

const cookieName = "accepted-cookies";

export default class Comp extends Component<{}, Props> {
  constructor() {
    super();
    const hasCookie = document.cookie.split(";").filter(item => item.indexOf(`${cookieName}=`) >= 0).length;
    if (!hasCookie) {
      document.cookie = `${cookieName}=1`;
    }
    this.state = { showWarning: !hasCookie };
  }
  public readonly render = (_: RenderableProps<{}>, { showWarning }: Readonly<Props>) => showWarning ?
    <F /> : null
}
