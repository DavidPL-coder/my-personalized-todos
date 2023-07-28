export default function toggleErroModalOverlayDisplay(displayMode) {
    const errorModalOverlay = document.querySelector("#error-modal-overlay");
    errorModalOverlay.style.display = displayMode;

    document.querySelector("#error-modal-close-button")
        .addEventListener("click", () => errorModalOverlay.style.display = "none");
}
