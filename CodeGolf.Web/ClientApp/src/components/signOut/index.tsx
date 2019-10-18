import { FunctionalComponent, h } from "preact";
import { useState } from "preact/hooks";

import { signOut } from "../../api/accessApi";
import FuncComp from "./funcComp";

const signOutAndRedirect = async () => {
    await signOut();
    window.location.replace("/");
};

const C: FunctionalComponent<{}> = () => {
    const [state, update] = useState({showModal: false });

    const toggleModal = () => update(s => ({ ...s, showModal: !s.showModal }));

    return (<FuncComp {...state} signOutFunc={signOutAndRedirect} toggleModal={toggleModal} />);
};

export default C;
