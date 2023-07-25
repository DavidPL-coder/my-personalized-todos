"use strict";

class UserCockpitInitializator {
    // TODO: take constants from config.
    #cancelTextContent;
    #authUserNameUrl;
    #siteTitle;
    #noTodosInfo;
    #getTodosUrl;
    #modalTitleInUpdateMode;
    #modalTitleInAddMode;
    #deleteTodoUrl;
    #updateTodoUrl;
    #addTodoUrl;
    #boxShadowOfTodoToDelete;
    #transformOfTodoToDelete;
    #todoForm;
    #panelRemoveBtn;
    #getSettingsUrl;
    #settingsColorInputsWithDefaults;

    constructor() {
        const SERVER_URL = "http://ec2-52-57-252-68.eu-central-1.compute.amazonaws.com:8080";

        this.#cancelTextContent = "Anuluj";
        this.#authUserNameUrl = "http://ec2-52-57-252-68.eu-central-1.compute.amazonaws.com/authorized-user-name";
        this.#siteTitle = username => "Lista user'a " + username;
        this.#noTodosInfo = "Brak zadań.";
        this.#getTodosUrl = username => `${SERVER_URL}/api/users/${username}/todos`;
        this.#modalTitleInUpdateMode = "Edycja Twojego ToDo";
        this.#modalTitleInAddMode = "Dodawanie Nowego ToDo";
        this.#deleteTodoUrl = todoTitle => `http://ec2-52-57-252-68.eu-central-1.compute.amazonaws.com/delete-todo/${todoTitle}`;
        this.#updateTodoUrl = todoTitle => `http://ec2-52-57-252-68.eu-central-1.compute.amazonaws.com/edit-todo/${todoTitle}`;
        this.#addTodoUrl = "http://ec2-52-57-252-68.eu-central-1.compute.amazonaws.com/add-todo";
        this.#boxShadowOfTodoToDelete = "0px 0px 7px 1px darkred";
        this.#transformOfTodoToDelete = "scale(1.03)";
        this.#todoForm = document.querySelector("#todo-form");
        this.#panelRemoveBtn = document.querySelector("#panel-remove-button");
        this.#getSettingsUrl = username => `${SERVER_URL}/api/users/${username}/settings`;
        this.#settingsColorInputsWithDefaults = [
            { input: document.querySelector("#text-color-input"), defaultValue: "#ffffff" },
            { input: document.querySelector("#background-color-input"), defaultValue: "#222930"},
            { input: document.querySelector("#header-color-input"), defaultValue: "#3199e3" }
        ];
    }

    async init() {
        await this.#setUsernameInStorage();
        this.#setSiteTitle();
        await this.#loadToDos();
        this.#setEventForModalCloseButton();
        this.#enableAutoResizeForModalDescriptionInput();
        this.#setEventForAddButton();
        this.#setEventForClickOnTodo();
        this.#setEventForPanelRemoveButton();
        this.#setValidationForRequiredInputs();
        this.#setParsingForSettings();
        this.#setEventForUserSettingsButton();
        await this.#loadSettingsOnSite();
    }

    async #setUsernameInStorage() {
        const response = await fetch(this.#authUserNameUrl);
        const username = await response.text();
        sessionStorage.setItem("username", username);
    }

    #setSiteTitle() {
        const username = sessionStorage.getItem("username");
        document.querySelector("#site-title").textContent = this.#siteTitle(username);
    }

    async #loadToDos() {
        const todosData = await this.#getToDosData();
        const todoStateInfo = document.querySelector("#todos-state-info");

        if (todosData.length === 0) {
            todoStateInfo.textContent = this.#noTodosInfo;
            return;
        }

        todoStateInfo.style.display = "none";
        todosData.forEach(todoData => this.#addToDoInContainer(todoData));
    }

    #setEventForModalCloseButton() {
        const closeButton = document.querySelector("#todo-modal-close-button");

        closeButton.addEventListener("click", () => {
            document.querySelector("#todo-modal-overlay").style.display = "none";
            this.#setInputsValues(null);
            document.querySelector("#title-validation-control").style.display = "none";
        });
    }

    #enableAutoResizeForModalDescriptionInput() {
        const descriptionInput = document.querySelector("#description-input");
        const defaultHeight = getComputedStyle(descriptionInput).height.slice(0, -2) - 2; // 2 is border width (top + bottom)

        descriptionInput.addEventListener("input", () => {
            descriptionInput.style.height = "auto";
            descriptionInput.style.height = (descriptionInput.scrollHeight > defaultHeight ? descriptionInput.scrollHeight : defaultHeight) + "px";
        });
    }

    #setEventForAddButton() {
        const addButton = document.querySelector("#panel-add-button");
        addButton.addEventListener("click", () => this.#initTodoModalObjects(false));
    }

    #setEventForClickOnTodo() {
        const todoCards = document.querySelectorAll(".todo-card");

        for (const todoCard of todoCards) {
            todoCard.addEventListener("click", async () => {
                if (this.#panelRemoveBtn.textContent == this.#cancelTextContent)
                    return;

                const todoTitle = todoCard.querySelector(".todo-title").textContent;
                const DataOftodos = await this.#getToDosData();
                const todoData = DataOftodos.find(t => t.title == todoTitle);

                this.#setInputsValues(todoData);
                this.#initTodoModalObjects(true, todoTitle);
            });
        }
    }

    #setEventForPanelRemoveButton() {
        const defaultTextContent = this.#panelRemoveBtn.textContent;

        this.#panelRemoveBtn.addEventListener("click", () => {
            if (this.#panelRemoveBtn.textContent == this.#cancelTextContent) {
                this.#toggleRemoveModeForCockpit(false, defaultTextContent);
                return;
            }

            this.#toggleRemoveModeForCockpit(true, this.#cancelTextContent);
        });
    }

    #setValidationForRequiredInputs() {
        const modalConfirmSubmitBtn = document.querySelector("#modal-confirm-submit-button");

        modalConfirmSubmitBtn.addEventListener("click", async event => {
            event.preventDefault();
            const titleInput = document.querySelector("#title-input");
            const convertedTitle = titleInput.value.trim();

            const isTitleValid = await this.#isTitleValid(convertedTitle);
            if (!isTitleValid) {
                document.querySelector("#title-validation-control").style.display = "flex";
                return;
            }

            titleInput.value = convertedTitle;
            this.#todoForm.submit();
        });
    }

    #setParsingForSettings() {
        const settingsForm = document.querySelector("#settings-form");

        settingsForm.addEventListener("formdata", event => {
            const settingsCheckboxes = document.querySelectorAll(".settings-checkbox");
            settingsCheckboxes.forEach(box => event.formData.set(box.name, box.checked));

            for (const inputDto of this.#settingsColorInputsWithDefaults) {
                let convertedColorValue = inputDto.input.value.trim();

                if (convertedColorValue[0] != "#")
                    convertedColorValue = "#" + convertedColorValue;

                if (/^#[0-9A-Fa-f]{3,6}$/g.test(convertedColorValue) == false)
                    convertedColorValue = inputDto.defaultValue; // TODO: display error instead of it

                event.formData.set(inputDto.input.name, convertedColorValue);
            }
        });
    }

    #setEventForUserSettingsButton() {
        const userSettingsButton = document.querySelector("#user-settings-button");

        userSettingsButton.addEventListener("click", () => {
            const settings = document.querySelector("#settings");
            settings.style.display = getComputedStyle(settings).display == "none" ? "inline-block" : null; // TODO: instead of null, just remove style attribute
        });
    }

    async #loadSettingsOnSite() {
        // TODO: refactor it.
        const username = sessionStorage.getItem("username");
        const response = await fetch(this.#getSettingsUrl(username));
        const settings = await response.json();

        const titles = document.querySelectorAll(".todo-title");
        const descriptions = document.querySelectorAll(".todo-description");
        this.#setSettingsInputsValues(settings);

        if (settings.italic) {
            titles.forEach(title => title.style.fontStyle = "italic");
            descriptions.forEach(description => description.style.fontStyle = "italic");
        }

        if (settings.bold)
            descriptions.forEach(description => description.style.fontWeight  = "bold");

        if (settings.uppercase) {
            titles.forEach(title => title.style.textTransform = "uppercase");
            descriptions.forEach(description => description.style.textTransform = "uppercase");
        }

        descriptions.forEach(description => description.style.color = settings.textColor);
        document.querySelectorAll(".todo-card").forEach(card => card.style.backgroundColor = settings.backgroundColor);
        document.querySelectorAll(".todo-header").forEach(header => header.style.backgroundColor = settings.headerColor);

        if (settings.fontSize == "0")
            descriptions.forEach(description => description.style.fontSize = "16px");

        else if (settings.fontSize == "1")
            descriptions.forEach(description => description.style.fontSize = "19px");

        else if (settings.fontSize == "2")
            descriptions.forEach(description => description.style.fontSize = "22px");
    }

    async #getToDosData() {
        const username = sessionStorage.getItem("username");
        const response = await fetch(this.#getTodosUrl(username));
        return await response.json();
    }

    #setInputsValues(todoData) {
        document.querySelector("#title-input").value = todoData?.title ?? "";
        document.querySelector("#description-input").textContent = todoData?.description;
        document.querySelector("#task-start-input").value = todoData?.taskStart;
        document.querySelector("#task-end-input").value = todoData?.taskEnd;
    }

    #initTodoModalObjects(hasUpdateMode, todoTitle) {
        const submitRemoveBtn = document.querySelector("#modal-remove-submit-btn");
        document.querySelector("#todo-modal-overlay").style.display = "flex";
        document.querySelector("#todo-modal-title").textContent = hasUpdateMode ? this.#modalTitleInUpdateMode : this.#modalTitleInAddMode;
        submitRemoveBtn.style.display = hasUpdateMode ? "flex" : "none";

        if (hasUpdateMode)
            submitRemoveBtn.addEventListener("click", () => this.#todoForm.action = this.#deleteTodoUrl(todoTitle));

        this.#todoForm.action = hasUpdateMode ? this.#updateTodoUrl(todoTitle) : this.#addTodoUrl;
        this.#todoForm.dataset.hasUpdateMode = hasUpdateMode;
    }

    #toggleRemoveModeForCockpit(hasRemoveMode, panelRemoveBtnTextContent) {
        const todoRemoveButtons = document.querySelectorAll(".todo-remove-btn");
        todoRemoveButtons.forEach(btn => btn.style.display = hasRemoveMode ? "inline" : "none");
        this.#panelRemoveBtn.textContent = panelRemoveBtnTextContent;

        const todoOnMouseOver = todoCard => {
            return () => {
                todoCard.style["box-shadow"] = this.#boxShadowOfTodoToDelete;
                todoCard.style.transform = this.#transformOfTodoToDelete;
            };
        };
        const todoOnMouseLeave = todoCard => { 
            return () => todoCard.removeAttribute("style"); 
        };

        const todoCards = document.querySelectorAll(".todo-card");
        todoCards.forEach(t => {
            t.onmouseover = hasRemoveMode ? todoOnMouseOver(t) : null;
            t.onmouseleave = hasRemoveMode ? todoOnMouseLeave(t) : null;
        });
    }

    async #isTitleValid(convertedTitle) {
        if (convertedTitle == "")
            return false;

        if (this.#todoForm.dataset.hasUpdateMode == "true")
            return true;
        
        const todosData = await this.#getToDosData();
        const isTitleUnique = todosData.map(t => t.title).every(title => title !== convertedTitle); // TODO: add filters options to todos on server side for example: https://localhost:4443/api/users/${username}/todos?fields=title
        return isTitleUnique;
    }

    #setSettingsInputsValues(settings) {
        document.querySelector("#italic-checkbox").checked = settings.italic;
        document.querySelector("#bold-checkbox").checked = settings.bold;
        document.querySelector("#uppercase-checkbox").checked = settings.uppercase;

        document.querySelector("#text-color-input").value = settings.textColor;
        document.querySelector("#background-color-input").value = settings.backgroundColor;
        document.querySelector("#header-color-input").value = settings.headerColor;

        document.querySelector("#font-size-input").value = settings.fontSize;
    }

    #addToDoInContainer(todoData) {
        const todoTitle = document.createElement("span");
        todoTitle.classList.add("todo-title");
        todoTitle.textContent = todoData.title;
        
        const todoRemoveBtn = document.createElement("input");
        todoRemoveBtn.type = "submit";
        todoRemoveBtn.value = "\u00D7";
        todoRemoveBtn.classList.add("todo-remove-btn");
        
        const todoRemoveForm = document.createElement("form");
        todoRemoveForm.method = "post";
        todoRemoveForm.action = this.#deleteTodoUrl(todoData.title);
        todoRemoveForm.append(todoRemoveBtn);
        
        const todoHeader = document.createElement("div");
        todoHeader.classList.add("todo-header");
        todoHeader.append(todoTitle);
        todoHeader.append(todoRemoveForm);
        
        const todoDescription = document.createElement("div");
        todoDescription.classList.add("todo-description");
        todoDescription.textContent = todoData.description;
        
        const todoCard = document.createElement("div");
        todoCard.classList.add("todo-card");
        todoCard.append(todoHeader);
        todoCard.append(todoDescription);
        
        const todoCardContainer = document.querySelector("#todo-container");
        todoCardContainer.append(todoCard);
    }
}

const initializator = new UserCockpitInitializator();
initializator.init();