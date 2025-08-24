class EventEmitter {
    constructor() {
        this.listeners = [];
    }
    emit(event) {
        this.listeners.forEach(listener => listener(event));
    }
    off(listener) {
        this.listeners = this.listeners.filter(l => l !== listener);
    }
    on(listener) {
        this.listeners.push(listener);
    }
}
//# sourceMappingURL=Main.js.map