import { utzParse } from "../api/utils";

describe("API Utils tests", () => {
    test("utzParse", () => {
        const time = utzParse("2019-01-02T03:04:05.9962911");
        expect(time).toStrictEqual(new Date(2019, 0, 2, 3, 4, 5, 996));
    });
});
