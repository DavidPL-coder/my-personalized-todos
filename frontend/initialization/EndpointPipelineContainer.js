import RequestSender from "./RequestSender.js"

export default class EndpointPipelineContainer {

    static SERVER_URL = process.env.MPT_BACKEND_URL_FOR_NODE;

    static renderHtmlFile(res, fileName, renderVariables = {}) {
        const cookieValue = JSON.stringify(global.appConfig);
        const cookieOptions = { sameSite: "Lax"};
        const filePath = `${global.__dirname}/${fileName}`;

        res.cookie("app-config", cookieValue, cookieOptions);
        res.render(filePath, renderVariables);
    };

    static async register(req, res) {
        if (typeof (req.body.purposes) === "string")
            req.body.purposes = [ req.body.purposes ];

        const response = await RequestSender.post(`${this.SERVER_URL}/api/users`, req.body, req); 
        if (response?.status !== 200)
            res.redirect("/error");

        res.redirect("/login");
    }

    static async login(req, res) {
        req.body.login = req.body.login.trim();
        req.body.password = req.body.password.trim();

        const response = await RequestSender.post(`${this.SERVER_URL}/api/tokens`, req.body, req); 
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

    static async addToDo(req, res) {
        const dataToSend = this.#parseTodoData(req.body);
        const username = await RequestSender.getAuthorizedUserName(req);
        const response = await RequestSender.post(`${this.SERVER_URL}/api/users/${username}/todos`, dataToSend, req);
        if (response?.status !== 200)
            res.redirect("/error");

        res.redirect("/todos");
    }

    static async deleteToDo(req, res) {
        const username = await RequestSender.getAuthorizedUserName(req);
        const response = await RequestSender.delete(`${this.SERVER_URL}/api/users/${username}/todos/${req.params.todoTitle}`, req);
        if (response?.status !== 200)
            res.redirect("/error");

        res.redirect("/todos");
    }

    static async editToDo(req, res) {
        const dataToSend = this.#parseTodoData(req.body);
        const username = await RequestSender.getAuthorizedUserName(req);
        const response = await RequestSender.put(`${this.SERVER_URL}/api/users/${username}/todos/${req.params.todoTitle}`, dataToSend, req);
        if (response?.status !== 200)
            res.redirect("/error");

        res.redirect("/todos");
    }

    static async authorizedUserName(req, res) {
        const username = await RequestSender.getAuthorizedUserName(req);
        res.send(username);
    }

    static async editSettings(req, res) {
        req.body.italic = req.body.italic === "true";
        req.body.bold = req.body.bold === "true";
        req.body.uppercase = req.body.uppercase === "true";
        req.body.fontSize = parseInt(req.body.fontSize);

        const username = await RequestSender.getAuthorizedUserName(req);
        const response = await RequestSender.put(`${this.SERVER_URL}/api/users/${username}/settings`, req.body, req);
        if (response?.status !== 200)
            res.redirect("/error");

        res.redirect("/todos");
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