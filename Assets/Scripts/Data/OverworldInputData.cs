using Unity.Entities;
using UnityEngine;

public struct OverworldInputData : IComponentData
{
    public float moveHorizontal;
    public float moveVertical;

    public bool select;
    public bool back;

    public bool escape;
}
