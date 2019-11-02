import { Challenge, ChallengeSet, ParamDescription } from "./types/types";

export const getInput = (challenge: Challenge) => `(${challenge.args.join(", ")})`;

export const getFunctionDeclaration = (challenge: ChallengeSet) =>
    `${challenge.returnType} Main(${challenge.params
        .map((a: ParamDescription) => `${a.type} ${a.suggestedName}`)
        .join(", ")}) { return ... ; }`;
