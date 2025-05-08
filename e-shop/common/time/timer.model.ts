export class Timer {
    private timer: string | number | /*NodeJS.Timeout*/ any | null;

    clearTimeout(): void {
        if (this.timer) {
            clearTimeout(this.timer);
        }
        this.timer = null;
    }

    setTimeout(callback: () => void, ms: number): void {
        this.timer = setTimeout(callback, ms);
    }
}
