import { FunctionalComponent, h } from "preact";

import Dashboard from "../../images/Dashboard.png";
import Game from "../../images/Game.png";

const Comp: FunctionalComponent = () => <div>
    <section class="section">
        <div class="container">
            <h1 class="title">What is code golf?</h1>
            <h2 class="subtitle">
                Code golf is a game where developers aim to complete a Kata using as few characters as possible.
            </h2>
        </div>
    </section>

    <section class="section">
        <div class="container">
            <h1 class="title">What does this website allow me to do?</h1>
            <h2 class="subtitle">
                This website aims to create a code golf framework to allow developers to meet at user groups and compete together.
            </h2>
        </div>
    </section>

    <section class="section">
        <div class="container">
            <h1 class="title">How do you calculate a score?</h1>
            <h2 class="subtitle">
                The score is based on the number chars in the solution, minus whitespace.  Variable names and curlies count.
            </h2>
        </div>
    </section>

    <section class="section">
        <div class="container">
            <h1 class="title">Which version of C# do you support?</h1>
            <h2 class="subtitle">
                Currently supporting C# 8.  Feel free to try a switch expression in your next Kata!
            </h2>
        </div>
    </section>

    <section class="section">
        <div class="container">
            <h1 class="title">Screenshots</h1>
            <h2 class="subtitle">
                <figure>
                    <img src={Game} />
                </figure>
                <br />
                <figure>
                    <img src={Dashboard} />
                </figure>
            </h2>
        </div>
    </section>

    <section class="section">
        <div class="container">
            <h1 class="title">Technologies</h1>
            <div class="content">
                Front End:
                <ul>
                    <li>Preact</li>
                    <li>TypeScript 3.8</li>
                    <li>Bulma</li>
                    <li>SignalR</li>
                </ul>
                Back End:
                <ul>
                    <li>DotNetCore 3.1</li>
                    <li>GRPC</li>
                    <li>SqLite</li>
                    <li>Roslyn</li>
                </ul>
            </div>
        </div>
    </section>
</div>;

export default Comp;
