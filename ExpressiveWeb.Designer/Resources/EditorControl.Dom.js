class DomHelper {
    constructor(owner) {
        this.parentEditor = owner;
    }
    reloadExternalResources() {
        this.parentEditor.adornerManager.clearDecorators();
        function appendCacheBuster(url) {
            let [base, query = ''] = url.split('?');
            if (query) {
                let params = query
                    .split('&')
                    .filter(p => !p.startsWith('v='))
                    .filter(p => p.trim() !== '');
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
                [...script.attributes].forEach(attr => {
                    if (attr.name !== 'src') {
                        newScript.setAttribute(attr.name, attr.value);
                    }
                });
                script.parentNode.replaceChild(newScript, script);
            }
        });
        const self = this;
        setTimeout(() => {
            self.parentEditor.adornerManager.updateDecorators();
        }, 500);
    }
    changeTagName(elementInternalId, tagName) {
        const element = this.parentEditor.getElementByInternalId(elementInternalId);
        if (!element) {
            return;
        }
        const newElement = document.createElement(tagName);
        newElement.className = element.className;
        newElement.style.cssText = element.style.cssText;
        [...element.attributes].forEach(attr => {
            if (attr.name !== 'style' && attr.name !== 'class') {
                newElement.setAttribute(attr.name, attr.value);
            }
        });
        newElement.innerHTML = element.innerHTML;
        element.parentNode.replaceChild(newElement, element);
        newElement.dataset.internalId = element.dataset.internalId;
    }
    moveElement(elementInternalId, newParentElementInternalId, index) {
        const element = this.parentEditor.getElementByInternalId(elementInternalId);
        if (!element) {
            return;
        }
        const newParentElement = this.parentEditor.getElementByInternalId(newParentElementInternalId);
        if (newParentElement) {
            this.insertElementAtPosition(element, newParentElement, index);
        }
    }
    setElementInnerHtml(elementInternalId, content) {
        const element = this.parentEditor.getElementByInternalId(elementInternalId);
        if (!element) {
            console.error('setElementInnerHtml: Element not found');
            return;
        }
        element.innerHTML = content;
    }
    removeElementJson(json) {
        const info = JSON.parse(json);
        let htmlElement = this.parentEditor.getElementByInternalId(info.internalId);
        if (htmlElement) {
            htmlElement.remove();
        }
    }
    insertElementJson(json) {
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
    insertElementAtPosition(element, newParentElement, index) {
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
        this.parentEditor.ensureInternalIds(element);
    }
}
//# sourceMappingURL=EditorControl.Dom.js.map