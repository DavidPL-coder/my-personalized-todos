"use strict";

class RegisterFormValidator {
    #minLoginLength;
    #minPasswordLength;
    #minAge;
    #maxAge;
    #validationRules;
    #planingCheckbox;
    #funCheckbox;
    #otherCheckbox;

    constructor() {
        // TODO: Take constants from config file.
        this.#minLoginLength = 4;
        this.#minPasswordLength = 6;
        this.#minAge = 5;
        this.#maxAge = 200;
        this.#validationRules = [];
        this.#planingCheckbox = document.querySelector("#planing-checkbox");
        this.#funCheckbox = document.querySelector("#fun-checkbox");
        this.#otherCheckbox = document.querySelector("#other-checkbox");
    }

    initValidator() {
        this.#setValidationRules();
        this.#setHidingOptionForPurposeCheckboxesValidationControl();
        this.#setValidationCheckBeforeSubmit();
        this.#setEventForModalCloseButton();
    }

    #setValidationRules() {
        const passwordInput = document.querySelector("#password-input");
        const hasWhitespaces = text => /\s/.test(text);

        this.#addValidationRule("login-input", "login-validation-control", input => input.value.length >= this.#minLoginLength && !hasWhitespaces(input.value));
        this.#addValidationRule("password-input", "password-validation-control", input =>  input.value.length >= this.#minPasswordLength && !hasWhitespaces(input.value));
        this.#addValidationRule("confirm-password-input", "confirm-password-validation-control", input => input.value === passwordInput.value);
        this.#addValidationRule("age-input", "age-validation-control", input => input.value >= this.#minAge && input.value <= this.#maxAge);
        this.#addValidationRule("date-input", "date-validation-control", input => input.value !== input.defaultValue);
        this.#addValidationRule("purposes-checkboxes", "purpose-of-use-validation-control", input => this.#planingCheckbox.checked || this.#funCheckbox.checked || this.#otherCheckbox.checked);
    }

    #setHidingOptionForPurposeCheckboxesValidationControl() {
        const purposeOfUseValidationControl = document.querySelector("#purpose-of-use-validation-control");

        this.#planingCheckbox.addEventListener("click", () => purposeOfUseValidationControl.style.display = "none");
        this.#funCheckbox.addEventListener("click", () => purposeOfUseValidationControl.style.display = "none");
        this.#otherCheckbox.addEventListener("click", () => purposeOfUseValidationControl.style.display = "none");
    }

    #setValidationCheckBeforeSubmit() {
        const submitButton = document.querySelector("#submit-button");
        submitButton.addEventListener("click", async event => {
            event.preventDefault();

            if (!this.#checkAllValidationRules())
                return;

            const statusCode = await this.#isUserWithGivenLoginExist();

            if (statusCode === 404)
                document.querySelector("#register-form").submit();
            else if (statusCode === 200) {
                const errorModalOverlay = document.querySelector("#error-modal-overlay");
                errorModalOverlay.style.display = "flex";
            }
            else
                window.location.replace("http://localhost/error"); // TODO: Get link from config or "global" variable
        });
    }

    #setEventForModalCloseButton() {
        const closeButton = document.querySelector("#error-modal-close-button");

        closeButton.addEventListener("click", () => {
            const errorModalOverlay = document.querySelector("#error-modal-overlay");
            errorModalOverlay.style.display = "none";
        });
    }

    #addValidationRule(inputId, validationControlId, predicate) {
        const input = document.querySelector(`#${inputId}`);
        const validationControl = document.querySelector(`#${validationControlId}`);
    
        input.addEventListener("focusout", () => {
            if (predicate(input))
                validationControl.style.display = "none";
            else
                validationControl.style.display = "block";
        });

        const validationRule = { input, validationControl, predicate };
        this.#validationRules.push(validationRule);
    }

    #checkAllValidationRules() {
        let isFormValid = true;

        for (const rule of this.#validationRules) {
            if (!rule.predicate(rule.input)) {
                isFormValid = false;
                rule.validationControl.style.display = "block";
            }
            else
                rule.validationControl.style.display = "none";
        }

        return isFormValid;
    }

    async #isUserWithGivenLoginExist() {
        try {
            const loginInputValue = document.querySelector("#login-input").value;
            const response = await fetch(`http://localhost:8080/api/users/${loginInputValue}`);
            return response.status;
        } 
        catch (error) {
            // TODO: Improve error handling
            if (error.response) {
                console.log("Server message:", error.response.data);
                console.log("Status code:", error.response.status);
                console.log("Data from request:", error.response.config);
            }
            else if (error.request)
                console.log("Request error:", error.request);
            else
                console.log("Unknown error:", error.message);

            return -1;
        }
    }
}

const formValidator = new RegisterFormValidator();
formValidator.initValidator();