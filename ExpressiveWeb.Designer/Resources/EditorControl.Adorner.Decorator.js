class AdornerDecorator extends HTMLElement {
    constructor(manager, attachedElement) {
        super();
        this._resizeObserver = null;
        this._inflateValue = 0;
        this._insertBar = null;
        this.chevronRightIcon = "<svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' version='1.1' baseProfile='full' width='10' height='10' viewBox='0 0 10.56 18.00' xml:space='preserve'><path fill='#000000' fill-opacity='1' stroke-width='0.666699' stroke-linejoin='round' d='M 2.59497,0.435019L 10.0798,7.92C 10.3999,8.23998 10.5598,8.59249 10.5598,8.9775C 10.5598,9.36251 10.3999,9.71502 10.0798,10.035L 2.59497,17.52C 2.2749,17.84 1.90747,18 1.49243,18C 1.07739,18 0.724854,17.8563 0.434814,17.5688C 0.14502,17.2812 0,16.93 0,16.515C 0,16.1 0.159912,15.7325 0.47998,15.4125L 6.91504,8.9775L 0.47998,2.54253C 0.0349121,2.16253 -0.107666,1.68377 0.0523682,1.10625C 0.212402,0.528769 0.563721,0.159994 1.1062,-9.53674e-006C 1.64868,-0.159983 2.14502,-0.0149632 2.59497,0.435019 Z '/></svg>";
        this._manager = manager;
        this._attachedElement = attachedElement;
        let rect = this._attachedElement.getBoundingClientRect();
        this.className = "adorner-decorator";
        if (this._manager.parentEditor.isSlot(this._attachedElement)) {
            this.draggable = false;
            this.classList.add("slot-decorator");
        }
        else {
            this.draggable = true;
        }
        this.style.boxSizing = "border-box";
        this.style.left = (rect.left + window.scrollX) + "px";
        this.style.top = (rect.top + window.scrollY) + "px";
        this.style.width = rect.width + "px";
        this.style.height = rect.height + "px";
        this.drawActionsMenuButton();
        this.dataset.sourceElementId = this._attachedElement.dataset.internalId;
    }
    connectedCallback() {
    }
    disconnectedCallback() {
        if (this._resizeObserver) {
            this._resizeObserver.disconnect();
            this._resizeObserver = null;
        }
    }
    attachInsertBar(bar) {
        this._insertBar = bar;
    }
    disable() {
        this.classList.add("state-disabled");
    }
    enable() {
        this.classList.remove("state-disabled");
    }
    select() {
        this.classList.add("state-active");
        this.drawMarginsDecorations();
        this.showInsertBar();
        this.activateDeepestDecorator();
    }
    activateDeepestDecorator() {
        const deepestParent = this._manager.getDeepestElement(this._attachedElement);
        if (deepestParent) {
            const decorator = this._manager.getDecoratorFromElement(deepestParent);
            if (decorator) {
                decorator.showInsertBar();
            }
        }
    }
    deActivateDeepestDecorator() {
        const deepestParent = this._manager.getDeepestElement(this._attachedElement);
        if (deepestParent) {
            const decorator = this._manager.getDecoratorFromElement(deepestParent);
            if (decorator) {
                decorator.hideInsertBar();
            }
        }
    }
    showInsertBar() {
        if (this._insertBar) {
            this._insertBar.show();
        }
    }
    hideInsertBar() {
        if (this._insertBar) {
            this._insertBar.hide();
        }
    }
    drawActionsMenuButton() {
        const component = this._manager.parentEditor.getComponentInfoFromHtmlElement(this._attachedElement);
        if (!component) {
            return;
        }
        if (component.hasContextualActions) {
            const actionsMenu = document.createElement("button");
            actionsMenu.className = "actions-menu-button";
            actionsMenu.innerHTML = this.chevronRightIcon;
            this.appendChild(actionsMenu);
            actionsMenu.addEventListener("mousedown", (e) => {
                e.preventDefault();
            });
            actionsMenu.addEventListener("click", (e) => {
                const rect = e.target.closest("button.actions-menu-button").getBoundingClientRect();
                $HOST_INTEROP.componentActionMenuOpening(rect.right + 1, rect.top);
                e.preventDefault();
            });
        }
    }
    startTextEditingMode() {
        this.clearMarginsDecorations();
        this.inflate(2);
        this.drawTextEditingDecorations();
        this._attachedElement.setAttribute("contenteditable", "true");
        this.classList.remove("state-active");
        this.classList.add("state-text-editing");
        this._resizeObserver = new ResizeObserver(entries => {
            let rect = this._attachedElement.getBoundingClientRect();
            this.style.width = rect.width + (this._inflateValue * 2) + "px";
            this.style.height = rect.height + (this._inflateValue * 2) + "px";
            this.style.left = (rect.left + window.scrollX - this._inflateValue) + "px";
            this.style.top = (rect.top + window.scrollY - this._inflateValue) + "px";
        });
        this._resizeObserver.observe(this._attachedElement);
        this._attachedElement.focus();
    }
    stopTextEditingMode() {
        this.inflate(0);
        this.clearEditingDecorations();
        this._attachedElement.removeAttribute("contenteditable");
        this.classList.remove("state-text-editing");
        if (this._resizeObserver) {
            this._resizeObserver.disconnect();
            this._resizeObserver = null;
        }
    }
    unSelect() {
        this.classList.remove("state-active");
        this.clearMarginsDecorations();
        this.hideInsertBar();
        this.deActivateDeepestDecorator();
    }
    clearEditingDecorations() {
        const allMarginsDecorations = this.querySelectorAll(".adorner-editing-border-decoration");
        allMarginsDecorations.forEach(x => {
            this.removeChild(x);
        });
    }
    clearMarginsDecorations() {
        const allMarginsDecorations = this.querySelectorAll(".adorner-margin-decoration, .adorner-padding-decoration");
        allMarginsDecorations.forEach(x => {
            this.removeChild(x);
        });
    }
    drawMarginsDecorations() {
        const style = window.getComputedStyle(this._attachedElement);
        const paddings = {
            top: style.paddingTop,
            right: style.paddingRight,
            bottom: style.paddingBottom,
            left: style.paddingLeft
        };
        const margins = {
            top: style.marginTop,
            right: style.marginRight,
            bottom: style.marginBottom,
            left: style.marginLeft
        };
        if (margins.top != "0px" && margins.top != "0px") {
            const topMargin = document.createElement("div");
            topMargin.className = "adorner-margin-decoration";
            topMargin.style.bottom = "100%";
            topMargin.style.left = "0";
            topMargin.style.width = "100%";
            topMargin.style.height = margins.top;
            this.appendChild(topMargin);
        }
        if (margins.bottom != "0px" && margins.bottom != "0px") {
            const bottomMargin = document.createElement("div");
            bottomMargin.className = "adorner-margin-decoration";
            bottomMargin.style.top = "100%";
            bottomMargin.style.left = "0";
            bottomMargin.style.width = "100%";
            bottomMargin.style.height = margins.bottom;
            this.appendChild(bottomMargin);
        }
        if (paddings.top != "0px" && paddings.top != "0px") {
            const paddingTop = document.createElement("div");
            paddingTop.className = "adorner-padding-decoration";
            paddingTop.style.top = "0px";
            paddingTop.style.left = "0";
            paddingTop.style.width = "100%";
            paddingTop.style.height = paddings.top;
            this.appendChild(paddingTop);
        }
        if (paddings.bottom != "0px" && paddings.bottom != "0px") {
            const paddingBottom = document.createElement("div");
            paddingBottom.className = "adorner-padding-decoration";
            paddingBottom.style.bottom = "0px";
            paddingBottom.style.left = "0";
            paddingBottom.style.width = "100%";
            paddingBottom.style.height = paddings.bottom;
            this.appendChild(paddingBottom);
        }
        if (paddings.left != "0px" && paddings.left != "0px") {
            const paddingTop = document.createElement("div");
            paddingTop.className = "adorner-padding-decoration";
            paddingTop.style.left = "0";
            paddingTop.style.top = "0";
            paddingTop.style.height = "100%";
            paddingTop.style.width = paddings.left;
            this.appendChild(paddingTop);
        }
        if (paddings.right != "0px" && paddings.right != "0px") {
            const paddingTop = document.createElement("div");
            paddingTop.className = "adorner-padding-decoration";
            paddingTop.style.right = "0";
            paddingTop.style.top = "0";
            paddingTop.style.height = "100%";
            paddingTop.style.width = paddings.right;
            this.appendChild(paddingTop);
        }
    }
    drawTextEditingDecorations() {
        const div1 = document.createElement("div");
        div1.className = "adorner-editing-border-decoration";
        div1.style.bottom = "100%";
        div1.style.left = "-4px";
        div1.style.right = "-4px";
        div1.style.height = "4px";
        this.appendChild(div1);
        const div2 = document.createElement("div");
        div2.className = "adorner-editing-border-decoration";
        div2.style.left = "100%";
        div2.style.top = "-4px";
        div2.style.bottom = "-4px";
        div2.style.width = "4px";
        this.appendChild(div2);
        const div3 = document.createElement("div");
        div3.className = "adorner-editing-border-decoration";
        div3.style.top = "100%";
        div3.style.left = "0px";
        div3.style.right = "0px";
        div3.style.height = "4px";
        this.appendChild(div3);
        const div4 = document.createElement("div");
        div4.className = "adorner-editing-border-decoration";
        div4.style.right = "100%";
        div4.style.top = "-4px";
        div4.style.bottom = "-4px";
        div4.style.width = "4px";
        this.appendChild(div4);
    }
    inflate(value) {
        this._inflateValue = value;
        this.style.width = this.offsetWidth + (value * 2) + "px";
        this.style.height = this.offsetHeight + (value * 2) + "px";
        this.style.left = (this.offsetLeft - value) + "px";
        this.style.top = (this.offsetTop - value) + "px";
    }
}
customElements.define('adorner-decorator', AdornerDecorator);
//# sourceMappingURL=EditorControl.Adorner.Decorator.js.map