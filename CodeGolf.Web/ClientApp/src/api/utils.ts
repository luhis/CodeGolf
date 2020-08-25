import { AxiosResponse } from "axios";
import { newValidDate } from "ts-date";

import { ChallengeSet, Hole, HoleId } from "../types/types";

export const utzParse = (date: string) => newValidDate(date + "z");

const nullToUndef = <T extends unknown>(t: T | null) => t == null ? undefined : t;

export const mapHole = (h?: HoleInt) => {
  if (h) {
    const start = utzParse(h.start);
    const end = utzParse(h.end);
    if (start && end) {
      const closed = h.closedAt ? nullToUndef(utzParse(h.closedAt)) : undefined;
      return { ...h, start, end, closedAt: closed } as Hole;
    }
  }

  // eslint-disable-next-line id-blacklist
  return undefined;
};

export interface HoleInt {
  readonly challengeSet: ChallengeSet;
  readonly start: string;
  readonly end: string;
  readonly closedAt?: string;
  readonly hasNext: boolean;
  readonly hole: { readonly holeId: HoleId };
}

export const getData = <T>(r: AxiosResponse<T>) => r.data;

export const JsonHeaders = {
  "Content-Type": "application/json"
};
