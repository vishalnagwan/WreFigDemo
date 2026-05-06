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

// Register click-outside handler for the detail panel
window.registerPanelClickOutside = function (dotNetRef) {
    // Remove any existing handler first
    if (window._panelClickHandler) {
        document.removeEventListener('mousedown', window._panelClickHandler);
    }
    window._panelClickHandler = function (e) {
        var panel = document.querySelector('.detail-panel');
        if (panel && !panel.contains(e.target)) {
            dotNetRef.invokeMethodAsync('OnEscapeKey'); // reuse same callback
        }
    };
    // Use mousedown so it fires before click, preventing stale-DOM issues
    document.addEventListener('mousedown', window._panelClickHandler);
};

window.unregisterPanelClickOutside = function () {
    if (window._panelClickHandler) {
        document.removeEventListener('mousedown', window._panelClickHandler);
        window._panelClickHandler = null;
    }
};
