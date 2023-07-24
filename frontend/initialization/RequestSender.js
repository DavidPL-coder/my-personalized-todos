import axios from "axios";

// TODO: Use diffrent color for error message?

export default class RequestSender {

    static SERVER_URL = "http://ec2-52-57-252-68.eu-central-1.compute.amazonaws.com:8080";

    static async get(url, req) {
        return await this.#sendRequest(async config => await axios.get(url, config), req.headers.cookie);
    }

    static async post(url, dataToSend, req) {
        return await this.#sendRequest(async config => await axios.post(url, dataToSend, config), req.headers.cookie);
    }

    static async delete(url, req) {
        return await this.#sendRequest(async config => await axios.delete(url, config), req.headers.cookie);
    }

    static async put(url, dataToSend, req) {
        return await this.#sendRequest(async config => await axios.put(url, dataToSend, config), req.headers.cookie);
    }

    static async getAuthorizedUserName(req) {
        const response = await RequestSender.get(`${this.SERVER_URL}/api/account/username`, req);
        return response.data.name;
    }

    static async #sendRequest(requestFunc, cookieToSend) {
        try {
            const config = { 
                headers: { 
                    cookie: cookieToSend
                } 
            };
            return await requestFunc(config);
        }
        catch (error) {
            if (error.response) {
                console.log("Server message:", error.response.data);
                console.log("Status code:", error.response.status);
                console.log("Data from request body:", error.response.config.data);
                return error.response;
            }
            else if (error.request)
                console.log("Request error:", error.request);
            else
                console.log("Unknown error:", error.message);
            
            return null; // TODO: Return something better in this case
        }
    }
}