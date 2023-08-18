import axios from "axios";

// TODO: Use diffrent color for error message?
// TODO: Decrease the number of methods arguments 

export default class RequestSender {

    static async get(url, req, res) {
        return await this.#sendRequest(async config => await axios.get(url, config), req.headers.cookie, res);
    }

    static async post(url, dataToSend, req, res) {
        return await this.#sendRequest(async config => await axios.post(url, dataToSend, config), req.headers.cookie, res);
    }

    static async delete(url, req, res) {
        return await this.#sendRequest(async config => await axios.delete(url, config), req.headers.cookie, res);
    }

    static async put(url, dataToSend, req, res) {
        return await this.#sendRequest(async config => await axios.put(url, dataToSend, config), req.headers.cookie, res);
    }

    static async tryToGetAuthorizedUserName(req, res) {
        const response = await RequestSender.get(`${process.env.MPT_BACKEND_URL_FOR_NODE}/api/account/username`, req, res);
        return [response?.data?.name, response?.status ?? 500]; // TODO: Is it a good idea to set 500 code as default?
    }

    static async #sendRequest(requestFunc, cookieToSend, res) {
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
            
            res.redirect("/error");
        }
    }
}