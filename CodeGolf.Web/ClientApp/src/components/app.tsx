import "bulma";

import { FunctionalComponent, h } from "preact";
import { Route, Router } from "preact-router";
import ReactGA from "react-ga";

import Admin from "../routes/admin";
import Attempt from "../routes/attempt";
import CodeFile from "../routes/codefile";
import Dashboard from "../routes/dashboard";
import Demo from "../routes/demo";
import Game from "../routes/game";
import Home from "../routes/home";
import Results from "../routes/results";
import CookieConsent from "./cookieConsent";
import Footer from "./footer";
import Header from "./header";

if (module.hot) {
  // tslint:disable-next-line:no-var-requires
  require("preact/debug");
}

ReactGA.initialize(process.env.PREACT_APP_GA_KEY as string);

const Comp: FunctionalComponent = () => (
  <div id="app">
    <Header />
    <div class="container is-fluid Site-content">
      <main role="main">
        <CookieConsent />
        <Router>
          <Route path="/" component={Home} />
          <Route path="/admin/" component={Admin} />
          <Route path="/demo/" component={Demo} />
          <Route path="/dashboard/" component={Dashboard} />
          <Route path="/game/" component={Game} />
          <Route path="/results/" component={Results} />
          <Route path="/attempt/:attemptId" component={Attempt} />
          <Route path="/codefile/:type/" component={CodeFile} />
        </Router>
      </main>
    </div>
    <Footer />
  </div>
);

export default Comp;
