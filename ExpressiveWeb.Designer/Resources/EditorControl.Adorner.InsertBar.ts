class AdornerInsertBar extends HTMLElement {

    private _manager: AdornerManager;
    private _attachedElement: HTMLElement;
    
    constructor(manager: AdornerManager, attachedElement: HTMLElement) {
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

    connectedCallback() {}
    
    
    public show(): void {
        this.style.display = "block";
    }
    
    public hide(): void {
        this.style.display = "none";
    }
    
    private drawActionsMenuButton(): void {
        const actionsMenu = document.createElement("button");
        actionsMenu.className = "actions-menu-button";
        actionsMenu.innerHTML = ">";

        this.appendChild(actionsMenu);

        actionsMenu.addEventListener("mousedown", (e): void => {
            e.preventDefault();
        })

        actionsMenu.addEventListener("click", (e): void => {
            const rect = (e.target as HTMLElement).closest("button.actions-menu-button").getBoundingClientRect();

            const json = JSON.stringify(this._manager.parentEditor.getElementInfo(this._attachedElement));
            
            $HOST_INTEROP.insertBarActionMenuOpening(json, rect.right + 1, rect.top);
            e.preventDefault();
        })
    }

}


customElements.define('adorner-insertbar', AdornerInsertBar);