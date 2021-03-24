using Unity.Entities;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

[GenerateAuthoringComponent]
public class ImagesData : IComponentData
{
    public List<Texture2D> images;
}
