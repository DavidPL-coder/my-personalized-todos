"use strict";

import express from "express";
import path from "path";
import { fileURLToPath } from "url";
import * as https from "https";
import Endpoints from "./initialization/EndpointPipelineContainer.js";
import HttpsSecurityOptionsProvider from "./initialization/HttpsSecurityOptionsProvider.js";
import AppConfigProvider from "./initialization/AppConfigProvider.js";
import ejs from "ejs";

// TODO: write unit tests.
// TODO: Some code should be execute in only development mode (for example: using self signed cert)

const __filename = fileURLToPath(import.meta.url);
const __filepath = path.dirname(__filename);
const __dirname = __filepath + "/public";
global.__dirname = __dirname;

const app = express();
app.use(express.static(__dirname));
app.use(express.urlencoded({ extended: true }));

app.set("view engine", "html");
app.engine("html", ejs.renderFile);

global.appConfig = AppConfigProvider.getConfig();

app.get("/", (req, res) => Endpoints.renderPage(res, "home.html"));
app.get("/login", (req, res) => Endpoints.renderPage(res, "login.html", { errorModalDisplayMode: "none" }));
app.get("/register", (req, res) => Endpoints.renderPage(res, "register.html", global.appConfig));
app.get("/todos", async (req, res) => await Endpoints.todos(req, res));

app.get("/error", (req, res) => Endpoints.renderPage(res, "error.html"));
app.get("/unauthorized", (req, res) => Endpoints.renderPage(res, "unauthorized.html"));
app.get("/authorized-user-name", async (req, res) => await Endpoints.authorizedUserName(req, res));

app.post("/register", async (req, res) => await Endpoints.register(req, res));
app.post("/login", async (req, res) => await Endpoints.login(req, res));
app.post("/logout", async (req, res) => await Endpoints.logout(req, res));
app.post("/add-todo", async (req, res) => await Endpoints.InvokeWithAuthorization(req, res, Endpoints.addToDo));
app.post("/delete-todo/:todoTitle", async (req, res) => await Endpoints.InvokeWithAuthorization(req, res, Endpoints.deleteToDo));
app.post("/edit-todo/:todoTitle", async (req, res) => await Endpoints.InvokeWithAuthorization(req, res, Endpoints.editToDo));
app.post("/edit-settings", async (req, res) => await Endpoints.InvokeWithAuthorization(req, res, Endpoints.editSettings));

// TODO: refactor it all below
if (process.env.MPT_APP_PROTOCOL === "https" && process.env.NODE_ENV === "Development")
    https.globalAgent.options.rejectUnauthorized = false;

const PORT = global.appConfig.FRONTEND_CONTAINER_PORT;

const shouldAppConfigureForHttps = process.env.MPT_APP_PROTOCOL === "https" && 
    process.env.MPT_CERT_FILE_NAME != undefined && process.env.MPT_KEY_FILE_NAME != undefined &&
    process.env.MPT_SSL_FILES_PATH != undefined;

const appRunner = shouldAppConfigureForHttps 
    ? https.createServer(HttpsSecurityOptionsProvider.getOptions(__filepath), app) 
    : app;

appRunner.listen(PORT, () => console.log(`My personalized todos app (frontend) listen to ${PORT} port (${process.env.MPT_APP_PROTOCOL}).`));