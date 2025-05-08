export function isDateExpired(expireDate: Date): boolean {
    if (!expireDate) {
        throw new Error('Expire date was not available!');
    }
    return new Date() > expireDate;
}
