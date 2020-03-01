import { FunctionalComponent, h } from "preact";

import { Score } from "../../types/types";

interface Props { readonly score: Score }

const Comp: FunctionalComponent<Props> = ({ score }) => (
  <article class="message is-success">
    <div class="message-header">
      <p>Success</p>
    </div>
    <div class="message-body">
      Score: {score.val}
    </div>
  </article>);

export default Comp;
