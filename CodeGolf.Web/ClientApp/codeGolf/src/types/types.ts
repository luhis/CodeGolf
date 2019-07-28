
export interface Loading { type: "Loading"; }

interface Loaded<T> { type: "Loaded"; readonly data: T; }
export interface Score { type: "Score"; val: number; }
export interface CompileError { type: "CompileError"; errors: ReadonlyArray<Error>; }
export interface RunErrorSet { type: "RunError"; errors: ReadonlyArray<RunError>; }
type ChallengeResult = Challenge & { found?: object; };

export type RunResult = Score | CompileError | RunErrorSet;

export interface Error { readonly line: number; readonly col: number; readonly endCol: number; readonly message: string; }
export interface RunError { readonly error?: string; readonly challenge: ChallengeResult; }
export type LoadingState<T> = Loading | Loaded<T>;

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
    readonly timeStamp: string;
}

export type AttemptWithCode = Attempt & {code: string };

export interface Result {
    readonly score: number;
    readonly rank: number;
    readonly loginName: string;
    readonly avatarUri: string;
}

export interface Hole { challengeSet: ChallengeSet; start: Date; end: Date; closedAt?: Date; hasNext: boolean; hole: { holeId: Guid }; }
