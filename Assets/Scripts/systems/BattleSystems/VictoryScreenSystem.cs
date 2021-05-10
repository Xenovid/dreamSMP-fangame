using Unity.Entities;
using Unity.Physics;
using UnityEngine.UIElements;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

public class VictoryScreenSystem : SystemBase
{
    UIDocument UIDoc;
    bool isPrintingVictoryData;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    InkDisplaySystem inkDisplaySystem;
    
    protected override void OnStartRunning(){
        // getting the endsinclation system for a entity component buffer later on
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        // getting the inkDisplaySystem to watch for when its done writing
        inkDisplaySystem = World.GetExistingSystem<InkDisplaySystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, ref Translation translation, ref TransitionData transitionData) =>
        {
            translation.Value = Vector3.MoveTowards(translation.Value, transitionData.newPosition, 10 * deltaTime);
            if(translation.Value.x == transitionData.newPosition.x && translation.Value.y == transitionData.newPosition.y)
            {
                InputGatheringSystem.currentInput = CurrentInput.overworld;
                EntityManager.AddComponentData(entity, new PhysicsCollider { Value = transitionData.colliderRef });
                EntityManager.RemoveComponent<BeforeBattleData>(entity);
            }
            else if(HasComponent<Unity.Physics.PhysicsCollider>(entity))
            {
                transitionData.colliderRef = GetComponent<PhysicsCollider>(entity).Value;
                ecb.RemoveComponent<Unity.Physics.PhysicsCollider>(entity);
            }
        }).Run();
        

      }
    private void FinishVictoryData_OnWritingFinished(object sender, System.EventArgs e){
        inkDisplaySystem.OnWritingFinished -= FinishVictoryData_OnWritingFinished;
        isPrintingVictoryData = false;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        // transition back once the writer is done
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref BeforeBattleData beforeBattleData, in Entity entity) =>
        {
            ecb.AddComponent(entity, new TransitionData{newPosition = beforeBattleData.previousLocation});
            ecb.RemoveComponent<BeforeBattleData>(entity);
        }).Run();
    }
    private void StartVictoryData_OnBattleEnd(object sender, OnBattleEndEventArgs e){
        if(e.isPlayerVictor){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            EntityQuery battleCharacterGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleCharacters = battleCharacterGroup.ToEntityArray(Allocator.TempJob);

            EntityQuery beforeBattleGroup = GetEntityQuery(typeof(BeforeBattleData));
            NativeArray<Entity> beforeBattleDatas = beforeBattleGroup.ToEntityArray(Allocator.TempJob);

            // if the player wins, give them some awards and give them some kind 
            // if the player loses, set them to their last respawn point

            //if a character loses or wins, say the results
            //since they aren't in a battle, remove the battle data
            foreach (Entity entity in battleCharacters){
                ecb.RemoveComponent<BattleData>(entity);
            }

            beforeBattleDatas.Dispose();
            battleCharacters.Dispose();

            isPrintingVictoryData = true;
            inkDisplaySystem.DisplayVictoryData();
            inkDisplaySystem.OnWritingFinished += FinishVictoryData_OnWritingFinished;
        }
        
    }

}
