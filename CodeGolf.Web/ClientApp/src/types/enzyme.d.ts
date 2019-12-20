/* eslint-disable id-blacklist */
// The official types conflict with PReact
declare module "enzyme" {
  export const render = ((f: FunctionalComponent<unknown>) => any);
  export const shallow = ((f: FunctionalComponent<unknown>) => any);
  export const configure = ((f: unknown) => undefined);
}
