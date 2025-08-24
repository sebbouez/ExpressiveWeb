class DragDropInfo {

    sourceElement: HTMLElement;
    targetElement: HTMLElement;

    sourceDecorator: HTMLElement;
    targetDecorator: HTMLElement;

    sourceComponentInfo: KitComponent;
    targetComponentInfo: KitComponent;

    desiredPosition: number;
}

class AdornerManager extends HTMLElement {

    parentEditor: EditorComponent;
    private readonly _adornerContainer: HTMLDivElement;
    private _verticalGuide: HTMLDivElement | null = null;
    private _horizontalGuide: HTMLDivElement | null = null;
    private _dragInfo: DragDropInfo | null;
    private _textEditingDecorator: AdornerDecorator | null = null;
    private _mouseDownElement: HTMLElement | null;

    constructor(owner: EditorComponent) {

        super();
        
        this.parentEditor = owner;

        const shadow = this.attachShadow({mode: 'open'});

        shadow.innerHTML = "<link rel='stylesheet' href='pfinternal://f/editorstyle.css'>";

        this._adornerContainer = document.createElement("div");
        this._adornerContainer.id = "decorators-container";
        this._adornerContainer.style.position = "absolute";
        this._adornerContainer.style.width = "100%";
        this._adornerContainer.style.height = "100%";
        this._adornerContainer.style.zIndex = "1000";

        shadow.appendChild(this._adornerContainer);

        this.setUpMouseEvents();
    }

    /**
     * Removes all decorator elements from the `_adornerContainer`.
     *
     * @return {void} This method does not return a value.
     */
    public clearDecorators(): void {

        const allDecorators = this._adornerContainer.querySelectorAll(".adorner-decorator");
        allDecorators.forEach(decorator => {
            this._adornerContainer.removeChild(decorator);
        })
    }

    public clearGuides(): void {
        this.clearVerticalGuide();
        this.clearHorizontalGuide();
    }

    /**
     * Removes the horizontal guide element from the adorner container if it exists.
     *
     *  @return {void} This method does not return a value.
     */
    public clearHorizontalGuide(): void {
        if (this._horizontalGuide) {
            this._adornerContainer.removeChild(this._horizontalGuide);
            this._horizontalGuide = null;
        }
    }

    /**
     * Removes the vertical guide from the adorner container and clears its reference.
     *
     *  @return {void} Does not return a value.
     */
    public clearVerticalGuide(): void {
        if (this._verticalGuide) {
            this._adornerContainer.removeChild(this._verticalGuide);
            this._verticalGuide = null;
        }
    }

    public removeDecorators(except: AdornerDecorator | null): void {

        const allDecorators = this._adornerContainer.querySelectorAll(".adorner-decorator");
        allDecorators.forEach(d => {

            if (d != except) {
                this._adornerContainer.removeChild(d);
            }
        })

    }

    startDragDecoratorHandler(owner: AdornerManager, e: DragEvent): void {

        if (e.ctrlKey) {
            e.dataTransfer.dropEffect = "copy";
        } else {
            e.dataTransfer.dropEffect = "move";
        }


        let componentInfo = owner.parentEditor.getComponentInfoFromDecorator(e.target as AdornerDecorator);

        owner._dragInfo = new DragDropInfo();
        owner._dragInfo.sourceElement = owner.parentEditor.getSourceElementFromDecorator(e.target as AdornerDecorator);
        owner._dragInfo.sourceDecorator = e.target as HTMLElement;
        owner._dragInfo.sourceComponentInfo = componentInfo;

    }


    public startTextEditMode(internalId: string): void {

        const element = this.parentEditor.getElementByInternalId(internalId);
        const decorator = this.getDecoratorFromElement(element);

        document.normalize();

        this.parentEditor.textEditor.attach(element);

        this.discardEvents();
        this.removeDecorators(decorator);
        this._textEditingDecorator = decorator;
        decorator.startTextEditingMode();
    }

    public exitTextEditMode(): void {

        document.normalize();
        this.restoreEvents();

        this.parentEditor.textEditor.detach();

        this._textEditingDecorator.stopTextEditingMode();
        this._textEditingDecorator = null;

        this.updateDecorators();
    }

    clearDragStates(): void {

        this._adornerContainer.querySelectorAll(".state-drag-after, .state-drag-before, .state-drag-inside").forEach(x => {
            x.classList.remove("state-drag-after");
            x.classList.remove("state-drag-before");
            x.classList.remove("state-drag-inside");
        });

    }

    dragOverDecoratorHandler(owner: AdornerManager, e: DragEvent): void {

        if (!owner._dragInfo) {
            return;
        }

        const targetComponentInfo: KitComponent = owner.parentEditor.getComponentInfoFromDecorator(e.target as AdornerDecorator);

        owner._dragInfo.targetElement = owner.parentEditor.getSourceElementFromDecorator(e.target as HTMLElement);
        owner._dragInfo.targetDecorator = e.target as HTMLElement;
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

        // Does the hovered element refuses to receive the dragging element ?
        // (refusing is more important than accepting)
        if (owner._dragInfo.targetComponentInfo.denies.some(x => x == owner._dragInfo.sourceComponentInfo.uid || x == "*") == true) {
            allowDropIntoTarget = false;
        }

        // Does the hovered element accept to receive the dragging element ?
        if (allowDropIntoTarget && owner._dragInfo.targetComponentInfo.accepts.some(x => x == owner._dragInfo.sourceComponentInfo.uid || x == "*") == false) {
            // target area does not accept source element type
            allowDropIntoTarget = false;
        }


        // Check if we are allowed to drop into parent
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

            const targetElement = e.target as HTMLElement;

            targetElement.classList.add("state-drag-inside");

            this._dragInfo.desiredPosition = 0;

            e.preventDefault();

        } else if (allowDropIntoTargetParent) {
            e.dataTransfer.dropEffect = "move";

            const targetElement = e.target as HTMLElement;
            const rect = targetElement.getBoundingClientRect();
            const offsetY = e.clientY - rect.top;

            if (offsetY < rect.height / 2) {
                this._dragInfo.desiredPosition = -1;

                targetElement.classList.add("state-drag-before");
            } else {
                this._dragInfo.desiredPosition = 1;

                targetElement.classList.add("state-drag-after");
            }

            e.preventDefault();

        } else {
            e.dataTransfer.dropEffect = "none";
            this._dragInfo.targetElement = null;
        }

    }

    /**
     * Handles the drop event for a decorator.
     *
     * @param {AdornerManager} owner - The owner or manager responsible for the adorners.
     * @param {any} e - The event argument containing details about the drop event.
     * @return {void} This method does not return a value.
     */
    dropDecoratorHandler(owner: AdornerManager, e: any): void {

        if (this._dragInfo.sourceElement && this._dragInfo.targetElement) {

            const sourceElementInfo = this.parentEditor.getElementInfo(this._dragInfo.sourceElement);
            const targetElementInfo = this.parentEditor.getElementInfo(this._dragInfo.targetElement);

            $HOST_INTEROP.dropElement(JSON.stringify(sourceElementInfo), JSON.stringify(targetElementInfo), this._dragInfo.desiredPosition);

            e.preventDefault();
        }

    }

    public highlightDecorator(decorator: HTMLDivElement): void {
        decorator.classList.add("state-highlight");
    }

    /**
     * Visually selects a decorator
     * @param decorator
     */
    public selectDecorator(decorator: AdornerDecorator): void {
        decorator.select();
    }


    /**
     * Tries to visually select a decorator from its attached Element
     * @param element
     */
    public selectDecoratorFromElement(element: HTMLElement): void {
        const foundDecorator = this.getDecoratorFromElement(element);
        if (foundDecorator) {
            this.selectDecorator(foundDecorator as AdornerDecorator);
        }
    }

    public getDecoratorFromElement(element: HTMLElement): AdornerDecorator | null {
        const internalId = element.getAttribute("data-internal-id");
        const foundDecorator = this._adornerContainer.querySelector("adorner-decorator[data-source-element-id='" + internalId + "']");
        return foundDecorator as AdornerDecorator | null;
    }

    /**
     * Displays a horizontal guide at the specified vertical position on the screen.
     *
     * @param {number} y - The y-coordinate (in pixels) where the horizontal guide should be displayed.
     * @return {void} Does not return a value.
     */
    public showHorizontalGuide(y: number): void {

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

    /**
     * Displays a vertical guide at the specified horizontal position.
     *
     * @param {number} x - The x-coordinate (in pixels) where the vertical guide should be displayed.
     * @return {void} Does not return a value.
     */
    public showVerticalGuide(x: number): void {

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

    public unSelectAll(): void {

        const allDecorators = this._adornerContainer.querySelectorAll("adorner-decorator.state-active");
        allDecorators.forEach(decorator => {

            (decorator as AdornerDecorator).unSelect();
        })
    }

    /**
     * Updates all decorators of the area
     */
    public updateDecorators(): void {

        this.clearDecorators();

        const marginLeft = 0;
        const marginTop = 0;

        const allVisualElements = document.querySelectorAll($EDITOR_KIT_DATA.ADORNER_DECORATORS_SELECTOR);

        const self = this;
        allVisualElements.forEach(el => {

            self.parentEditor.ensureInternalId(el as HTMLElement);

            let decorator0 = new AdornerDecorator(self, el as HTMLElement);

            if (decorator0.draggable === true) {
                decorator0.addEventListener("dragstart", function (e) {
                    self.startDragDecoratorHandler(self, e);
                }, false);
            }

            decorator0.addEventListener("drop", function (e) {
                self.dropDecoratorHandler(self, e);
            }, false);

            decorator0.addEventListener("dragover", function (e) {
                self.dragOverDecoratorHandler(self, e);
            }, false);


            decorator0.addEventListener("dblclick", function (e) {
                self.dblClickDecoratorHandler(self, e);
            }, false);


            //if (rect.width > 0 && rect.height > 0) {
            self._adornerContainer.appendChild(decorator0);
            //}

        });
    }


    private dblClickDecoratorHandler(owner: AdornerManager, e: MouseEvent): void {


        const element = this.parentEditor.getSourceElementFromDecorator(e.target as HTMLElement);
        if (element) {
            const info = this.parentEditor.getElementInfo(element);
            const component = this.parentEditor.getComponentInfoFromDecorator(e.target as AdornerDecorator);

            $HOST_INTEROP.raiseElementDblClick(JSON.stringify(info));
        }

    }


    /**
     * Temporarily stops events listening
     */
    public discardEvents() {
        this._adornerContainer.style.pointerEvents = "none";
    }

    /**
     * Ensure the events will be captured by the layer
     */
    public restoreEvents() {
        this._adornerContainer.style.pointerEvents = "auto";
    }


    private handleMouseUp(owner: AdornerManager, e: MouseEvent): void {

        if (this._dragInfo) {
            this._dragInfo = null;
        }

        this.clearDragStates();
    }

    private handleMouseClick(owner: AdornerManager, e: MouseEvent) {

        if (e.defaultPrevented) {
            return;
        }
        
        if (e.target instanceof HTMLElement) {

            // when clicking inside an element that is in text edit mode
            // we don't want to quit text edit
            const closestEditable = e.target.closest('[contenteditable]');
            if (closestEditable) {
                return;
            }

        }

        const info: ElementInfo = owner.parentEditor.getElementInfo(this._mouseDownElement);

        $HOST_INTEROP.raiseElementClick(JSON.stringify(info));

        // if (this._textEditingDecorator) {
        //     this.exitTextEditMode();
        // }
    }

    private handleMouseDown(event: MouseEvent): void {

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

                let component = this.parentEditor.getComponentInfoFromHtmlElement(sourceElement as HTMLElement);

                if (this.parentEditor.isSlot(sourceElement as HTMLElement)) {
                    sourceElement = this.parentEditor.getClosestComponentAsHtmlElement(sourceElement as HTMLElement);
                }

                this.parentEditor.handleSelection(sourceElement as HTMLElement);
            }

        }
    }


    /**
     * Sets up mouse event listeners to handle user interactions.
     *
     * @return {void} Does not return a value.
     */
    private setUpMouseEvents() {

        const self = this;

        this._adornerContainer.addEventListener("mouseup", function (e) {
            self.handleMouseUp(self, e);
        });


        this._adornerContainer.addEventListener("mousedown", function (e) {
            // keep track the the mousedown element to guaranty that click event
            // is consistant
            self._mouseDownElement = e.target as HTMLElement;

            self.handleMouseDown(e);
        });


        this._adornerContainer.addEventListener("click", function (e) {
            // we only want to manage real "click" on elements
            // meaning that the mousedown and mouseup events were on the same element
            // otherwise "click" will be raised when starting to select text in a P
            // and releasing the mouse button on another element
            if (e.target != self._mouseDownElement) {
                return;
            }

            self.handleMouseClick(self, e);
        })


        this._adornerContainer.addEventListener("contextmenu", function (e) {
            $HOST_INTEROP.contextMenuOpening(e.clientX, e.clientY);
        })

        document.addEventListener("contextmenu", function (e) {
            $HOST_INTEROP.contextMenuOpening(e.clientX, e.clientY);
        })


        document.addEventListener("mousedown", function (e) {
            // keep track the the mousedown element to guaranty that click event
            // is consistant
            self._mouseDownElement = e.target as HTMLElement;
        });

        document.addEventListener("click", function (e) {
            if (e.target != self._mouseDownElement) {
                return;
            }
            self.handleMouseClick(self, e);
        })


    }

}

customElements.define('adorner-layer', AdornerManager);