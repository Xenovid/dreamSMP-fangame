using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct UIInputData : IComponentData
{
    public bool moveup;
    public bool movedown;
    public bool moveright;
    public bool moveleft;
    public bool goselected;
    public bool goback;
}
