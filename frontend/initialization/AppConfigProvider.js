export default class AppConfigProvider {
    static getConfig() {
        // Is it secure to use backend url in app in this way?
        return {
            FRONTEND_CONTAINER_PORT: process.env.MPT_FRONTEND_CONTAINER_PORT,
            FRONTEND_URL: process.env.MPT_FRONTEND_URL,
            BACKEND_URL: process.env.MPT_BACKEND_URL,
            BACKEND_URL_FOR_NODE: process.env.MPT_BACKEND_URL_FOR_NODE,
            MIN_LOGIN_LENGTH: process.env.MPT_MIN_LOGIN_LENGTH,
            MIN_PASSWORD_LENGTH: process.env.MPT_MIN_PASSWORD_LENGTH,
            MIN_AGE: process.env.MPT_MIN_AGE,
            MAX_AGE: process.env.MPT_MAX_AGE
        };
    }
}