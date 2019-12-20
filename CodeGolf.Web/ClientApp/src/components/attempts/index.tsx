import { FunctionalComponent, h } from "preact";

import { Attempt } from "../../types/types";
import Icon from "../icons";

const Row: FunctionalComponent<{ readonly attempt: Attempt }> = ({ attempt }) => (<tr>
  <td>{attempt.rank}.</td>
  <td><figure class="image is-48x48"><img src={attempt.avatar} /></figure></td>
  <td>{attempt.loginName}</td>
  <td>{attempt.score}</td>
  <td>{attempt.timeStamp.toLocaleTimeString()}</td>
  <td>
    <a target="_blank" rel="noopener noreferrer" href={`/attempt/${attempt.id}`} class="button">
      <Icon icon="code" />&nbsp;View Code
    </a>
  </td>
</tr>);

interface Props { readonly attempts: ReadonlyArray<Attempt> }

const comp: FunctionalComponent<Readonly<Props>> = ({ attempts }) => (<table class="table is-striped is-fullwidth">
  <thead>
    <tr>
      <th />
      <th />
      <th>Name</th>
      <th>Score</th>
      <th>Time</th>
      <th>Code</th>
    </tr>
  </thead>
  <tbody>
    {attempts.map(a => <Row attempt={a} key={a.id} />)}
  </tbody>
</table>);

export default comp;
