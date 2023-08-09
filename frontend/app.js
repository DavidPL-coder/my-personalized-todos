"use strict";

import express from "express";
import path from "path";
import { fileURLToPath } from "url";
import * as https from "https";
import EndpointPipelineContainer from "./initialization/EndpointPipelineContainer.js";
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

app.get("/", (req, res) => EndpointPipelineContainer.renderHtmlFile(res, "home.html"));
app.get("/login", (req, res) => EndpointPipelineContainer.renderHtmlFile(res, "login.html", { errorModalDisplayMode: "none" }));
app.get("/register", (req, res) => EndpointPipelineContainer.renderHtmlFile(res, "register.html", global.appConfig));
app.get("/error", (req, res) => EndpointPipelineContainer.renderHtmlFile(res, "error.html"));
app.get("/todos", (req, res) => EndpointPipelineContainer.renderHtmlFile(res, "todos.html"));

app.get("/authorized-user-name", async (req, res) => await EndpointPipelineContainer.authorizedUserName(req, res));

app.post("/login", async (req, res) => await EndpointPipelineContainer.login(req, res));
app.post("/register", async (req, res) => await EndpointPipelineContainer.register(req, res));
app.post("/add-todo", async (req, res) => await EndpointPipelineContainer.addToDo(req, res));
app.post("/delete-todo/:todoTitle", async (req, res) => await EndpointPipelineContainer.deleteToDo(req, res));
app.post("/edit-todo/:todoTitle", async (req, res) => await EndpointPipelineContainer.editToDo(req, res));

app.post("/edit-settings", async (req, res) => await EndpointPipelineContainer.editSettings(req, res));

// TODO: refactor it all below
https.globalAgent.options.rejectUnauthorized = false; // use it for development https config // ?????
const PORT = global.appConfig.FRONTEND_CONTAINER_PORT;

const shouldAppConfigureForHttps = process.env.MPT_APP_PROTOCOL === "https" && 
    process.env.MPT_CERT_FILE_NAME != undefined && process.env.MPT_KEY_FILE_NAME != undefined &&
    process.env.MPT_SSL_FILES_PATH != undefined;

const appRunner = shouldAppConfigureForHttps 
    ? https.createServer(HttpsSecurityOptionsProvider.getOptions(__filepath), app) 
    : app;

appRunner.listen(PORT, () => console.log(`My personalized todos app (frontend) listen to ${PORT} port (${process.env.MPT_APP_PROTOCOL}).`));