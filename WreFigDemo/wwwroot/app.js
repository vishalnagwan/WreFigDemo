// Register Esc key handler for paint mode
window.registerEscKey = function (dotNetRef) {
    if (window._escHandler) {
        window.removeEventListener('keydown', window._escHandler);
    }
    window._escHandler = function (e) {
        if (e.key === 'Escape') {
            dotNetRef.invokeMethodAsync('OnEscapeKey');
        }
    };
    window.addEventListener('keydown', window._escHandler);
};
