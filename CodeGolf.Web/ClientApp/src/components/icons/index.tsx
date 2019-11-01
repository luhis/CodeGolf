import { IconName, library } from "@fortawesome/fontawesome-svg-core";
import { faCheckCircle, faCode, faExclamationTriangle, faInfoCircle, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon, FontAwesomeIconProps } from "@fortawesome/react-fontawesome";
import { FunctionComponent, h } from "preact";

library.add(
    faCheckCircle,
    faCode,
    faExclamationTriangle,
    faInfoCircle,
    faSpinner,
);

type IconString = Extract<IconName, "check-circle" | "code" | "exclamation-triangle" | "info-circle" | "spinner">;

const Icon: FunctionComponent<Omit<FontAwesomeIconProps, "icon"> & {readonly icon: IconString}> = (props) => {
    const {icon, ...rest} = props;
    return <FontAwesomeIcon {...rest} icon={icon} />;
};

export default Icon;
