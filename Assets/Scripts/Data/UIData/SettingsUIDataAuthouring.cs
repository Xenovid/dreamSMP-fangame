using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
[GenerateAuthoringComponent]
public struct SettingsUIData : IComponentData
{
    [HideInInspector]
    public SettingsTab currentTab;
    public bool isOnTitleScreen;
    [HideInInspector]

    public bool isActive;
    [HideInInspector]

    public bool isSetUp;
    [HideInInspector]

    public bool isSelected;
    [HideInInspector]

    public int currentItem;
}
