using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class PolyInteractiveAuthoring : MonoBehaviour
{
    [SerializeReference]
    public IPolyInteractiveData interactiveData;
    public PolyInteractiveData.TypeId typeId;
    public SharedInteractiveData sharedInteractiveData; 
}
public class PolyInteractiveConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PolyInteractiveAuthoring polyInteractiveAuthoring) =>{
            Entity entitiy = GetPrimaryEntity(polyInteractiveAuthoring);
            PolyInteractiveData polyInteractiveData;
            switch (polyInteractiveAuthoring.typeId)
            {
                case PolyInteractiveData.TypeId.WeaponChestData:
                    polyInteractiveData = new PolyInteractiveData(
                        (WeaponChestData)polyInteractiveAuthoring.interactiveData,
                        polyInteractiveAuthoring.sharedInteractiveData);
                    DstEntityManager.AddComponentData(entitiy, polyInteractiveData);
                    break;
                case PolyInteractiveData.TypeId.ArmorChestData:
                    polyInteractiveData = new PolyInteractiveData(
                        (ArmorChestData)polyInteractiveAuthoring.interactiveData,
                        polyInteractiveAuthoring.sharedInteractiveData);
                    DstEntityManager.AddComponentData(entitiy, polyInteractiveData);
                    break;
                case PolyInteractiveData.TypeId.CharmChestData:
                    polyInteractiveData = new PolyInteractiveData(
                        (CharmChestData)polyInteractiveAuthoring.interactiveData,
                        polyInteractiveAuthoring.sharedInteractiveData);
                    DstEntityManager.AddComponentData(entitiy, polyInteractiveData);
                    break;
                case PolyInteractiveData.TypeId.CutsceneInteractiveData:
                    polyInteractiveData = new PolyInteractiveData(
                        (CutsceneInteractiveData)polyInteractiveAuthoring.interactiveData,
                        polyInteractiveAuthoring.sharedInteractiveData);
                    DstEntityManager.AddComponentData(entitiy, polyInteractiveData);
                    break;
                case PolyInteractiveData.TypeId.ItemChestData:
                    polyInteractiveData = new PolyInteractiveData(
                        (ItemChestData)polyInteractiveAuthoring.interactiveData,
                        polyInteractiveAuthoring.sharedInteractiveData);
                    DstEntityManager.AddComponentData(entitiy, polyInteractiveData);
                    break;
            }
            



            //PolyInteractiveData polyInteractiveData = new PolyInteractiveData(polyInteractiveAuthoring.interactiveData, )
        });
    }
}
