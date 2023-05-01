"use strict";

import express from "express";
import path from "path";
import { fileURLToPath } from "url";
// import * as https from "https";
import EndpointPipelineContainer from "./initialization/EndpointPipelineContainer.js";
// import HttpsSecurityOptionsProvider from "./initialization/HttpsSecurityOptionsProvider.js";
// import ProjectPathGetter from "./initialization/ProjectPathGetter.js";
import ejs from "ejs";

// TODO: write unit tests.
// TODO: Take constants from config file don't use them in code.
// TODO: Some code should be execute in only development mode (for example: using self signed cert)

const __filename = fileURLToPath(import.meta.url);
const __filepath = path.dirname(__filename);
const __dirname = __filepath + "/public";

const app = express();
app.use(express.static(__dirname));
app.use(express.urlencoded({ extended: true }));

app.set("view engine", "html");
app.engine("html", ejs.renderFile);

app.get("/", (req, res) => res.sendFile(__dirname + "/index.html"));
app.get("/login", (req, res) => res.render(__dirname + "/login.html", { errorModalDisplayMode: "none" }));
app.get("/register", (req, res) => res.sendFile(__dirname + "/register.html"));
app.get("/error", (req, res) => res.sendFile(__dirname + "/error.html"));
app.get("/todos", (req, res) => res.sendFile(__dirname + "/todos.html"));

app.get("/authorized-user-name", async (req, res) => await EndpointPipelineContainer.authorizedUserName(req, res));

app.post("/login", async (req, res) => await EndpointPipelineContainer.login(req, res, __dirname));
app.post("/register", async (req, res) => await EndpointPipelineContainer.register(req, res));
app.post("/add-todo", async (req, res) => await EndpointPipelineContainer.addToDo(req, res));
app.post("/delete-todo/:todoTitle", async (req, res) => await EndpointPipelineContainer.deleteToDo(req, res));
app.post("/edit-todo/:todoTitle", async (req, res) => await EndpointPipelineContainer.editToDo(req, res));

app.post("/edit-settings", async (req, res) => await EndpointPipelineContainer.editSettings(req, res));

// https.globalAgent.options.rejectUnauthorized = false; // use it for development https config // ?????

/// https config:
// const __projectpath = ProjectPathGetter.getPath(__filepath, process.env.NODE_ENV);
// const httpsSecurityOptions = HttpsSecurityOptionsProvider.getOptions(__projectpath);
// https.createServer(httpsSecurityOptions, app)
//     .listen(443, () => console.log("My personalized todos app (frontend) listen to 443 port (https)."));

app.enable('trust proxy');
app.set('trust proxy', 1);

// for heroku production app use http insted of https
const PORT = process.env.PORT || 80;
app.listen(PORT, () => console.log(`My personalized todos app (frontend) listen to ${PORT} port.`));