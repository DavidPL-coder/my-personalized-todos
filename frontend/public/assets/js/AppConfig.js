export default class AppConfig {
    static getAppConfig() {
        if (document.cookie !== "") {
            const cookies = document.cookie.split(/; */);
    
            for (let cookie of cookies) {
                const [ cookieName, cookieVal ] = cookie.split("=");
                if (cookieName === decodeURIComponent("app-config")) {
                    return JSON.parse(decodeURIComponent(cookieVal));
                }
            }
        }
    
        return undefined; // TODO: Return or do something more better than this
    }
}

export const getAppConfig = AppConfig.getAppConfig;

