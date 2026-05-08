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

// ── Viewed-note state persistence (localStorage) ──────────────────────────
// Keys are scoped per user so multiple accounts on the same browser stay isolated.

window.wreGetViewedBranches = function (userId) {
    try {
        var raw = localStorage.getItem('wre-viewed-branches:' + userId);
        return raw ? JSON.parse(raw) : [];
    } catch (e) { return []; }
};

window.wreSetViewedBranch = function (userId, branchId) {
    try {
        var storageKey = 'wre-viewed-branches:' + userId;
        var raw = localStorage.getItem(storageKey);
        var arr = raw ? JSON.parse(raw) : [];
        if (arr.indexOf(branchId) === -1) arr.push(branchId);
        // Cap at 2000 entries to prevent unbounded growth
        if (arr.length > 2000) arr = arr.slice(arr.length - 2000);
        localStorage.setItem(storageKey, JSON.stringify(arr));
    } catch (e) { }
};

window.wreGetViewedCells = function (userId) {
    try {
        var raw = localStorage.getItem('wre-viewed-cells:' + userId);
        return raw ? JSON.parse(raw) : [];
    } catch (e) { return []; }
};

window.wreSetViewedCell = function (userId, cellKey) {
    try {
        var storageKey = 'wre-viewed-cells:' + userId;
        var raw = localStorage.getItem(storageKey);
        var arr = raw ? JSON.parse(raw) : [];
        if (arr.indexOf(cellKey) === -1) arr.push(cellKey);
        // Cap at 5000 entries (employeeId:date pairs)
        if (arr.length > 5000) arr = arr.slice(arr.length - 5000);
        localStorage.setItem(storageKey, JSON.stringify(arr));
    } catch (e) { }
};
