using Unity.Entities;
using Unity.Collections;
[System.Serializable]
public struct SavePointData : IComponentData
{
    public FixedString128 savePointName;
    public float timePassed;
}
