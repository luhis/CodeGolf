import axios from "axios";

import { getData } from "./utils";

export const isLoggedIn = () => axios.get<boolean>("/api/Access/isLoggedIn").then(getData);

export const isAdmin = () => axios.get<boolean>("/api/Access/isAdmin").then(getData);
