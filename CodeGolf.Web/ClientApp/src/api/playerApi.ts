import axios from "axios";

import { ChallengeSet, CodeError, HoleId, RunError, RunResult } from "../types/types";
import { getData, HoleInt, JsonHeaders, mapHole } from "./utils";

type Result = { readonly type: "Score", readonly score?: number } |
{ readonly type: "CompileError", readonly compileErrors: ReadonlyArray<CodeError> } |
{ readonly type: "RunResultSet", readonly runErrors: ReadonlyArray<RunError> }

export const getCurrentHole = () => axios
  .get<HoleInt | undefined>("/api/Challenge/CurrentChallenge")
  .then(getData).then(mapHole);

export const getDemoChallenge = () => axios
  .get<ChallengeSet>("/api/Challenge/DemoChallenge")
  .then(getData);

export const submitChallenge = (code: string, holeId: HoleId): Promise<RunResult> => axios
  .post<Result>(`/api/Challenge/SubmitChallenge/${holeId}`, JSON.stringify(code), {
    headers: JsonHeaders
  })
  .then(getData)
  .then(data => {
    switch (data.type) {
      case "Score":
        return { type: "Score", val: data.score } as RunResult;
      case "CompileError":
        return { type: "CompileError", errors: data.compileErrors } as RunResult;
      case "RunResultSet":
        return { type: "RunResultSet", errors: data.runErrors } as RunResult;
    }
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
    switch (data.type) {
      case "Score":
        return { type: "Score", val: data.score } as RunResult;
      case "CompileError":
        return { type: "CompileError", errors: data.compileErrors } as RunResult;
      case "RunResultSet":
        return { type: "RunResultSet", errors: data.runErrors } as RunResult;
    }
  });

export const getCsFile = (style: ("debug" | "preview"), code: string) =>
  axios.post<string>(`/api/code/${style}?Code=${escape(code)}`, undefined, { headers: JsonHeaders }).then(getData);
