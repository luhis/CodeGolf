import axios from "axios";

import { ChallengeSet, CodeError, GameId, HoleId, RunError, RunResult } from "../types/types";
import { getData, HoleInt, JsonHeaders, mapHole } from "./utils";

interface Result { readonly score?: number, readonly runErrors?: ReadonlyArray<RunError>, readonly compileErrors?: ReadonlyArray<CodeError> }

export const getCurrentHole = (id: GameId) => axios
    .get<HoleInt | undefined>(`/api/Challenge/CurrentChallenge/${id}`)
    .then(getData).then(mapHole);

export const getDemoChallenge = () => axios
  .get<ChallengeSet>("/api/Challenge/DemoChallenge")
  .then(getData);

export const verifyGameCode = (accessKey: string) => axios.get<GameId>(`/api/Game/GetGameId?AccessKey=${accessKey}`).then(getData);

export const submitChallenge = (code: string, holeId: HoleId) => axios
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
      return { type: "RunResultSet", errors: data.runErrors } as RunResult;
    }
    throw new Error("failed to convert");
  });

export const tryCompile = (code: string) => axios
  .post<ReadonlyArray<CodeError>>("/api/code/TryCompile", JSON.stringify(code), {
  headers: JsonHeaders
})
  .then(getData);

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
      return { type: "RunResultSet", errors: data.runErrors };
    }
    throw new Error("failed to convert");
  });

export const getCsFile = (style: ("debug" | "preview"), code: string) =>
  axios.post<string>(`/api/code/${style}?Code=${escape(code)}`, undefined, { headers: JsonHeaders }).then(getData);
