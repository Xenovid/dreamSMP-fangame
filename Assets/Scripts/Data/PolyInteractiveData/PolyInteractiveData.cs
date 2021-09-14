using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[System.Serializable]
public class PolyInteractiveData : IComponentData
{
    public enum TypeId{
        WeaponChestData,
        ArmorChestData,
        CharmChestData,
        ItemChestData,
        CutsceneInteractiveData
    }
    public SharedInteractiveData SharedInteractiveData;

    public WeaponChestData WeaponChestData;
    public ArmorChestData ArmorChestData;
    public CharmChestData CharmChestData;
    public ItemChestData ItemChestData;
    public CutsceneInteractiveData CutsceneInteractiveData;
    public TypeId CurrentTypeId;
    public PolyInteractiveData(){
        WeaponChestData = default;
        ArmorChestData = default;
        CharmChestData = default;
        CutsceneInteractiveData = default;
        SharedInteractiveData = default;
        CurrentTypeId = TypeId.WeaponChestData;
    }
    public PolyInteractiveData(in ItemChestData c, in SharedInteractiveData d){
        ItemChestData = c;
        WeaponChestData = default;
        ArmorChestData = default;
        CharmChestData = default;
        CutsceneInteractiveData = default;
        SharedInteractiveData = d;
        CurrentTypeId = TypeId.ItemChestData;
    }

    public PolyInteractiveData(in WeaponChestData c, in SharedInteractiveData d){
        ItemChestData = default;
        WeaponChestData = c;
        ArmorChestData = default;
        CharmChestData = default;
        CutsceneInteractiveData = default;
        SharedInteractiveData = d;
        CurrentTypeId = TypeId.WeaponChestData;
    }
    public PolyInteractiveData(in ArmorChestData c, in SharedInteractiveData d){
        ItemChestData = default;
        WeaponChestData = default;
        ArmorChestData = c;
        CharmChestData = default;
        CutsceneInteractiveData = default;
        SharedInteractiveData = d;
        CurrentTypeId = TypeId.ArmorChestData;
    }
    public PolyInteractiveData(in CharmChestData c, in SharedInteractiveData d){
        ItemChestData = default;
        WeaponChestData = default;
        ArmorChestData = default;
        CharmChestData = c;
        CutsceneInteractiveData = default;
        SharedInteractiveData = d;
        CurrentTypeId = TypeId.CharmChestData;
    }
    public PolyInteractiveData(in CutsceneInteractiveData c, in SharedInteractiveData d){
        ItemChestData = default;
        WeaponChestData = default;
        ArmorChestData = default;
        CharmChestData = default;
        CutsceneInteractiveData = c;
        SharedInteractiveData = d;
        CurrentTypeId = TypeId.CutsceneInteractiveData;
    }
}
