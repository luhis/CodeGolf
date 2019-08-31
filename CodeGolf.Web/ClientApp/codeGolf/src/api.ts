import axios from "axios";

import { getData, HoleInt, JsonHeaders, MapHole, utzParse } from "./Api/utils";
import { Attempt, AttemptWithCode, ChallengeSet, CodeError, Guid, RunError, RunResult } from "./types/types";

export const getDemoChallenge = () => axios
  .get<ChallengeSet>("/api/Challenge/DemoChallenge")
  .then(getData);

interface AttemptInt {
  readonly rank: number;
  readonly id: Guid;
  readonly loginName: string;
  readonly avatar: string;
  readonly score: number;
  readonly timeStamp: string;
}

export const getCurrentHole = () => axios
  .get<HoleInt | undefined>("/api/Challenge/CurrentChallenge")
  .then(getData)
  .then(MapHole);

export const getCurrentChallenge = () => axios
  .get<HoleInt | undefined>("/api/Admin/CurrentHole")
  .then(getData)
  .then(MapHole);

interface Result { readonly score?: number; readonly runErrors?: ReadonlyArray<RunError>; readonly compileErrors?: ReadonlyArray<CodeError>; }

export const submitDemo = (code: string, reCaptcha: string): Promise<RunResult> => axios
  .post<Result>("/api/Challenge/SubmitDemo", JSON.stringify(code), {
    headers: {
      ...JsonHeaders,
      "g-recaptcha-response": reCaptcha
    }
  })
  .then(getData)
  .then(data => {
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
  .post<Result>(`/api/Challenge/SubmitChallenge/${holeId}`, JSON.stringify(code), {
    headers: JsonHeaders
  })
  .then(getData)
  .then(data => {
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
  .post<ReadonlyArray<CodeError>>(`/api/code/TryCompile/${challengeId}`, JSON.stringify(code), {
    headers: JsonHeaders
  })
  .then(getData);

export const getResults = (holeId: Guid) => axios.get<ReadonlyArray<AttemptInt>>(`/api/Admin/Results/${holeId}`)
  .then(getData).then(attempts => attempts.map(a => ({ ...a, timeStamp: utzParse(a.timeStamp) } as Attempt)));

export const nextHole = () => axios.post("/api/Admin/nextHole");

export const endHole = () => axios.post("/api/Admin/endHole");

export const isLoggedIn = () => axios.get<boolean>("/api/Access/isLoggedIn").then(getData);

export const isAdmin = () => axios.get<boolean>("/api/Access/isAdmin").then(getData);

export const getFinalResults = () => axios.get<ReadonlyArray<Result>>("/api/Admin/GetFinalScores").then(getData);

export const getAttempt = (attemptId: Guid) => axios.get<AttemptWithCode>(`/api/Admin/GetAttempt/${attemptId}`).then(getData);

export const getCsFile = (style: ("debug" | "preview"), code: string) => axios.get<string>(`/api/code/${style}?Code=${escape(code)}`).then(getData);
