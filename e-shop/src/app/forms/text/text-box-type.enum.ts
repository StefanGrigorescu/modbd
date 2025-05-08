import { textBoxLgMaxLength, textBoxMdMaxLength } from "./text-max-lengths.const";

export type TextBoxType = "md" | "lg";

export const defaultTextBoxType: TextBoxType = "md";

export function getTextBoxMaxLength(textBoxType: TextBoxType): number {
    switch (textBoxType) {
        case "lg":
            return textBoxLgMaxLength;
        case "md":
            return textBoxMdMaxLength;
        default:
            throw Error(`Unexpected text box type: ${textBoxType}`);
    }
}
