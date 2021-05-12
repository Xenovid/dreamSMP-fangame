using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Physics;
using System;

// moves an entity from one position to another over time
public class TransitionSystem : SystemBase
{
    public BattleSystem battleSystem;
    public event EventHandler OnTransitionEnd;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    
    protected override void OnCreate(){
        // getting the endsinclation system for a entity component buffer later on
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleStart += MoveToBattlePositions_OnBattleStart;
    }
    // on every update move entities towards the goal position
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        // used to activate the event when the event is done
        bool translationDone = false;
        float dT = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, ref Translation translation, ref TransitionData transitionData, ref PhysicsCollider collider) =>
        {
            // making it so that the collision wont hit anything while moving to the location
            collider.Value.Value.Filter = CollisionFilter.Zero;
            translation.Value = Vector3.MoveTowards(translation.Value, transitionData.newPosition, 10 * dT);
            if(translation.Value.x == transitionData.newPosition.x && translation.Value.y == transitionData.newPosition.y)
            {
                translationDone = true;
                collider.Value.Value.Filter = CollisionFilter.Default;
                // now in the location, stop translating
                ecb.RemoveComponent<TransitionData>(entity);
            }  
        }).Run();
        // if the translation is done, let every system that wants to know, know
        if(translationDone){
            OnTransitionEnd?.Invoke(this, System.EventArgs.Empty);
        }
    }

    private void MoveToBattlePositions_OnBattleStart(System.Object sender, System.EventArgs e){
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        Debug.Log("hello");

        // needed so the battlers are on the right location on screen
        EntityQuery cameraQuery = GetEntityQuery(typeof(Camera));
        Camera camera = cameraQuery.ToComponentArray<Camera>()[0];
        // finds all the players in the party and moves their positions for battle
        int i = 0;
        int playerLength = battleSystem.playerEntities.Count;
        foreach(Entity entity in battleSystem.playerEntities){
            Vector3 tempPos = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth * .1f, ((i + 1) * (camera.pixelHeight / playerLength)) - camera.pixelHeight / (playerLength * 2), 0));
            ecb.AddComponent(entity, new TransitionData{newPosition = new Vector3(tempPos.x,tempPos.y,0)});
            i++;
        }
        i = 0;
        int enemyLength = battleSystem.enemyEntities.Count;
        //finds all the enemies and moves them to battle positions
        foreach(Entity entity in battleSystem.enemyEntities){
            Vector3 tempPos = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth * .9f, ((i + 1) * (camera.pixelHeight / enemyLength)) - camera.pixelHeight / (enemyLength * 2), 0));
            ecb.AddComponent(entity, new TransitionData{newPosition = new Vector3(tempPos.x,tempPos.y,0)});
            i++;
        }
    }
}

