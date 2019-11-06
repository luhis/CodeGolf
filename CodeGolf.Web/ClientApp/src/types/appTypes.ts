interface Loading { readonly type: "Loading"; }
interface Loaded<T> { readonly type: "Loaded"; readonly data: T; }
export type LoadingState<T> = Loading | Loaded<T>;

export const ifLoaded = <T, TT>(l: LoadingState<T>, some: ((t: T) => TT), none: (() => TT)) => l.type === "Loaded" ? some(l.data) : none();
