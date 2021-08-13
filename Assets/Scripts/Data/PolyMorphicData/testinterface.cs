using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class testinterface : MonoBehaviour
{
    [SerializeReference]
    public List<IPolySkillData> test = new List<IPolySkillData>(){
        new BasicPolySkill()
    };
}
