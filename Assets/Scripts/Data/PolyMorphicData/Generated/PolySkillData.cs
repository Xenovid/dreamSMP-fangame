using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct PolySkillData : IBufferElementData
{
	public enum TypeId
	{
		BasicPolySkill,
		ThrowablePolySkill,
	}

	public SharedSkillData SharedSkillData;

	public BasicPolySkill BasicPolySkill;
	public ThrowablePolySkill ThrowablePolySkill;

	public TypeId CurrentTypeId;

	public PolySkillData(in BasicPolySkill c, in SharedSkillData d)
	{
		ThrowablePolySkill = default;
		BasicPolySkill = c;
		CurrentTypeId = TypeId.BasicPolySkill;
		SharedSkillData = d;
	}

	public PolySkillData(in ThrowablePolySkill c, in SharedSkillData d)
	{
		BasicPolySkill = default;
		ThrowablePolySkill = c;
		CurrentTypeId = TypeId.ThrowablePolySkill;
		SharedSkillData = d;
	}


	public void UseSkill(System.Int32 skillNumber, Unity.Entities.EntityCommandBuffer ecb, UnityEngine.Animator animator, Unity.Entities.Entity target, Unity.Entities.Entity user, ref SharedSkillData sharedSkillData)
	{
		switch (CurrentTypeId)
		{
			case TypeId.BasicPolySkill:
				BasicPolySkill.UseSkill(skillNumber, ecb, animator, target, user, ref sharedSkillData);
				break;
			case TypeId.ThrowablePolySkill:
				ThrowablePolySkill.UseSkill(skillNumber, ecb, animator, target, user, ref sharedSkillData);
				break;
		}
	}

	public void GetStrings()
	{
		switch (CurrentTypeId)
		{
			case TypeId.BasicPolySkill:
				BasicPolySkill.GetStrings();
				break;
			case TypeId.ThrowablePolySkill:
				ThrowablePolySkill.GetStrings();
				break;
		}
	}
}
