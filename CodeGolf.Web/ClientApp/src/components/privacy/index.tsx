import { FunctionalComponent, h } from "preact";
import { useState } from "preact/hooks";

import FuncComp from "./funcComp";

const C: FunctionalComponent = () => {
  const [state, update] = useState({ showModal: false });

  const toggleModal = () => update(s => ({ ...s, showModal: !s.showModal }));

  return (<FuncComp {...state} toggleModal={toggleModal} />);
};

export default C;
