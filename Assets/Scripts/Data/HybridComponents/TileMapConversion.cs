using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithNone<TilemapCollider2D>()
        .ForEach((Tilemap tileMap, TilemapRenderer renderer) =>
        {
            AddHybridComponent(tileMap.layoutGrid);
            AddHybridComponent(tileMap);
            AddHybridComponent(renderer);
        });
    }
}
