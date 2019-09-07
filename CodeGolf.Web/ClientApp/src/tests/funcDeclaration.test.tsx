
// See: https://github.com/mzgoddard/preact-render-spy
import { getChallengeOverView } from "../funcDeclaration";

describe("Func Declaration tests", () => {
    test("getChallengeOverView", () => {
        const sig = getChallengeOverView({args: [], expectedResult: ("string" as unknown as object)}, "string");
        expect(sig).toBe("() => string");
    });
});
