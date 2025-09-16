static class $EDITOR_KIT_DATA {
    static COMPONENTS_CATALOG: KitComponent[];
    static KNOWN_COMPONENTS_SELECTOR: string;
    static ADORNER_DECORATORS_SELECTOR: string;
}


//
// IMPORTANT, Methods will be exposed using camelCase
// declare them correctly here
//
static class $HOST_INTEROP {
    static contextMenuOpening(x: number, y: number): void;
    static componentActionMenuOpening(x: number, y: number): void;
    static insertBarActionMenuOpening(json: string, x: number, y: number): void;
    static raiseSelectedElementChanged(json: string): void;
    static raiseTextSelected(text: string): void;
    static dropElement(sourceElementInfoJson: string, targetElementInfoJson: string, position: number, ctrlPressed: boolean): void;
    static raiseElementDblClick(json: string): void;
    static raiseElementClick(json: string): void;
    static raiseScroll(): void;
}