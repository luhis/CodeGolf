import axios from "axios";

import { Access } from "../types/types";
import { getData } from "./utils";

export const getAccess = () => axios.get<Access>("/api/Access/getAccess").then(getData);

export const signOut = () => axios.post<void>("/api/Access/signOut").then(getData);
