using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Tilemaps;
using Unity.Physics;
using UnityEngine;

public class ColliderConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithNone<Prefab>()
        .ForEach((PolygonCollider2D collider, Transform transform) => {
            Entity entity = GetPrimaryEntity(collider);
            Mesh tempMesh = collider.CreateMesh(true, true);
            NativeArray<float3> vertices = new NativeArray<float3>(tempMesh.vertices.Length, Allocator.Temp);
            NativeArray<int3> trianges = new NativeArray<int3>(tempMesh.triangles.Length, Allocator.Temp);

            int i = 0;
            foreach(Vector3 vertice in tempMesh.vertices){
                vertices[i] = vertice - transform.position;
                i++;
            }
            i=0;
            
            for(int j = 0; j < tempMesh.triangles.Length; j += 3){
                trianges[i] = new int3(tempMesh.triangles[j], tempMesh.triangles[j+1], tempMesh.triangles[j+2]);
                i++;
            }
            
            BlobAssetReference<Unity.Physics.Collider> colliderRef = Unity.Physics.MeshCollider.Create(vertices, trianges, CollisionFilter.Default);//ConvexCollider.Create(vertices, ConvexHullGenerationParameters.Default);
            DstEntityManager.AddComponentData(entity, new PhysicsCollider{Value = colliderRef});
            DstEntityManager.AddComponentData(entity, PhysicsMass.CreateKinematic(MassProperties.UnitSphere));
            DstEntityManager.AddComponentData(entity, new PhysicsVelocity{Linear = float3.zero, Angular = float3.zero});
            DstEntityManager.AddComponentData(entity, new PhysicsDamping{Linear = 10, Angular = 5});
            DstEntityManager.AddComponentData(entity, new PhysicsGravityFactor{Value = 1});
            PhysicsGravityFactor grav = DstEntityManager.GetComponentData<PhysicsGravityFactor>(entity);
            grav.Value = 0;
            DstEntityManager.SetComponentData(entity, grav);
            vertices.Dispose();
            trianges.Dispose();
        });
        Entities.ForEach((BoxCollider2D collider, Transform transform) => {
            Entity entity = GetPrimaryEntity(collider);
            Mesh tempMesh = collider.CreateMesh(true, true);
            NativeArray<float3> vertices = new NativeArray<float3>(tempMesh.vertices.Length, Allocator.Temp);
            NativeArray<int3> trianges = new NativeArray<int3>(tempMesh.triangles.Length, Allocator.Temp);

            int i = 0;
            foreach(Vector3 vertice in tempMesh.vertices){
                vertices[i] = vertice - transform.position;
                i++;
            }
            i=0;
            
            for(int j = 0; j < tempMesh.triangles.Length; j += 3){
                trianges[i] = new int3(tempMesh.triangles[j], tempMesh.triangles[j+1], tempMesh.triangles[j+2]);
                i++;
            }
            
            BlobAssetReference<Unity.Physics.Collider> colliderRef = Unity.Physics.ConvexCollider.Create(vertices, ConvexHullGenerationParameters.Default);// MeshCollider.Create(vertices, trianges, CollisionFilter.Default);
            DstEntityManager.AddComponentData(entity, new PhysicsCollider{Value = colliderRef});
            DstEntityManager.AddComponentData(entity, PhysicsMass.CreateKinematic(MassProperties.UnitSphere));
            DstEntityManager.AddComponentData(entity, new PhysicsVelocity{Linear = float3.zero, Angular = float3.zero});
            DstEntityManager.AddComponentData(entity, new PhysicsDamping{Linear = 10, Angular = 5});
            DstEntityManager.AddComponentData(entity, new PhysicsGravityFactor{Value = 1});
            PhysicsGravityFactor grav = DstEntityManager.GetComponentData<PhysicsGravityFactor>(entity);
            grav.Value = 0;
            DstEntityManager.SetComponentData(entity, grav);
            vertices.Dispose();
            trianges.Dispose();
        });
        Entities.ForEach((Tilemap tileMap, TilemapRenderer renderer, TilemapCollider2D collider, Transform transform) => {
            AddHybridComponent(tileMap.layoutGrid);
            AddHybridComponent(tileMap);
            AddHybridComponent(renderer);

            Entity entity = GetPrimaryEntity(collider);
            Mesh tempMesh = collider.CreateMesh(true, true);
            NativeArray<float3> vertices = new NativeArray<float3>(tempMesh.vertices.Length, Allocator.Temp);
            NativeArray<int3> trianges = new NativeArray<int3>(tempMesh.triangles.Length, Allocator.Temp);
            int i = 0;
            foreach(Vector3 vertice in tempMesh.vertices){

                vertices[i] = vertice - transform.position;
                i++;
            }
            i=0;
            
            for(int j = 0; j < tempMesh.triangles.Length; j += 3){
                trianges[i] = new int3(tempMesh.triangles[j], tempMesh.triangles[j+1], tempMesh.triangles[j+2]);
                i++;
            }
            
            BlobAssetReference<Unity.Physics.Collider> colliderRef = Unity.Physics.MeshCollider.Create(vertices, trianges, CollisionFilter.Default); //ConvexCollider.Create(vertices, ConvexHullGenerationParameters.Default);

            DstEntityManager.AddComponentData(entity, new PhysicsCollider{Value = colliderRef});
            DstEntityManager.AddComponentData(entity, PhysicsMass.CreateKinematic(MassProperties.UnitSphere));
            DstEntityManager.AddComponentData(entity, new PhysicsVelocity{Linear = float3.zero, Angular = float3.zero});
            DstEntityManager.AddComponentData(entity, new PhysicsDamping{Linear = 10, Angular = 5});
            DstEntityManager.AddComponentData(entity, new PhysicsGravityFactor{Value = 1});
            PhysicsGravityFactor grav = DstEntityManager.GetComponentData<PhysicsGravityFactor>(entity);
            grav.Value = 0;
            DstEntityManager.SetComponentData(entity, grav);
            vertices.Dispose();
            trianges.Dispose();
            
        });
    }
}
