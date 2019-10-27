
// See: https://github.com/mzgoddard/preact-render-spy
import { getChallengeOverView, getFunctionDeclaration } from "../funcDeclaration";
import { ChallengeSetId } from "../types/types";

describe("Func Declaration tests", () => {
    test("getChallengeOverView", () => {
        const sig = getChallengeOverView({args: [], expectedResult: ("string" as unknown as object)}, "string");
        expect(sig).toBe("() => string");
    });
    test("getChallengeOverView with params", () => {
        const sig = getChallengeOverView({args: ["int", "string"], expectedResult: ("string" as unknown as object)}, "string");
        expect(sig).toBe("(int, string) => string");
    });
    test("getFunctionDeclaration", () => {
        const sig = getFunctionDeclaration({id: "id" as ChallengeSetId, title: "func title", description: "description", returnType: "int", params: [], challenges: [] });
        expect(sig).toBe("int Main() { return ... ; }");
    });
    test("getFunctionDeclaration with params", () => {
        const sig = getFunctionDeclaration({id: "id" as ChallengeSetId, title: "func title", description: "description", returnType: "int", params: [{type: "string", suggestedName: "s"}], challenges: [] });
        expect(sig).toBe("int Main(string s) { return ... ; }");
    });
});
