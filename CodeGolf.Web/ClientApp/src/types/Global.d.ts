declare interface NodeModule {
  readonly hot: {
    readonly accept: (readonly path?: string, readonly fn: () => void, readonly callback?: () => void) => void;
  };
}
