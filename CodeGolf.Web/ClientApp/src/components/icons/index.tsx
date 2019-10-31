import { library } from "@fortawesome/fontawesome-svg-core";
import { faCheckCircle, faCode, faExclamationTriangle, faInfoCircle, faSpinner, IconDefinition } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon, FontAwesomeIconProps } from "@fortawesome/react-fontawesome";
import { FunctionComponent, h } from "preact";

library.add(
    faCheckCircle,
    faCode,
    faExclamationTriangle,
    faInfoCircle,
    faSpinner,
);

type iconString = "check-circle" | "code" | "exclamation-triangle" | "info-circle" | "spinner";

const Icon: FunctionComponent<Omit<FontAwesomeIconProps, "icon"> & {readonly icon: iconString}> = (props) => {
    const {icon, ...rest} = props;
    const iconPrime = icon as unknown as IconDefinition;
    return <FontAwesomeIcon {...rest} icon={iconPrime} />;
};

export default Icon;
