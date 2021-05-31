using Unity.Entities;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.Mathematics;
using UnityEngine;

public class HeadsUpUIData : IComponentData
{
    public VisualElement UI;
    public List<Message> messages;
}
public struct Message{
    public Label label;
    public float timePassed;
    public float2 direction;
}
