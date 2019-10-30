// the official types conflict with PReact
declare module "enzyme" {
    export const render = ((f: FunctionalComponent<any>) => any);
    export const shallow = ((f: FunctionalComponent<any>) => any);
    export const configure = ((f: any) => undefined);
}
