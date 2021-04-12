using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(( Grid grid) =>
        {
            AddHybridComponent(grid);
        });
    }
}
