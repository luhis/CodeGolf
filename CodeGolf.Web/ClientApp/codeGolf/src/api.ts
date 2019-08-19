import axios from "axios";

import { Attempt, AttemptWithCode, ChallengeSet, CodeError, Guid, Hole, RunError, RunResult } from "./types/types";

export const getDemoChallenge = () => axios
  .get<ChallengeSet>("api/Challenge/DemoChallenge")
  .then(response => response.data);

const utzParse = (date: string) => new Date(date + "z");

interface HoleInt { readonly challengeSet: ChallengeSet; readonly start: string; readonly end: string; readonly closedAt?: string; }
interface AttemptInt {
  readonly rank: number;
  readonly id: Guid;
  readonly loginName: string;
  readonly avatar: string;
  readonly score: number;
  readonly timeStamp: string;
}
const MapHole = (h?: HoleInt) => {
  if (h) {
    return { ...h, start: utzParse(h.start), end: utzParse(h.end), closedAt: h.closedAt ? utzParse(h.closedAt) : undefined } as Hole;
  }

  // tslint:disable-next-line: no-return-undefined
  return undefined;
};

export const getCurrentHole = () => axios
  .get<HoleInt | undefined>("api/Challenge/CurrentChallenge")
  .then(response => MapHole(response.data));

export const getCurrentChallenge = () => axios
  .get<HoleInt | undefined>("api/Admin/CurrentHole")
  .then(response => MapHole(response.data));

interface Result { readonly score?: number; readonly runErrors?: ReadonlyArray<RunError>; readonly compileErrors?: ReadonlyArray<CodeError>; }

export const submitDemo = (code: string, reCaptcha: string): Promise<RunResult> => axios
  .post<Result>("api/Challenge/SubmitDemo", JSON.stringify(code), {
    headers: {
      "Content-Type": "application/json",
      "g-recaptcha-response": reCaptcha
    }
  })
  .then(response => {
    const data = response.data;
    if (data.score) {
      return { type: "Score", val: data.score };
    }
    if (data.compileErrors) {
      return { type: "CompileError", errors: data.compileErrors };
    }
    if (data.runErrors) {
      return { type: "RunError", errors: data.runErrors };
    }
    throw new Error("failed to convert");
  });

export const submitChallenge = (code: string, holeId: Guid) => axios
  .post<Result>(`api/Challenge/SubmitChallenge/${holeId}`, JSON.stringify(code), {
    headers: {
      "Content-Type": "application/json"
    }
  })
  .then(response => {
    const data = response.data;
    if (data.score) {
      return { type: "Score", val: data.score } as RunResult;
    }
    if (data.compileErrors) {
      return { type: "CompileError", errors: data.compileErrors } as RunResult;
    }
    if (data.runErrors) {
      return { type: "RunError", errors: data.runErrors } as RunResult;
    }
    throw new Error("failed to convert");
  });

export const tryCompile = (challengeId: Guid, code: string) => axios
  .post<ReadonlyArray<CodeError>>(`api/code/TryCompile/${challengeId}`, JSON.stringify(code), {
    headers: {
      "Content-Type": "application/json"
    }
  })
  .then(response => response.data);

export const getResults = (holeId: Guid) => axios.get<ReadonlyArray<AttemptInt>>(`./api/Admin/Results/${holeId}`)
  .then(response => response.data).then(attempts => attempts.map(a => ({...a, timeStamp: utzParse(a.timeStamp)} as Attempt)));

export const nextHole = () => axios.post("./api/Admin/nextHole");

export const endHole = () => axios.post("./api/Admin/endHole");

export const isLoggedIn = () => axios.get<boolean>("/api/Access/isLoggedIn").then(a => a.data);

export const isAdmin = () => axios.get<boolean>("/api/Access/isAdmin").then(a => a.data);

export const getFinalResults = () => axios.get<ReadonlyArray<Result>>("/api/Admin/GetFinalScores").then(a => a.data);

export const getAttempt = (attemptId: Guid) => axios.get<AttemptWithCode>(`/api/Admin/GetAttempt/${attemptId}`).then(a => a.data);
