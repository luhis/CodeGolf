import { Challenge, ChallengeSet, ParamDescription } from "./types/types";

const convertReturn = (obj: object, returnType: string) => {
    return Array.isArray(obj) ? `[${(obj as any[]).map(a => returnType.startsWith("string") ? `"${a}"` : a).join(", ")}]` : obj.toString();
};

export const getChallengeOverView = (challenge: Challenge, returnType: string) =>
    `(${challenge.args.join(", ")}) => ${convertReturn(challenge.expectedResult, returnType)}`;

export const getFunctionDeclaration = (challenge: ChallengeSet) =>
    `${challenge.returnType} Main(${challenge.params
        .map((a: ParamDescription) => `${a.type} ${a.suggestedName}`)
        .join(", ")}) { return ... ; }`;
