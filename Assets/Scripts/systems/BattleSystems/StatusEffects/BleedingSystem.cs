using Unity.Entities;

public class BleedingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities.ForEach((ref DynamicBuffer<DamageData> damages, ref BleedingData bleedingData) =>{
            bleedingData.timeFromLastDamageTick += dt;
            //deal damage every other second
            if(bleedingData.timeFromLastDamageTick >= 2){
                damages.Add(new DamageData{damage = bleedingData.level, color = damageColor.red});
            }
        }).Schedule();
    }
}
