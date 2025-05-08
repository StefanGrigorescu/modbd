import { textFieldMdMaxLength, textFieldSmMaxLength } from "./text-max-lengths.const";

export type TextFieldType = "sm" | "md";

export const defaultTextFieldType: TextFieldType = "sm";

export function getTextFieldMaxLength(textFieldType: TextFieldType): number {
    switch (textFieldType) {
        case "md":
            return textFieldMdMaxLength;
        case "sm":
            return textFieldSmMaxLength;
        default:
            throw Error(`Unexpected text field type: ${textFieldType}`);
    }
}
