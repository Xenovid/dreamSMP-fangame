using Unity.Entities;
using UnityEngine.UIElements;

public class PlayerSelectorUI : IComponentData
{
    public VisualElement UI;
    public int currentItem;
    public bool isSelected;
    public bool isHovered;
    public bool notSelectable;
}
