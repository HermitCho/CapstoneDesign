using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Skill", fileName = "Skill")]
public class Skill : ScriptableObject
{
    public float coolTime; //스킬 쿨타임
    public float currentCoolTime; //현재 스킬 쿨타임
    public float skillCount; //한 판에 쓸 수 있는 스킬 개수
}
