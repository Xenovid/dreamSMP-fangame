using Unity.Entities;
using UnityEngine.UIElements;

public class PlayerSelectorUI : IComponentData
{
    public VisualElement UI;
    public battleSelectables currentSelection;
    public bool isSelected;
    public bool isHovered;
    public bool isSelectable;
}
public enum battleSelectables{
    fight,
    skills,
    items,
    run
}
