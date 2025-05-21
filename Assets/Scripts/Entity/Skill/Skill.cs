using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    /// <summary>
    /// 스킬 타입
    /// instantCount - 바로 나가는 카운트형 스킬
    /// instantCooldown - 바로 나가는 쿨타임형 스킬
    /// count - 추가 조작이 필요한 카운트형 스킬
    /// cooldown - 추가 조작이 필요한 쿨타임형 스킬
    /// countCooldown - 카운트와 쿨타임이 둘 다 필요한 스킬
    /// </summary>
    public enum SkillType
    {
        instantCount,
        instantCooldown,
        count,
        cooldown,
        countCooldown
    };

    private LivingEntity livingEntity; //스킬을 가질 캐릭터의 리빙엔티티(캐릭터가 살아있는지 확인)

    public SkillType skillType { get; protected set; } // 스킬 타입

    //스킬이 쿨타임 타입인 경우
    public float maxCoolDown { get; protected set; } // 최대 쿨타임(스킬 사용 시 줄어들 때 몇 초만큼 기다릴지)
    public float currentCoolDown { get; protected set; } // 현재 쿨타임

    // 스킬이 카운트 타입인 경우
    public int maxSkillCount { get; protected set; } // 한 판당 사용 가능 횟수
    public int currentSkillCount { get; protected set; } // 현재 사용 가능 횟수

    public bool checkSkill = false; // 스킬이 현재 사용가능한지 확인


    // 시작할 때 스킬 초기화
    public virtual void OnEnable()
    {
        currentCoolDown = 0;
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
            currentCoolDown = maxCoolDown;
            currentSkillCount--;
            checkSkill = false;
        }
    }

    // 쿨타임 스킬 사용 가능 확인 및 쿨타임이 도는 것 구현(해당 메서드는 각 스킬 Update()문에 넣어줘야 함)
    public void skillCoolDownCheck()
    {
        // 쿨타임이 0초보다 크면 시간이 지나는 만큼 감소
        if (currentCoolDown > 0f)
        {
            currentCoolDown -= Time.deltaTime;
            //Debug.Log(currentCoolDown);
        }
        //스킬 쿨타임이 0이고, 스킬 횟수가 있는 경우에만 스킬 사용 가능 확인
        else if (currentCoolDown <= 0f)
        {
            checkSkill = true;
        }
    }

    // 카운트 스킬의 사용 가능 횟수 확인
    public void skillCountCheck()
    {
        if (currentSkillCount > 0)
        {
            checkSkill = true;
        }
    }

    // 스킬 카운트와 쿨타임이 둘 다 사용 조건을 만족하는지 확인
    public void skillbothCheck()
    {
        // Debug.Log("currentSkillCount : " + currentSkillCount);
        // Debug.Log("currentCoolDown : " + currentCoolDown);
        if (currentSkillCount > 0)
        {
            if (currentCoolDown <= 0f)
            {
                Debug.Log("스킬 사용 가능");
                checkSkill = true;
            }
            else
            {
                currentCoolDown -= Time.deltaTime;
            }
        }
    }
}
