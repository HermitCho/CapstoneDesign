using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private LivingEntity livingEntity; //스킬을 가질 캐릭터의 리빙엔티티

    public float maxCoolTime { get; protected set; } // 최대 쿨타임(스킬 사용 시 줄어들 때 몇 초만큼 기다릴지)
    public float currentCoolTime { get; protected set; } // 현재 쿨타임
    public int maxSkillCount { get; protected set; } // 한 판당 사용 가능 횟수
    public int currentSkillCount { get; protected set; } // 현재 사용 가능 횟수
    public bool checkSkill = false; // 스킬이 사용가능한지 확인


    // 시작할 때 스킬 초기화
    public virtual void OnEnable()
    {
        livingEntity = GetComponent<LivingEntity>();

        // 해당 스킬을 지닌 캐릭터가 살아있는 경우에 쿨타임, 사용 가능 횟수 초기화
        if (livingEntity.dead != true)
        {
            Debug.Log("스킬 초기화");
            currentCoolTime = 0f;
            currentSkillCount = maxSkillCount;
        }
    }

    // 스킬 버튼 클릭 시 조건을 확인하는 메서드
    public virtual void inputSkillKey()
    {

    }

    // 스킬을 직접 사용하는 메서드
    public virtual void invokeSkill()
    {
        // 스킬 사용가능 시, 사용하면 쿨타임이 도는 것과 사용 횟수가 주는 것만 구현
        if (checkSkill == true)
        {
            currentCoolTime = maxCoolTime;
            Debug.Log("스킬 사용 ,     " + currentCoolTime);
            currentSkillCount--;
            checkSkill = false;
        }
    }

    // 쿨타임이 도는 것 구현(해당 메서드는 각 스킬 Update()문에 넣어줘야 함)
    public void countCoolTime()
    {
        // 쿨타임이 0초보다 크면 시간이 지나는 만큼 감소
        if (currentCoolTime > 0f && currentSkillCount > 0)
        {
            currentCoolTime -= Time.deltaTime;
            //Debug.Log(currentCoolTime);
        }
        //스킬 쿨타임이 0이고, 스킬 횟수가 있는 경우에만 스킬 사용 가능 확인
        else if (currentCoolTime <= 0f && currentSkillCount > 0)
        {
            checkSkill = true;
        }
    }
}
