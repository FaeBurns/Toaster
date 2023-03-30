using System;

namespace Toaster.Definition;

[Flags]
public enum DefinitionParameterFlag
{
    REGISTER = 1 << 0,
    LABEL = 1 << 1,
    CONSTANT = 1 << 2,
    PIN_SINGLE = 1 << 3,
    PIN_MULTIPLE = 1 << 4,

    REGISTER_CONSTANT = REGISTER | CONSTANT,
    REGISTER_CONSTANT_LABEL = REGISTER_CONSTANT | LABEL,

    PIN_ALL = PIN_SINGLE | PIN_MULTIPLE,
}