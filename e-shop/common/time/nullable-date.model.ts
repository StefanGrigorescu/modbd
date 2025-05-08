export type NullableDate = Date | null;

export function nullableDateFromUTCString(nullableDateString: string | null): NullableDate {
    if (nullableDateString === null || nullableDateString.length === 0) {
        return null;
    }
    if (nullableDateString.endsWith('Z')) {
        return new Date(nullableDateString);
    }
    return new Date(`${nullableDateString}Z`);
}

export function nullableDateFromLocaleString(nullableDateString: string | null): NullableDate {
    return nullableDateString === null ?
        null :
        new Date(nullableDateString);
}
