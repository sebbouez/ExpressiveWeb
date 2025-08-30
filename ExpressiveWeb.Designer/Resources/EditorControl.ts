class KitComponent {
    uid: string;
    allowsInlineEditing: boolean;
    hasContextualActions: boolean;
    selector: string;
    accepts: string[];
    denies: string[];
    slots: string[];
}

class ElementAttributeInfo {
    name: string;
    value: string
}

class ElementInfo {
    componentUid: string;
    tagName: string;
    cssClass: string;
    internalId: string;
    innerHtml: string;
    innerText: string;
    parentInternalId: string;
    index: number;
    parentChildrenCount: number;
    attributes: ElementAttributeInfo[];
}


class EditorComponent {

    private _throttleResizer: number | undefined;
    public adornerManager: AdornerManager;
    public domHelper: DomHelper;
    private _resizeObserver: ResizeObserver;
    public textEditor: TextEditor;


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

        //document.body.appendChild(this.adornerManager.adornerContainer);

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

    ensureInternalId(element: HTMLElement) {
        const self = this;

        if ((element as HTMLElement).dataset.internalId === undefined) {
            (element as HTMLElement).dataset.internalId = self.newUUID();
        }
    }

    ensureInternalIds(parentElement: HTMLElement | null = null) {

        if (parentElement == null) {
            parentElement = document.body;
        }

        const allChildren = parentElement.querySelectorAll($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);
        const self = this;
        allChildren.forEach(el => {
            self.ensureInternalId(el as HTMLElement);
        });
    }


    getAllElements(): NodeListOf<Element> {
        return document.querySelectorAll($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);
    }

    /**
     * Gets the closest Element that corresponds to a component from the specified Element
     * @param htmlElement
     */
    public getClosestComponentAsHtmlElement(htmlElement: HTMLElement): HTMLElement | null {

        const foundElement = htmlElement.parentElement.closest($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);

        if (foundElement == null) {
            return null;
        }

        return (foundElement as HTMLElement);
    }

    public selectParentElement(internalId: string): void {

        this.unSelectAll();

        const element = this.getElementByInternalId(internalId);
        const foundElement: HTMLElement = element.parentElement.closest($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);

        if (foundElement == null) {
            return null;
        }

        let parentInternalId = this.getElementInternalId(foundElement);
        this.selectElementByInternalId(parentInternalId);
    }

    public getClosestComponentInfoFromHtmlElement(htmlElement: HTMLElement): KitComponent | null {

        const foundElement = htmlElement.parentElement.closest($EDITOR_KIT_DATA.KNOWN_COMPONENTS_SELECTOR);

        if (foundElement == null) {
            return null;
        }

        return this.getComponentInfoFromHtmlElement(foundElement as HTMLElement);
    }

    public getComponentInfoFromDecorator(decorator: AdornerDecorator): KitComponent | null {
        const sourceElementId = decorator.getAttribute('data-source-element-id');
        const sourceElement = this.getElementByInternalId(sourceElementId);
        return this.getComponentInfoFromHtmlElement(sourceElement as HTMLElement);
    }

    public getComponentInfoFromHtmlElement(element: HTMLElement): KitComponent | null {

        let found = null;
        $EDITOR_KIT_DATA.COMPONENTS_CATALOG.some(comp => {
            if (element.matches(comp.selector)) {
                found = comp;
                return true;
            }

            if (!found && comp.slots.length > 0) {
                let r: boolean = false;

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

        })

        return found;
    }

    public getElementByInternalId(internalId: string): HTMLElement | null {
        return document.querySelector('[data-internal-id="' + internalId + '"]');
    }


    public getJsonElementInfoFromInternalId(internalId: string): string | null {
        const element = this.getElementByInternalId(internalId);
        if (element == null) {
            return null;
        }
        const info = this.getElementInfo(element as HTMLElement);

        return JSON.stringify(info);
    }

    public getElementInfo(element: HTMLElement): ElementInfo {
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

    /**
     * Gets the internal id of the provided element. This ensures that the element has a valid id.
     * @param {HTMLElement} element - source HTMLElement
     * @returns {string} Valid Editor Internal Id
     */
    public getElementInternalId(element: HTMLElement): string {

        let value: string = element.getAttribute('data-internal-id');
        if (!value) {
            value = this.newUUID();
            element.setAttribute('data-internal-id', value);
        }
        return value;
    }

    public getSourceElementFromDecorator(decorator: HTMLElement): HTMLElement {
        const sourceElementId = decorator.getAttribute('data-source-element-id');
        return this.getElementByInternalId(sourceElementId);
    }

    /**
     * Checks if the specified Element is known as a slot for a component
     * @param element
     */
    public isSlot(element: HTMLElement): boolean {

        let found: boolean = false;
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
        })

        return found;

    }

    newUUID(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            let r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    public selectElementByInternalId(internalId: string) {
        const element: HTMLElement = this.getElementByInternalId(internalId);
        this.handleSelection(element);
    }

    public unSelectAll(): void {
        this.adornerManager.unSelectAll();
        $HOST_INTEROP.raiseSelectedElementChanged("{}");
    }


    public handleSelection(element: HTMLElement) {

        if (!element) {
            console.error('handleSelection: Element not found');
            return;
        }

        this.adornerManager.selectDecoratorFromElement(element as HTMLElement);

        const info = this.getElementInfo(element as HTMLElement);
        $HOST_INTEROP.raiseSelectedElementChanged(JSON.stringify(info));
    }

    private setUpDocumentEvents(): void {
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

    private setUpMouseEvents(): void {

        let self = this;


    }
}

//customElements.define('editor-area', EditorComponent);
