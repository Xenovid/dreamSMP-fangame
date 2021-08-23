using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PolySkillData : IBufferElementData
{
	public enum TypeId
	{
		BasicPolySkill,
	}

	public SharedSkillData SharedSkillData;

	public BasicPolySkill BasicPolySkill;

	public TypeId CurrentTypeId;

	public PolySkillData(in BasicPolySkill c, in SharedSkillData d)
	{
		BasicPolySkill = c;
		CurrentTypeId = TypeId.BasicPolySkill;
		SharedSkillData = d;
	}


	public void UseSkill(Unity.Entities.EntityManager entityManager, Unity.Entities.Entity target, Unity.Entities.Entity user, ref SharedSkillData sharedSkillData)
	{
		switch (CurrentTypeId)
		{
			case TypeId.BasicPolySkill:
				//BasicPolySkill.UseSkill(entityManager, target, user, ref sharedSkillData);
				break;
		}
	}
}
