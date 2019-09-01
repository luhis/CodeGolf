
interface Loading { readonly type: "Loading"; }
interface Loaded<T> { readonly type: "Loaded"; readonly data: T; }

export interface Score { readonly type: "Score"; readonly val: number; }
export interface CompileError { readonly type: "CompileError"; readonly errors: ReadonlyArray<CodeError>; }
export interface RunErrorSet { readonly type: "RunError"; readonly errors: ReadonlyArray<RunError>; }
type ChallengeResult = Challenge & { readonly found?: object; };

export type RunResult = Score | CompileError | RunErrorSet;

export interface CodeError { readonly line: number; readonly col: number; readonly endCol: number; readonly message: string; }
export interface RunError { readonly error?: string; readonly challenge: ChallengeResult; }
export type LoadingState<T> = Loading | Loaded<T>;

export const ifLoaded = <T, TT>(l: LoadingState<T>, some: ((t: T) => TT), none: (() => TT)) => l.type === "Loaded" ? some(l.data) : none();

export interface ParamDescription { readonly type: string; readonly suggestedName: string; }
export interface Challenge { readonly args: ReadonlyArray<string>; readonly expectedResult: object; }

export interface ChallengeSet {
    readonly id: Guid;
    readonly title: string;
    readonly description: string;
    readonly returnType: string;
    readonly params: ReadonlyArray<ParamDescription>;
    readonly challenges: ReadonlyArray<Challenge>;
}

export type Guid = string & {};

export interface Attempt {
    readonly rank: number;
    readonly id: Guid;
    readonly loginName: string;
    readonly avatar: string;
    readonly score: number;
    readonly timeStamp: Date;
}

export type AttemptWithCode = Attempt & { readonly code: string };

export interface Result {
    readonly score: number;
    readonly rank: number;
    readonly loginName: string;
    readonly avatarUri: string;
}

export interface Hole {
    readonly challengeSet: ChallengeSet; readonly start: Date;
    readonly end: Date; readonly closedAt?: Date; readonly hasNext: boolean; readonly hole: { readonly holeId: Guid };
}

export interface Round {
    readonly id: Guid;
    readonly title: string;
}

export interface Game {
    readonly id: Guid;
    readonly accessKey: string;
    readonly rounds: ReadonlyArray<Round>;
}
