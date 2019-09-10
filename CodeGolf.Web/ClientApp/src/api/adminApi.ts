import axios from "axios";

import { Attempt, AttemptWithCode, Game, Guid, Result, Round } from "../types/types";
import { getData, HoleInt, JsonHeaders, MapHole, utzParse } from "./utils";

interface AttemptInt {
  readonly rank: number;
  readonly id: Guid;
  readonly loginName: string;
  readonly avatar: string;
  readonly score: number;
  readonly timeStamp: string;
}

export const getCurrentChallenge = () => axios
  .get<HoleInt | undefined>("/api/Admin/CurrentHole").then(getData)
  .then(MapHole);

export const getResults = (holeId: Guid) => axios.get<ReadonlyArray<AttemptInt>>(`/api/Admin/Results/${holeId}`)
  .then(getData).then(attempts => attempts.map(a => ({ ...a, timeStamp: utzParse(a.timeStamp) } as Attempt)));

export const nextHole = () => axios.post("/api/Admin/nextHole");

export const endHole = () => axios.post("/api/Admin/endHole");

export const getFinalResults = () => axios.get<ReadonlyArray<Result>>("/api/Admin/FinalScores").then(getData);

export const getAttempt = (attemptId: Guid) => axios.get<AttemptWithCode>(`/api/Admin/Attempt/${attemptId}`).then(getData);

export const getMyGames = () => axios.get<ReadonlyArray<Game>>("/api/Admin/MyGames").then(getData);

export const resetGame = (gameId: Guid)=> axios.post<AttemptWithCode>(`/api/Admin/Reset/${gameId}`).then(getData);

export const getAllChallenges = () => axios.get<ReadonlyArray<Round>>("/api/Admin/AllChallenges").then(getData);

export const AddGame = (g: Game) => axios.post<Game>("/api/Admin/CreateGame", JSON.stringify(g), {
  headers: JsonHeaders
});
