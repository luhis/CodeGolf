import axios from "axios";

import { getData } from "./utils";

export const isLoggedIn = () => axios.get<boolean>("/api/Access/isLoggedIn").then(getData);

export const isAdmin = () => axios.get<boolean>("/api/Access/isAdmin").then(getData);

export const signOut = () => axios.post<void>("/api/Access/signOut").then(getData);
