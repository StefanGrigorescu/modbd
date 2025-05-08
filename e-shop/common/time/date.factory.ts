export function dateFromUTCString(nullableDateString: string | null): Date {
    if (nullableDateString === null || nullableDateString.length === 0) {
        throw new Error('Date was not provided!');
    }
    if (nullableDateString.endsWith('Z')) {
        return new Date(nullableDateString);
    }
    return new Date(`${nullableDateString}Z`);
}

export function dateFromLocaleString(nullableDateString: string | null): Date {
    if (nullableDateString === null) {
        throw new Error('Date was not provided!');
    }
    return new Date(nullableDateString);
}
