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


	public void Update(System.Single deltaTime, Unity.Entities.EntityManager entityManager, ref SharedSkillData sharedSkillData)
	{
		switch (CurrentTypeId)
		{
			case TypeId.BasicPolySkill:
				BasicPolySkill.Update(deltaTime, entityManager, ref sharedSkillData);
				break;
		}
	}

	public void UseSkill(UnityEngine.Animator animator, Unity.Entities.EntityManager entityManager, ref SharedSkillData sharedSkillData)
	{
		switch (CurrentTypeId)
		{
			case TypeId.BasicPolySkill:
				BasicPolySkill.UseSkill(animator, entityManager, ref sharedSkillData);
				break;
		}
	}
}
