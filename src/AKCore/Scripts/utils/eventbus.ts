class Event {
  constructor(private events: any = {}) {}

  on(eventName: string, fn: (data: any) => void) {
    this.events[eventName] = this.events[eventName] || [];
    this.events[eventName].push(fn);
  }

  off(eventName: string, fn: (data: any) => void) {
    if (this.events[eventName]) {
      for (let i = 0; i < this.events[eventName].length; i++) {
        if (this.events[eventName][i] === fn) {
          this.events[eventName].splice(i, 1);
          break;
        }
      }
    }
  }

  trigger(eventName: string, data: any) {
    if (this.events[eventName]) {
      this.events[eventName].forEach(function (fn: (data: any) => void) {
        fn(data);
      });
    }
  }
}
export const EventBus = new Event();
