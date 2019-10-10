import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionalComponent, h } from "preact";

const FuncComp: FunctionalComponent = () =>
 <div><FontAwesomeIcon icon={faSpinner} spin={true}/> Loading...</div>;

export default FuncComp;
