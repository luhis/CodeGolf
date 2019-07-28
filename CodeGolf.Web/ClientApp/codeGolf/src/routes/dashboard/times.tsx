import { FunctionalComponent, h } from "preact";

const comp: FunctionalComponent<{ start: Date, end: Date }> = ({ start, end }) => (<div class="field is-grouped is-grouped-multiline">
    <div class="control">
        <div class="tags has-addons">
            <span class="tag is-dark">start</span>
            <span class="tag is-success">{start.toTimeString()}</span>
        </div>
    </div>

    <div class="control">
        <div class="tags has-addons">
            <span class="tag is-dark">end</span>
            <span class="tag is-info">{end.toTimeString()}</span>
        </div>
    </div>
</div>);

export default comp;
