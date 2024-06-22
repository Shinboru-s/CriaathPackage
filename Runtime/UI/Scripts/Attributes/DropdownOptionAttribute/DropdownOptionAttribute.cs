using UnityEngine;

public class DropdownOptionAttribute : PropertyAttribute
{
    public string OptionsFieldName { get; private set; }

    public DropdownOptionAttribute(string optionsFieldName)
    {
        OptionsFieldName = optionsFieldName;
    }
}
