import { FunctionalComponent, h } from "preact";

import Privacy from "../privacy";

const footer: FunctionalComponent = () => (<footer class="footer is-thin">
    <div class="content has-text-centered">
        <p>
            &copy; 2019 - CodeGolf - <Privacy /> - <a href="https://github.com/luhis/codegolf">Fork Me</a>
        </p>
    </div>
</footer>);

export default footer;
