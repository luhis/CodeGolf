import { faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Markdown from "markdown-to-jsx";
import { FunctionalComponent, h } from "preact";

import { getFunctionDeclaration, getInput } from "../../funcDeclaration";
import { Challenge, ChallengeSet } from "../../types/types";

interface Props { readonly challenge: ChallengeSet; readonly onCodeClick?: () => void; }

const X: FunctionalComponent<{ readonly challenge: Challenge, readonly returnType: string; }> = ({ challenge, returnType }) =>
    (<div
        class="panel-block"
    >{getInput(challenge)} =>
    <pre class="result">{challenge.expectedResult}</pre>
    </div>);

const Comp: FunctionalComponent<Readonly<Props>> = ({ challenge, onCodeClick }) => (<div class="panel">
    <div class="panel-heading">
        <p>{challenge.title}</p>
    </div>
    <div class="panel-block">
        <div class="content">
            <Markdown>{challenge.description}
            </Markdown>
        </div>
    </div>
    {challenge.challenges.map(a => <X key={a.expectedResult.toString()} challenge={a} returnType={challenge.returnType} />)}
    <div class="panel-block">
        <FontAwesomeIcon icon={faInfoCircle} className="has-text-info" />&nbsp;
        Create a function like&nbsp;
        <code
            class={onCodeClick ? "is-clickable" : ""}
            onClick={onCodeClick}
        >{getFunctionDeclaration(challenge)}</code>
    </div>
</div >);

export default Comp;
