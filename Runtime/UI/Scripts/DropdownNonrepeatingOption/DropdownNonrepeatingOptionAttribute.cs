using UnityEngine;

public class DropdownNonrepeatingOptionAttribute : PropertyAttribute
{
    public string OptionsFieldName;

    public DropdownNonrepeatingOptionAttribute(string optionsFieldName)
    {
        OptionsFieldName = optionsFieldName;
    }
}
