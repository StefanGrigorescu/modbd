export class NormalizedString extends String {
    readonly asString: string;

    hasSubstring(other: String): boolean {
        return this.includes(
            toNormalized(other)
        );
    }

    equals(other: NormalizedString): boolean {
        return this.asString === other.asString;
    }

    static readonly empty: NormalizedString = new NormalizedString("");

    constructor (value: String) {
        super(
            toNormalized(value)
        );
        this.asString = this.toString();
    }
}


export function areEqual(left: String, right: String): boolean {
    return toNormalized(left) === toNormalized(right);
}


export function toNormalized(str: String): string {
    return str
        .replace(/-/g, ' ')         // Replace hyphens with spaces
        .replace(/\s+/g, ' ')       // Replace more subsequent whitespaces with a single space
        .trim()                     // Remove leading and trailing whitespaces

        // Replace some special non-latin characters with latin equivalent.
        .replace('Ü', 'u')
        .replace('Ãœ', 'u')

        .replace(/[^\w\s]/g, '')    // Remove punctuation (after replacing non-latin characters)
        .toLocaleLowerCase();       // Convert to lowercase
}
