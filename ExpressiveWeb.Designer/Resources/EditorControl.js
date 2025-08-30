class KitComponent {
}
class ElementAttributeInfo {
}
class ElementInfo {
}
class EditorComponent {
    constructor() {
        const style = document.createElement('style');
        style.textContent = `
          :host {
            position: relative;
          }
         
          
        `;
        this.adornerManager = new AdornerManager(this);
        this.domHelper = new DomHelper(this);
        this.textEditor = new TextEditor(this);
        document.body.appendChild(this.adornerManager);
        let self = this;
        this.setUpMouseEvents();
        this.setUpDocumentEvents();
        window.setTimeout(() => {
            this.adornerManager.updateDecorators();
            this.ensureInternalIds();
        });
    }
    disconnectedCallback() {
        if (this._resizeObserver) {
            this._resizeObserver.disconnect();
            this._resizeObserver = null;
        }
    }
    ensureInternalId(element) {
        const self = this;
        if (element.dataset.internalId === undefined) {
            element.dataset.internalId = self.newUUID();
        }
    }
    ensureInternalIds(parentElement = null) {
        if (parentElement == null) {
            parentElement = document.body;
        }
        const allChildren = parentElement.querySelectorAll($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);
        const self = this;
        allChildren.forEach(el => {
            self.ensureInternalId(el);
        });
    }
    getAllElements() {
        return document.querySelectorAll($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);
    }
    getClosestComponentAsHtmlElement(htmlElement) {
        const foundElement = htmlElement.parentElement.closest($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);
        if (foundElement == null) {
            return null;
        }
        return foundElement;
    }
    selectParentElement(internalId) {
        this.unSelectAll();
        const element = this.getElementByInternalId(internalId);
        const foundElement = element.parentElement.closest($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);
        if (foundElement == null) {
            return null;
        }
        let parentInternalId = this.getElementInternalId(foundElement);
        this.selectElementByInternalId(parentInternalId);
    }
    getClosestComponentInfoFromHtmlElement(htmlElement) {
        const foundElement = htmlElement.parentElement.closest($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);
        if (foundElement == null) {
            return null;
        }
        return this.getComponentInfoFromHtmlElement(foundElement);
    }
    getComponentInfoFromDecorator(decorator) {
        const sourceElementId = decorator.getAttribute('data-source-element-id');
        const sourceElement = this.getElementByInternalId(sourceElementId);
        return this.getComponentInfoFromHtmlElement(sourceElement);
    }
    getComponentInfoFromHtmlElement(element) {
        let found = null;
        $EDITOR_KIT_DATA.COMPONENTS_CATALOG.some(comp => {
            if (element.matches(comp.selector)) {
                found = comp;
                return true;
            }
            if (!found && comp.slots.length > 0) {
                let r = false;
                comp.slots.some(x => {
                    if (x.length > 0 && element.matches(x)) {
                        found = comp;
                        r = true;
                        return true;
                    }
                });
                if (r) {
                    return true;
                }
            }
        });
        return found;
    }
    getElementByInternalId(internalId) {
        return document.querySelector('[data-internal-id="' + internalId + '"]');
    }
    getJsonElementInfoFromInternalId(internalId) {
        const element = this.getElementByInternalId(internalId);
        if (element == null) {
            return null;
        }
        const info = this.getElementInfo(element);
        return JSON.stringify(info);
    }
    getElementInfo(element) {
        let result = new ElementInfo();
        result.tagName = element.tagName.toLowerCase();
        result.cssClass = element.className;
        result.componentUid = "";
        result.innerHtml = "";
        result.internalId = this.getElementInternalId(element);
        result.parentInternalId = this.getElementInternalId(element.parentElement);
        result.index = Array.from(element.parentElement.children).indexOf(element);
        result.parentChildrenCount = element.parentElement.children.length;
        const component = this.getComponentInfoFromHtmlElement(element);
        if (component) {
            result.componentUid = component.uid;
            result.innerHtml = element.innerHTML;
            result.innerText = element.innerText;
        }
        result.attributes = [];
        element.getAttributeNames().forEach(name => {
            result.attributes[name] = element.getAttribute(name);
        });
        return result;
    }
    getElementInternalId(element) {
        let value = element.getAttribute('data-internal-id');
        if (!value) {
            value = this.newUUID();
            element.setAttribute('data-internal-id', value);
        }
        return value;
    }
    getSourceElementFromDecorator(decorator) {
        const sourceElementId = decorator.getAttribute('data-source-element-id');
        return this.getElementByInternalId(sourceElementId);
    }
    isSlot(element) {
        let found = false;
        $EDITOR_KIT_DATA.COMPONENTS_CATALOG.some(comp => {
            if (comp.slots.length > 0) {
                comp.slots.some(x => {
                    if (x.length > 0 && element.matches(x)) {
                        found = true;
                        return true;
                    }
                });
                if (found) {
                    return true;
                }
            }
        });
        return found;
    }
    newUUID() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            let r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
    selectElementByInternalId(internalId) {
        const element = this.getElementByInternalId(internalId);
        this.handleSelection(element);
    }
    unSelectAll() {
        this.adornerManager.unSelectAll();
        $HOST_INTEROP.raiseSelectedElementChanged("{}");
    }
    handleSelection(element) {
        if (!element) {
            console.error('handleSelection: Element not found');
            return;
        }
        this.adornerManager.selectDecoratorFromElement(element);
        const info = this.getElementInfo(element);
        $HOST_INTEROP.raiseSelectedElementChanged(JSON.stringify(info));
    }
    setUpDocumentEvents() {
        let self = this;
        window.addEventListener("resize", function (e) {
            if (!self._throttleResizer) {
                self._throttleResizer = setTimeout(() => {
                    self._throttleResizer = undefined;
                    self.adornerManager.updateDecorators();
                }, 50);
            }
        }, false);
        window.addEventListener("scroll", function (e) {
            $HOST_INTEROP.raiseScroll();
        });
    }
    setUpMouseEvents() {
        let self = this;
    }
}
//# sourceMappingURL=EditorControl.js.map