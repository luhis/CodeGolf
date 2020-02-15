import axios from "axios";

import { Attempt, AttemptId, AttemptWithCode, Game, GameId, HoleId, Result, Round } from "../types/types";
import { getData, HoleInt, JsonHeaders, mapHole, utzParse } from "./utils";

interface AttemptInt {
  readonly rank: number;
  readonly id: AttemptId;
  readonly loginName: string;
  readonly avatar: string;
  readonly score: number;
  readonly timeStamp: string;
}

export const getCurrentChallenge = () => axios
  .get<HoleInt | undefined>("/api/Admin/CurrentHole").then(getData)
  .then(mapHole);

export const getResults = (holeId: HoleId) => axios.get<ReadonlyArray<AttemptInt>>(`/api/Admin/Results/${holeId}`)
  .then(getData).then(attempts => attempts.map(a => ({ ...a, timeStamp: utzParse(a.timeStamp) } as Attempt)));

export const nextHole = () => axios.post<void>("/api/Admin/nextHole").then(getData);

export const endHole = () => axios.post<void>("/api/Admin/endHole").then(getData);

export const getFinalResults = () => axios.get<ReadonlyArray<Result>>("/api/Admin/FinalScores").then(getData);

export const getAttempt = (attemptId: AttemptId) => axios.get<AttemptWithCode>(`/api/Admin/Attempt/${attemptId}`).then(getData);

export const getMyGames = () => axios.get<ReadonlyArray<Game>>("/api/Admin/MyGames").then(getData);

export const resetGame = (gameId: GameId) => axios.post<AttemptWithCode>(`/api/Admin/Reset/${gameId}`).then(getData);

export const getAllChallenges = () => axios.get<ReadonlyArray<Round>>("/api/Admin/AllChallenges").then(getData);

export const addGame = (g: Game) => axios.post<Game>("/api/Admin/CreateGame", JSON.stringify(g), {
  headers: JsonHeaders
});
