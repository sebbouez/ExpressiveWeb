class DragDropInfo {
}
class AdornerManager extends HTMLElement {
    constructor(owner) {
        super();
        this._verticalGuide = null;
        this._horizontalGuide = null;
        this._textEditingDecorator = null;
        this.parentEditor = owner;
        const shadow = this.attachShadow({ mode: 'open' });
        shadow.innerHTML = "<link rel='stylesheet' href='pfinternal://f/editorstyle.css'>";
        this._adornerContainer = document.createElement("div");
        this._adornerContainer.id = "decorators-container";
        this._adornerContainer.style.zIndex = "1000";
        shadow.appendChild(this._adornerContainer);
        this.setUpMouseEvents();
    }
    clearDecorators() {
        const allDecorators = this._adornerContainer.querySelectorAll(".adorner-decorator");
        allDecorators.forEach(decorator => {
            this._adornerContainer.removeChild(decorator);
        });
        const allInsertBars = this._adornerContainer.querySelectorAll(".adorner-insertbar");
        allInsertBars.forEach(insertBar => {
            this._adornerContainer.removeChild(insertBar);
        });
    }
    clearGuides() {
        this.clearVerticalGuide();
        this.clearHorizontalGuide();
    }
    clearHorizontalGuide() {
        if (this._horizontalGuide) {
            this._adornerContainer.removeChild(this._horizontalGuide);
            this._horizontalGuide = null;
        }
    }
    clearVerticalGuide() {
        if (this._verticalGuide) {
            this._adornerContainer.removeChild(this._verticalGuide);
            this._verticalGuide = null;
        }
    }
    removeDecorators(except) {
        const allDecorators = this._adornerContainer.querySelectorAll(".adorner-decorator");
        allDecorators.forEach(d => {
            if (d != except) {
                this._adornerContainer.removeChild(d);
            }
        });
    }
    startDragDecoratorHandler(owner, e) {
        if (e.ctrlKey) {
            e.dataTransfer.dropEffect = "copy";
        }
        else {
            e.dataTransfer.dropEffect = "move";
        }
        let componentInfo = owner.parentEditor.getComponentInfoFromDecorator(e.target);
        owner._dragInfo = new DragDropInfo();
        owner._dragInfo.sourceElement = owner.parentEditor.getSourceElementFromDecorator(e.target);
        owner._dragInfo.sourceDecorator = e.target;
        owner._dragInfo.sourceComponentInfo = componentInfo;
    }
    startTextEditMode(internalId) {
        const element = this.parentEditor.getElementByInternalId(internalId);
        const decorator = this.getDecoratorFromElement(element);
        document.normalize();
        this.parentEditor.textEditor.attach(element);
        this.discardEvents();
        this.removeDecorators(decorator);
        this._textEditingDecorator = decorator;
        decorator.startTextEditingMode();
    }
    exitTextEditMode() {
        document.normalize();
        this.restoreEvents();
        this.parentEditor.textEditor.detach();
        this._textEditingDecorator.stopTextEditingMode();
        this._textEditingDecorator = null;
        this.updateDecorators();
    }
    clearDragStates() {
        this._adornerContainer.querySelectorAll(".state-drag-after, .state-drag-before, .state-drag-inside").forEach(x => {
            x.classList.remove("state-drag-after");
            x.classList.remove("state-drag-before");
            x.classList.remove("state-drag-inside");
        });
    }
    dragOverDecoratorHandler(owner, e) {
        if (!owner._dragInfo) {
            return;
        }
        const targetComponentInfo = owner.parentEditor.getComponentInfoFromDecorator(e.target);
        owner._dragInfo.targetElement = owner.parentEditor.getSourceElementFromDecorator(e.target);
        owner._dragInfo.targetDecorator = e.target;
        owner._dragInfo.targetComponentInfo = targetComponentInfo;
        this.clearDragStates();
        if (owner.parentEditor.getElementInternalId(owner._dragInfo.sourceElement) ==
            owner.parentEditor.getElementInternalId(owner._dragInfo.targetElement)) {
            e.dataTransfer.dropEffect = "none";
            owner._dragInfo.targetElement = null;
            return;
        }
        let allowDropIntoTarget = true;
        let allowDropIntoTargetParent = true;
        if (owner._dragInfo.targetComponentInfo.denies.some(x => x == owner._dragInfo.sourceComponentInfo.uid || x == "*") == true) {
            allowDropIntoTarget = false;
        }
        if (allowDropIntoTarget && owner._dragInfo.targetComponentInfo.accepts.some(x => x == owner._dragInfo.sourceComponentInfo.uid || x == "*") == false) {
            allowDropIntoTarget = false;
        }
        if (!allowDropIntoTarget) {
            let parentComponent = owner.parentEditor.getClosestComponentInfoFromHtmlElement(owner._dragInfo.targetElement);
            if (parentComponent.accepts.some(x => x == owner._dragInfo.sourceComponentInfo.uid || x == "*") == false) {
                allowDropIntoTargetParent = false;
            }
            if (parentComponent.denies.some(x => x == owner._dragInfo.sourceComponentInfo.uid || x == "*") == true) {
                allowDropIntoTargetParent = false;
            }
        }
        if (allowDropIntoTarget) {
            e.dataTransfer.dropEffect = "move";
            const targetElement = e.target;
            targetElement.classList.add("state-drag-inside");
            this._dragInfo.desiredPosition = 0;
            e.preventDefault();
        }
        else if (allowDropIntoTargetParent) {
            e.dataTransfer.dropEffect = "move";
            const targetElement = e.target;
            const rect = targetElement.getBoundingClientRect();
            const offsetY = e.clientY - rect.top;
            if (offsetY < rect.height / 2) {
                this._dragInfo.desiredPosition = -1;
                targetElement.classList.add("state-drag-before");
            }
            else {
                this._dragInfo.desiredPosition = 1;
                targetElement.classList.add("state-drag-after");
            }
            e.preventDefault();
        }
        else {
            e.dataTransfer.dropEffect = "none";
            this._dragInfo.targetElement = null;
        }
    }
    dropDecoratorHandler(owner, e) {
        if (this._dragInfo.sourceElement && this._dragInfo.targetElement) {
            const sourceElementInfo = this.parentEditor.getElementInfo(this._dragInfo.sourceElement);
            const targetElementInfo = this.parentEditor.getElementInfo(this._dragInfo.targetElement);
            $HOST_INTEROP.dropElement(JSON.stringify(sourceElementInfo), JSON.stringify(targetElementInfo), this._dragInfo.desiredPosition, e.ctrlKey);
            e.preventDefault();
        }
    }
    highlightDecorator(decorator) {
        decorator.classList.add("state-highlight");
    }
    selectDecorator(decorator) {
        decorator.select();
    }
    selectDecoratorFromElement(element) {
        const foundDecorator = this.getDecoratorFromElement(element);
        if (foundDecorator) {
            this.selectDecorator(foundDecorator);
        }
    }
    disableDecoratorFromElementInternalId(internalId) {
        const foundDecorator = this._adornerContainer.querySelector("adorner-decorator[data-source-element-id='" + internalId + "']");
        if (foundDecorator) {
            foundDecorator.disable();
        }
    }
    enableDecoratorFromElementInternalId(internalId) {
        const foundDecorator = this._adornerContainer.querySelector("adorner-decorator[data-source-element-id='" + internalId + "']");
        if (foundDecorator) {
            foundDecorator.enable();
        }
    }
    getDecoratorFromElement(element) {
        const internalId = element.getAttribute("data-internal-id");
        const foundDecorator = this._adornerContainer.querySelector("adorner-decorator[data-source-element-id='" + internalId + "']");
        return foundDecorator;
    }
    showHorizontalGuide(y) {
        if (!this._horizontalGuide) {
            this._horizontalGuide = document.createElement("div");
            this._horizontalGuide.style.position = "absolute";
            this._horizontalGuide.style.left = "0";
            this._horizontalGuide.style.width = document.body.scrollWidth + "px";
            this._horizontalGuide.style.borderTop = "1px dotted black";
            this._adornerContainer.appendChild(this._horizontalGuide);
        }
        this._horizontalGuide.style.top = y + "px";
    }
    showVerticalGuide(x) {
        if (!this._verticalGuide) {
            this._verticalGuide = document.createElement("div");
            this._verticalGuide.style.position = "absolute";
            this._verticalGuide.style.top = "0";
            this._verticalGuide.style.height = document.body.scrollHeight + "px";
            this._verticalGuide.style.borderLeft = "1px dotted black";
            this._adornerContainer.appendChild(this._verticalGuide);
        }
        this._verticalGuide.style.left = x + "px";
    }
    unSelectAll() {
        const allDecorators = this._adornerContainer.querySelectorAll("adorner-decorator.state-active");
        allDecorators.forEach(decorator => {
            decorator.unSelect();
        });
    }
    getDeepestElement(element) {
        let current = element;
        while (current && current.parentElement) {
            if (current.parentElement.tagName.toLowerCase() === 'main') {
                return current;
            }
            current = current.parentElement;
        }
        return null;
    }
    updateDecorators() {
        this.clearDecorators();
        const allVisualElements = document.querySelectorAll($EDITOR_KIT_DATA.ADORNER_DECORATORS_SELECTOR);
        const self = this;
        allVisualElements.forEach(el => {
            self.parentEditor.ensureInternalId(el);
            let decorator = new AdornerDecorator(self, el);
            if (decorator.draggable === true) {
                decorator.addEventListener("dragstart", function (e) {
                    self.startDragDecoratorHandler(self, e);
                }, false);
            }
            decorator.addEventListener("drop", function (e) {
                self.dropDecoratorHandler(self, e);
            }, false);
            decorator.addEventListener("dragover", function (e) {
                self.dragOverDecoratorHandler(self, e);
            }, false);
            decorator.addEventListener("dblclick", function (e) {
                self.dblClickDecoratorHandler(self, e);
            }, false);
            self._adornerContainer.appendChild(decorator);
            if (el.parentElement.tagName.toLowerCase() == "main") {
                let insertBar = new AdornerInsertBar(self, el);
                self._adornerContainer.appendChild(insertBar);
                decorator.attachInsertBar(insertBar);
            }
        });
    }
    discardEvents() {
        this._adornerContainer.style.pointerEvents = "none";
    }
    restoreEvents() {
        this._adornerContainer.style.pointerEvents = "auto";
    }
    dblClickDecoratorHandler(owner, e) {
        const element = this.parentEditor.getSourceElementFromDecorator(e.target);
        if (element) {
            const info = this.parentEditor.getElementInfo(element);
            const component = this.parentEditor.getComponentInfoFromDecorator(e.target);
            $HOST_INTEROP.raiseElementDblClick(JSON.stringify(info));
        }
    }
    handleMouseUp(owner, e) {
        if (this._dragInfo) {
            this._dragInfo = null;
        }
        this.clearDragStates();
    }
    handleMouseClick(owner, e) {
        if (e.defaultPrevented) {
            return;
        }
        if (e.target instanceof HTMLElement) {
            const closestEditable = e.target.closest('[contenteditable]');
            if (closestEditable) {
                return;
            }
        }
        this.unSelectAll();
        const info = owner.parentEditor.getElementInfo(this._mouseDownElement);
        $HOST_INTEROP.raiseElementClick(JSON.stringify(info));
    }
    handleMouseDown(event) {
        if (event.defaultPrevented) {
            return;
        }
        this.unSelectAll();
        if (event.target instanceof HTMLElement) {
            if (event.target.classList.contains('adorner-decorator')) {
                const sourceElementId = event.target.getAttribute('data-source-element-id');
                let sourceElement = document.querySelector('[data-internal-id="' + sourceElementId + '"]');
                if (sourceElement == null) {
                    return;
                }
                let component = this.parentEditor.getComponentInfoFromHtmlElement(sourceElement);
                if (this.parentEditor.isSlot(sourceElement)) {
                    sourceElement = this.parentEditor.getClosestComponentAsHtmlElement(sourceElement);
                }
                this.parentEditor.handleSelection(sourceElement);
            }
        }
    }
    setUpMouseEvents() {
        const self = this;
        this._adornerContainer.addEventListener("mouseup", function (e) {
            self.handleMouseUp(self, e);
        });
        this._adornerContainer.addEventListener("mousedown", function (e) {
            self._mouseDownElement = e.target;
            self.handleMouseDown(e);
        });
        this._adornerContainer.addEventListener("click", function (e) {
            if (e.target != self._mouseDownElement) {
                return;
            }
            self.handleMouseClick(self, e);
        });
        this._adornerContainer.addEventListener("contextmenu", function (e) {
            $HOST_INTEROP.contextMenuOpening(e.clientX, e.clientY);
        });
        document.addEventListener("contextmenu", function (e) {
            $HOST_INTEROP.contextMenuOpening(e.clientX, e.clientY);
        });
        document.addEventListener("mousedown", function (e) {
            self._mouseDownElement = e.target;
        });
        document.addEventListener("click", function (e) {
            if (e.target != self._mouseDownElement) {
                return;
            }
            if (e.target instanceof HTMLElement) {
                if (e.target.tagName.toLowerCase() == "adorner-layer") {
                    return;
                }
            }
            self.handleMouseClick(self, e);
        });
    }
}
customElements.define('adorner-layer', AdornerManager);
//# sourceMappingURL=EditorControl.Adorner.js.map