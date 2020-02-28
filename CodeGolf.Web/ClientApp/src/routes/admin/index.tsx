import { FunctionalComponent, h } from "preact";
import * as Hooks from "preact/hooks";

import { getAllChallenges, getMyGames, resetGame } from "../../api/adminApi";
import { LoadingState } from "../../types/appTypes";
import { Game, Round } from "../../types/types";
import FuncComp from "./funcComp";

interface State {
  readonly myGames: LoadingState<ReadonlyArray<Game>>;
  readonly allChallenges: LoadingState<ReadonlyArray<Round>>;
  readonly showCreate: boolean;
}

const comp: FunctionalComponent<{}> = () => {
  const [state, setState] = Hooks.useState<State>({ myGames: { type: "Loading" }, allChallenges: { type: "Loading" }, showCreate: false });
  Hooks.useEffect(() => {
    const fetchData = async () => {
      const games = await getMyGames();
      const challenges = await getAllChallenges();
      setState(s => ({ ...s, myGames: { type: "Loaded", data: games }, allChallenges: { type: "Loaded", data: challenges } }));
    };
    // tslint:disable-next-line: no-floating-promises
    fetchData();
  }, []);

  const setShowCreate = (show: boolean) => setState(s => ({ ...s, showCreate: show }));

  return (<FuncComp {...state} toggleCreate={setShowCreate} resetGame={resetGame} />);
};

export default comp;
