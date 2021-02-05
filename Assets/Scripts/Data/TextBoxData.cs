using UnityEngine.UIElements;
using Unity.Entities;
using UnityEngine;

public struct TextBoxData : IComponentData
{
    public int currentChar;
    public int currentPage;
    public bool isFinishedPage;
    public float timeFromLastChar;
}
