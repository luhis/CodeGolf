import { FunctionComponent, h } from "preact";

const Template: FunctionComponent<{ readonly name: string, readonly score: number, readonly avatarUri: string }> =
    ({ name, score, avatarUri }) =>
        (<div>
            <p>New Top Score!</p>
            <figure class="image container is-48x48" > <img src={avatarUri} />
            </figure >
            <p>{name}, {score} strokes</p>
        </div>);

export default Template;
