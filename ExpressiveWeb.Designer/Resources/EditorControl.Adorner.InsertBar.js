class AdornerInsertBar extends HTMLElement {
    constructor(manager, attachedElement) {
        super();
        this._manager = manager;
        this._attachedElement = attachedElement;
        let rect = this._attachedElement.getBoundingClientRect();
        this.className = "adorner-insertbar";
        this.style.boxSizing = "border-box";
        this.style.left = (rect.left + window.scrollX) + "px";
        this.style.top = (rect.bottom + window.scrollY) + "px";
        this.style.width = rect.width + "px";
        this.drawActionsMenuButton();
    }
    connectedCallback() { }
    show() {
        this.style.display = "block";
    }
    hide() {
        this.style.display = "none";
    }
    drawActionsMenuButton() {
        const actionsMenu = document.createElement("button");
        actionsMenu.className = "actions-menu-button";
        actionsMenu.innerHTML = ">";
        this.appendChild(actionsMenu);
        actionsMenu.addEventListener("mousedown", (e) => {
            e.preventDefault();
        });
        actionsMenu.addEventListener("click", (e) => {
            const rect = e.target.closest("button.actions-menu-button").getBoundingClientRect();
            const json = JSON.stringify(this._manager.parentEditor.getElementInfo(this._attachedElement));
            $HOST_INTEROP.insertBarActionMenuOpening(json, rect.right + 1, rect.top);
            e.preventDefault();
        });
    }
}
customElements.define('adorner-insertbar', AdornerInsertBar);
//# sourceMappingURL=EditorControl.Adorner.InsertBar.js.map