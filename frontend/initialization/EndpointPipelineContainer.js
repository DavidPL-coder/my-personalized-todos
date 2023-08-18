import RequestSender from "./RequestSender.js"

export default class EndpointPipelineContainer {

    static SERVER_URL = process.env.MPT_BACKEND_URL_FOR_NODE;

    static renderPage(res, fileName, renderVariables = {}) {
        const cookieValue = JSON.stringify(global.appConfig);
        const cookieOptions = { sameSite: "Lax" };
        const filePath = `${global.__dirname}/${fileName}`;

        res.cookie("app-config", cookieValue, cookieOptions);
        res.render(filePath, renderVariables);
    };

    static async todos(req, res) {
        const [_, statusCode] = await RequestSender.tryToGetAuthorizedUserName(req, res);

        if (statusCode === 200)
            this.renderPage(res, "todos.html");
        else if (statusCode === 401)
            res.redirect("/unauthorized");
        else
            res.redirect("/error");
    }

    static async register(req, res) {
        if (typeof (req.body.purposes) === "string")
            req.body.purposes = [ req.body.purposes ];

        const response = await RequestSender.post(`${this.SERVER_URL}/api/users`, req.body, req, res); 
        this.#tryToRedirect("/login", res, response?.status);
    }

    static async login(req, res) {
        req.body.login = req.body.login.trim();
        req.body.password = req.body.password.trim();

        const response = await RequestSender.post(`${this.SERVER_URL}/api/tokens`, req.body, req, res); 
        const statusCode = response?.status;
        
        if (statusCode == 200) {
            res.setHeader("set-cookie", response.headers["set-cookie"]);
            res.redirect("/todos");
        }
        else if (statusCode == 400)
            res.render(global.__dirname + "/login.html", { errorModalDisplayMode: "flex" });
        else
            res.redirect("/error");
    }

    static async logout(req, res) {
        const [_, statusCode] = await RequestSender.tryToGetAuthorizedUserName(req, res);
        if (statusCode === 401)
            return;

        const response = await RequestSender.delete(`${this.SERVER_URL}/api/tokens`, req, res);
        res.clearCookie(process.env.MPT_TOKEN_COOKIE_NAME);
        this.#tryToRedirect("/", res, response?.status);
    }

    static async InvokeWithAuthorization(req, res, endpointFunc) {
        const [username, statusCode] = await RequestSender.tryToGetAuthorizedUserName(req, res);
        if (statusCode === 401) {
            res.redirect("/unauthorized");
            return;
        }

        endpointFunc = endpointFunc.bind(this);
        const response = await endpointFunc(req, res, username);
        this.#tryToRedirect("/todos", res, response?.status);
    }

    static async addToDo(req, res, username) {
        const dataToSend = this.#parseTodoData(req.body);
        return await RequestSender.post(`${this.SERVER_URL}/api/users/${username}/todos`, dataToSend, req, res);
    }

    static async deleteToDo(req, res, username) {
        return await RequestSender.delete(`${this.SERVER_URL}/api/users/${username}/todos/${req.params.todoTitle}`, req, res);
    }

    static async editToDo(req, res, username) {
        const dataToSend = this.#parseTodoData(req.body);
        return await RequestSender.put(`${this.SERVER_URL}/api/users/${username}/todos/${req.params.todoTitle}`, dataToSend, req, res);
    }

    static async authorizedUserName(req, res) {
        const [username, statusCode] = await RequestSender.tryToGetAuthorizedUserName(req, res);
        res.status(statusCode).send(username);
    }

    static async editSettings(req, res, username) {
        req.body.italic = req.body.italic === "true";
        req.body.bold = req.body.bold === "true";
        req.body.uppercase = req.body.uppercase === "true";
        req.body.fontSize = parseInt(req.body.fontSize);

        return await RequestSender.put(`${this.SERVER_URL}/api/users/${username}/settings`, req.body, req, res);
    }

    static #tryToRedirect(url, res, statusCode) {
        if (statusCode !== 200) {
            res.redirect("/error");
            return;
        }

        res.redirect(url);
    }

    static #parseTodoData(data) {
        data.description = data.description.trim();

        for (const key in data) {
            if (data.hasOwnProperty(key) && data[key] == "")
                data[key] = null;
        }

        return data;
    }
}