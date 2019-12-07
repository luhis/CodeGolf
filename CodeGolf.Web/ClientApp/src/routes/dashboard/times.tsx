import { FunctionalComponent, h } from "preact";
import { ValidDate } from "ts-date";

const comp: FunctionalComponent<{ readonly start: ValidDate, readonly end: ValidDate }> = ({ start, end }) => (<div class="field is-grouped is-grouped-multiline">
    <div class="control">
        <div class="tags has-addons">
            <span class="tag is-dark">start</span>
            <span class="tag is-success">{start.toLocaleTimeString()}</span>
        </div>
    </div>

    <div class="control">
        <div class="tags has-addons">
            <span class="tag is-dark">end</span>
            <span class="tag is-info">{end.toLocaleTimeString()}</span>
        </div>
    </div>
</div>);

export default comp;
