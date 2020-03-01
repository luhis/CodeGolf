import { FunctionalComponent, h } from "preact";
import { useEffect, useState } from "preact/hooks";

const F: FunctionalComponent = () => (<p>This website uses cookies. By continuing to use our site, you agree to our use of cookies</p>);

interface Props { readonly showWarning: boolean }

const cookieName = "accepted-cookies";

const Comp: FunctionalComponent = () => {
  const [state, setState] = useState<Props>({ showWarning: false });
  useEffect(() => {
    const hasCookie = document.cookie.split(";").filter(item => item.indexOf(`${cookieName}=`) >= 0).length > 0;
    if (!hasCookie) {
      // eslint-disable-next-line functional/immutable-data
      document.cookie = `${cookieName}=1`;
    }
    setState({ showWarning: !hasCookie });
  }, []);
  return state.showWarning ?
    <F /> : null;
};

export default Comp;
