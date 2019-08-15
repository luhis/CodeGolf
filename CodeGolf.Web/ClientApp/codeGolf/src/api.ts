import axios from "axios";

import { Attempt, AttemptWithCode, ChallengeSet, Error, Guid, Hole, RunError, RunResult } from "./types/types";

export const getDemoChallenge = () => axios
  .get<ChallengeSet>("api/Challenge/DemoChallenge")
  .then(response => response.data);

interface HoleInt { challengeSet: ChallengeSet; start: string; end: string; closedAt?: string; }

export const getCurrentHole = () => axios
  .get<HoleInt | null>("api/Challenge/CurrentChallenge")
  .then(response => {
    const d = response.data;
    if (d) {
      return { ...d, start: new Date(d.start), end: new Date(d.end), closedAt: d.closedAt ? new Date(d.closedAt) : undefined } as Hole;
    }
    
      return null;
    
  });

export const getCurrentChallenge = () => axios
  .get<HoleInt>("api/Admin/CurrentHole")
  .then(response => {
    const d = response.data;
    return { ...d, start: new Date(d.start), end: new Date(d.end), closedAt: d.closedAt ? new Date(d.closedAt) : undefined } as Hole;
  });

interface Result { readonly score?: number; readonly runErrors?: ReadonlyArray<RunError>; readonly compileErrors?: ReadonlyArray<Error>; }

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
  .post<ReadonlyArray<Error>>(`api/code/TryCompile/${challengeId}`, JSON.stringify(code), {
    headers: {
      "Content-Type": "application/json"
    }
  })
  .then(response => response.data);

export const getResults = (holeId: Guid) => axios.get<ReadonlyArray<Attempt>>(`./api/Admin/Results/${holeId}`)
  .then(response => response.data);

export const nextHole = () => axios.post("./api/Admin/nextHole");

export const endHole = () => axios.post("./api/Admin/endHole");

export const isLoggedIn = () => axios.get<boolean>("/api/Access/isLoggedIn").then(a => a.data);

export const isAdmin = () => axios.get<boolean>("/api/Access/isAdmin").then(a => a.data);

export const getFinalResults = () => axios.get<ReadonlyArray<Result>>("/api/Admin/GetFinalScores").then(a => a.data);

export const getAttempt = (attemptId: Guid) => axios.get<AttemptWithCode>(`/api/Admin/GetAttempt/${attemptId}`).then(a => a.data);