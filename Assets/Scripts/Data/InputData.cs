using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class InputData : IComponentData
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode selectKey;
    public KeyCode backKey;
    public KeyCode escapeKey;
}
