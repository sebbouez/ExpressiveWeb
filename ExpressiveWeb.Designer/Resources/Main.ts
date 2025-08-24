type Listener<T> = (event: T) => void;

class EventEmitter<T> {
    private listeners: Listener<T>[] = [];

    emit(event: T): void {
        this.listeners.forEach(listener => listener(event));
    }

    off(listener: Listener<T>): void {
        this.listeners = this.listeners.filter(l => l !== listener);
    }

    on(listener: Listener<T>): void {
        this.listeners.push(listener);
    }
}
