class TextSelectionInfo {

    selectedText: string;
    isSelectionBold: boolean;
    isSelectionItalic: boolean;
    isSelectionUnderline: boolean;
    isSelectionStrikeThrough: boolean;
    isSelectionSuperScript: boolean;
    isSelectionSubScript: boolean;
    isSelectionBulletList: boolean;
    isSelectionNumberList: boolean;
    isSelectionLeftAlign: boolean;
    isSelectionCenterAlign: boolean;
    isSelectionRightAlign: boolean;
    isSelectionJustifyAlign: boolean;
    selectionRangeRectX: number;
    selectionRangeRectY: number;
    selectionRangeRectWidth: number;
    selectionRangeRectHeight: number;
    canCreateLink: boolean;
    canUnLink: boolean;
    parentTagName: string;
    selectionTagName: string;
    selectionClassName: string;

}

class TextEditor {

    private _parentEditor: EditorComponent;
    private _element: HTMLElement;
    private _throttleSelectionChanged: number | undefined;

    constructor(parentEditor: EditorComponent) {
        this._parentEditor = parentEditor;
        this.setUpEvents();
    }

    public attach(element: HTMLElement) {
        this._element = element;


        document.addEventListener('selectionchange', this.handleSelectionChange);

    }

    private handleSelectionChange(e: Event) {

        const isBold = document.queryCommandState("bold");
        const isItalic = document.queryCommandState("italic");
        const isUnderline = document.queryCommandState("underline");
        const isStrikeThrough = document.queryCommandState("strikethrough");
        const isSuperScript = document.queryCommandState("superscript");
        const isSubScript = document.queryCommandState("subscript");

        const isBulletList = document.queryCommandState("insertUnorderedList");
        const isNumberList = document.queryCommandState("insertOrderedList");
        const isLeftAlign = document.queryCommandState("justifyLeft");
        const isCenterAlign = document.queryCommandState("justifyCenter");
        const isRightAlign = document.queryCommandState("justifyRight");
        const isJustifyAlign = document.queryCommandState("justifyFull");
        const canCreateLink = true;

        let canUnLink = false;
        let parentTagName = "";
        let selectionTagName = "";
        let selectionClassName = "";
        
        if (window.getSelection().toString() !== "") {
            try {
                let selection = window.getSelection().getRangeAt(0);
                if (selection) {
                    if (selection.startContainer.parentNode.nodeName === "A"
                        || selection.endContainer.parentNode.nodeName === "A") {
                        canUnLink = true;
                    }

                    parentTagName = selection.endContainer.parentNode.nodeName.toLowerCase();
                    selectionTagName = selection.endContainer.nodeName;
                    selectionClassName = selection.endContainer.nodeName;
                }
            } catch (er) {
            }
        }

        const sel = window.getSelection();
        let rangeRect = new DOMRect();
        rangeRect.x = 0;
        rangeRect.y = 0;
        rangeRect.width = 0;
        rangeRect.height = 0;

        try {
            let range = sel.getRangeAt(0);
            rangeRect = range.getBoundingClientRect();
        } catch (err) {
        }

        let o = new TextSelectionInfo();
        o.selectedText = document.getSelection().toString();
        o.isSelectionBold = isBold;
        o.isSelectionItalic = isItalic;
        o.isSelectionUnderline = isUnderline;
        o.isSelectionStrikeThrough = isStrikeThrough;
        o.isSelectionSubScript = isSubScript;
        o.isSelectionSuperScript = isSuperScript;
        o.isSelectionBulletList = isBulletList;
        o.isSelectionNumberList = isNumberList;
        o.isSelectionLeftAlign = isLeftAlign;
        o.isSelectionCenterAlign = isCenterAlign;
        o.isSelectionRightAlign = isRightAlign;
        o.isSelectionJustifyAlign = isJustifyAlign;
        o.selectionRangeRectX = rangeRect.x;
        o.selectionRangeRectY = rangeRect.y;
        o.selectionRangeRectWidth = rangeRect.width;
        o.selectionRangeRectHeight = rangeRect.height;
        o.canCreateLink = canCreateLink;
        o.canUnLink = canUnLink;
        o.parentTagName = parentTagName;
        o.selectionTagName = selectionTagName;
        o.selectionClassName = selectionClassName;

        if (!this._throttleSelectionChanged) {
            this._throttleSelectionChanged = setTimeout(() => {

                const json = JSON.stringify(o);

                $HOST_INTEROP.raiseTextSelected(json);

                this._throttleSelectionChanged = undefined;

            }, 150);
        }

    }

    public detach() {
        document.removeEventListener('selectionchange', this.handleSelectionChange);
    }


    private setUpEvents(): void {

        document.addEventListener('keydown', function (event) {

            if ((event.key === "ArrowUp" || event.key === "ArrowDown" ||
                event.key === "ArrowLeft" || event.key === "ArrowRight")
            ) {
                return;
            }

            if (event.ctrlKey || event.metaKey) {
                event.preventDefault();
            }

        });
    }

}

