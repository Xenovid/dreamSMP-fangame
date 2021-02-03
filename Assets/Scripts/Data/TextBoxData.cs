using UnityEngine.UIElements;
using Unity.Entities;
using UnityEngine;

public class TextBoxData : IComponentData
{
    public int currentChar = 0;
    public int currentPage = 0;
    public float timeFromLastChar = 0.0f;
    public Label textBoxText;
}
