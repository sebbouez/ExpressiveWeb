class DomHelper {

    parentEditor: EditorComponent;

    constructor(owner: EditorComponent) {
        this.parentEditor = owner;
    }

    public reloadExternalResources(): void {

        this.parentEditor.adornerManager.clearDecorators();

        function appendCacheBuster(url) {
            let [base, query = ''] = url.split('?');

            if (query) {
                let params = query
                    .split('&')
                    .filter(p => !p.startsWith('v=')) // retire tout v=
                    .filter(p => p.trim() !== '');    // évite les paramètres vides
                query = params.join('&');
            }

            let newParam = 'v=' + Date.now();
            return query ? `${base}?${query}&${newParam}` : `${base}?${newParam}`;
        }

        document.querySelectorAll('link[rel="stylesheet"]').forEach(link => {

            if (link.getAttribute('editor-usage') != 'private') {
                let newHref = appendCacheBuster(link.getAttribute('href'));
                link.setAttribute('href', newHref);
            }
        });

        document.querySelectorAll('script[src]').forEach(script => {
            if (script.getAttribute('editor-usage') != 'private') {
                let newSrc = appendCacheBuster(script.getAttribute('src'));
                let newScript = document.createElement('script');
                newScript.src = newSrc;

                // On copie tous les autres attributs du script original
                [...script.attributes].forEach(attr => {
                    if (attr.name !== 'src') {
                        newScript.setAttribute(attr.name, attr.value);
                    }
                });

                script.parentNode.replaceChild(newScript, script);
            }
        });


        const self = this;

        // dirty, find a better way to wait for the document to be updated
        setTimeout(() => {
            self.parentEditor.adornerManager.updateDecorators();
        }, 500);
    }

    
    public changeTagName(elementInternalId: string, tagName: string): void {

        const element: HTMLElement = this.parentEditor.getElementByInternalId(elementInternalId);
        if (!element) {
            return;
        }

        const newElement = document.createElement(tagName);

        // Copy classes
        newElement.className = element.className;

        // Copy style
        newElement.style.cssText = element.style.cssText;

        // Copy attributes
        [...element.attributes].forEach(attr => {
            if (attr.name !== 'style' && attr.name !== 'class') {
                newElement.setAttribute(attr.name, attr.value);
            }
        });

        // Copy content
        newElement.innerHTML = element.innerHTML;

        // Replace the old element with the new one
        element.parentNode.replaceChild(newElement, element);

        // We just wanted to change the tag name, so we don't need to change the editorid
        newElement.dataset.internalId = element.dataset.internalId;
    }

    public moveElement(elementInternalId: string, newParentElementInternalId: string, index: number): void {
        const element: HTMLElement = this.parentEditor.getElementByInternalId(elementInternalId);
        if (!element) {
            return;
        }

        const newParentElement = this.parentEditor.getElementByInternalId(newParentElementInternalId);
        if (newParentElement) {
            this.insertElementAtPosition(element, newParentElement, index);
        }
    }


    public setElementInnerHtml(elementInternalId: string, content: string): void {

        const element: HTMLElement = this.parentEditor.getElementByInternalId(elementInternalId);
        if (!element) {
            console.error('setElementInnerHtml: Element not found');
            return;
        }

        element.innerHTML = content;
    }


    public removeElementJson(json: string) {
        const info = JSON.parse(json);

        let htmlElement = this.parentEditor.getElementByInternalId(info.internalId);
        if (htmlElement) {
            htmlElement.remove();
        }
    }


    public insertElementJson(json: string) {

        const info = JSON.parse(json);

        let htmlElement = document.createElement(info.tagName);
        htmlElement.className = info.cssClass;
        htmlElement.innerHTML = info.innerHtml;
        htmlElement.dataset.internalId = info.internalId;

        const newParentElement = this.parentEditor.getElementByInternalId(info.parentInternalId);
        if (newParentElement) {
            this.insertElementAtPosition(htmlElement, newParentElement, info.index);
        }

    }

    private insertElementAtPosition(element: HTMLElement, newParentElement: HTMLElement, index: number): void {

        const lastIndex = newParentElement.children.length;
        if (index < 0) {
            index = Math.max(0, lastIndex + 1 + index);
        }
        newParentElement.appendChild(element);

        if (index < lastIndex) {
            const targetElement = newParentElement.children[index];
            if (targetElement) {
                newParentElement.insertBefore(element, targetElement);
            }
        }

        // after we insert something new in the editor, make sure everything has a good editorid
        this.parentEditor.ensureInternalIds(element);
    }
}