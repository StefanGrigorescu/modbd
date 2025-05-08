export function isEmail(value: string): boolean {
    return emailRegex.test(value);
}

const emailRegex: RegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
