import { getAppConfig } from "./AppConfig.js";

const url = getAppConfig().FRONTEND_URL;
document.querySelector("#login-button").href = `${url}/login`;