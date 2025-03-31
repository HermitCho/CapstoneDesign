using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeSkill : Skill
{
    [SerializeField] GameObject smokePrefab; //연막탄 프리팹
    GameObject smokeObject; // 연막탄 오브젝트
    float coolTime = 3f; // 연막탄 스킬 쿨타임
    float currentCoolTime = 0; // 현재 연막탄 스킬 쿨타임
    private int count = 2; // 연막탄 스킬 개수
    private PlayerInput playerInput; // 연막탄을 가진 캐릭터의 키인풋 컴포넌트
    Smoke smoke; //연막탄 프리팹
    HandlingWeapon handlingWeapon; //손에 든 무기에 관한 컴포넌트

    public GameObject smokePivot; // 연막탄 피벗

    // 연막탄 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolTime = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;

        playerInput = GetComponent<PlayerInput>();
        smokePivot = transform.GetChild(5).gameObject; //e스킬 자리의 투척류 피벗
        handlingWeapon = GetComponent<HandlingWeapon>();
    }

    // 스킬 키 입력 시
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        if (checkSkill == true)
        {
            base.inputSkillKey();
            if (checkSkill == true)
            {
                //연막탄을 처음 꺼냄(스킬키를 처음 누름) 혹은 연막탄을 던진 후 스킬 개수가 남아있을 때 스킬키 입력 시
                if (smokePivot.transform.childCount == 0)
                {
                    smokeObject = Instantiate(smokePrefab, smokePivot.transform.position, transform.rotation);
                    smokeObject.transform.parent = smokePivot.transform;
                    smoke = smokeObject.GetComponent<Smoke>();
                    Debug.Log("연막탄 장착!");

                    handlingWeapon.showGun = false;
                    handlingWeapon.controlPlayerShooter(false);
                }
                //연막탄 피벗에 이미 연막탄이 있을 경우
                else if (smokePivot.transform.childCount > 0)
                {
                    smokeObject.transform.parent = smokePivot.transform;
                    handlingWeapon.showGun = false;
                    handlingWeapon.controlPlayerShooter(false);
                }
            }
        }
    }

    // 스킬이 직접적으로 사용되는 함수
    public override void invokeSkill()
    {
        base.invokeSkill();
    }

    //스킬 쿨타임 관리 + 키 입력 인식
    void Update()
    {
        countCoolTime();

        if (currentCoolTime >= 0f && playerInput.skill_2_Button)
        {
            inputSkillKey();
        }

        if (smoke != null) // smoke가 null이 아닌 경우에만 Handling 호출
        {
            smoke.Handling();
            smoke.Throwing();
            if (smoke.state == Smoke.State.Fire)
            {
                invokeSkill();
            }
        }
        else
        {
            Debug.LogWarning("Grenade가 초기화되지 않았습니다.");
        }
    }
}
