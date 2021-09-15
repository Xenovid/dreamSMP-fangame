using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
public class CutsceneTriggerSystem : SystemBase
{
    StepPhysicsWorld physicsWorld;
    CollisionWorld collisionWorld;
    UISystem uISystem;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    InkDisplaySystem inkDisplaySystem;
    protected override void OnCreate()
    {
        uISystem = World.GetOrCreateSystem<UISystem>();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        inkDisplaySystem = World.GetExistingSystem<InkDisplaySystem>();
    }

    protected override void OnUpdate()
    {
        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;

        EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag), typeof(MovementData));
            

        foreach(TriggerEvent triggerEvent in triggerEvents)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (!HasComponent<ToLoadData>(entityA) && HasComponent<CutsceneTriggerTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB) && uISystem.textBoxUI != null)
            {
                if(!GetComponent<CutsceneTriggerTag>(entityA).isTriggered){
                    CutsceneData text = EntityManager.GetComponentObject<CutsceneData>(entityA);
                    inkDisplaySystem.StartCutScene(text.cutsceneName);
                    InputGatheringSystem.currentInput = CurrentInput.ui;
                    EntityManager.SetComponentData(entityA, new CutsceneTriggerTag{isTriggered = true});
                }
            }
            else if (!HasComponent<ToLoadData>(entityB) &&HasComponent<CutsceneTriggerTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA) && uISystem.textBoxUI != null)
            {
                if(!GetComponent<CutsceneTriggerTag>(entityB).isTriggered){
                    CutsceneData text = EntityManager.GetComponentObject<CutsceneData>(entityB);
                    inkDisplaySystem.StartCutScene(text.cutsceneName);
                    InputGatheringSystem.currentInput = CurrentInput.ui;
                    EntityManager.SetComponentData(entityB, new CutsceneTriggerTag{isTriggered = true});
                }
            }
        }
        
    }
}
