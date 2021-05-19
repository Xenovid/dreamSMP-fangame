using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Tilemap tileMap, TilemapRenderer renderer) =>
        {
            AddHybridComponent(tileMap.layoutGrid);
            AddHybridComponent(tileMap);
            AddHybridComponent(renderer);
        });
    }
}
