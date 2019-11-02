
import { getFunctionDeclaration } from "../funcDeclaration";
import { ChallengeSetId } from "../types/types";

describe("Func Declaration tests", () => {
    test("getFunctionDeclaration", () => {
        const sig = getFunctionDeclaration({id: "id" as ChallengeSetId, title: "func title", description: "description", returnType: "int", params: [], challenges: [] });
        expect(sig).toBe("int Main() { return ... ; }");
    });
    test("getFunctionDeclaration with params", () => {
        const sig = getFunctionDeclaration({id: "id" as ChallengeSetId, title: "func title", description: "description", returnType: "int", params: [{type: "string", suggestedName: "s"}], challenges: [] });
        expect(sig).toBe("int Main(string s) { return ... ; }");
    });
});
