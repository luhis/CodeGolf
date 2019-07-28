import { FunctionalComponent, h } from "preact";

const footer: FunctionalComponent<{}> = () => (<footer class="footer is-thin">
    <div class="content has-text-centered">
        <p>
            &copy; 2019 - CodeGolf - <a asp-area="" asp-page="/Privacy">Privacy</a> - <a href="https://github.com/luhis/codegolf">Fork Me</a>
        </p>
    </div>
</footer>);

export default footer;