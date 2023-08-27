export default class AppConfigProvider {
    static getConfig() {
        return {
            FRONTEND_CONTAINER_PORT: process.env.MPT_FRONTEND_CONTAINER_PORT,
            FRONTEND_URL: process.env.MPT_FRONTEND_URL,
            MIN_LOGIN_LENGTH: process.env.MPT_MIN_LOGIN_LENGTH,
            MIN_PASSWORD_LENGTH: process.env.MPT_MIN_PASSWORD_LENGTH,
            MIN_AGE: process.env.MPT_MIN_AGE,
            MAX_AGE: process.env.MPT_MAX_AGE
        };
    }
}