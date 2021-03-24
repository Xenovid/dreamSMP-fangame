using Unity.Collections;
using Unity.Entities;

public class BattleManagerSystem : SystemBase
{
      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

      protected override void OnStartRunning(){
            base.OnStartRunning();

            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      }

      protected override void OnUpdate()
      {
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            EntityQuery battleCharacterGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleCharacters = battleCharacterGroup.ToEntityArray(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .ForEach((Entity battleManagerEntity, BattleManagerData battleManager) =>{
                  // if the player wins, give them some awards and give them some kind 
                  // if the player loses, set them to their last respawn point
                  if(battleManager.hasPlayerWon){
                        foreach(Entity entity in battleCharacters){
                              ecb.RemoveComponent<BattleData>(entity);
                        }
                        ecb.RemoveComponent<BattleManagerData>(battleManagerEntity);
                  }
            }).Run();

            battleCharacters.Dispose();
      }
}
