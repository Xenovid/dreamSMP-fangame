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


	public void Update(System.Single deltaTime)
	{
		switch (CurrentTypeId)
		{
			case TypeId.BasicPolySkill:
				BasicPolySkill.Update(deltaTime);
				break;
		}
	}
}
