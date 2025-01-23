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
    GameObject smokePivot;
    Smoke smoke;

    // 연막탄 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolTime = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;
        playerInput = GetComponent<PlayerInput>();
        Debug.Log(transform.GetChild(4).name);
        smokePivot = transform.GetChild(5).gameObject;
    }

    // 스킬 키 입력 시
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        if (checkSkill == true)
        {
            if (smokePivot.transform.childCount == 0)
            {
                smokeObject = Instantiate(smokePrefab, smokePivot.transform.position, transform.rotation);
                smokeObject.transform.parent = smokePivot.transform;
                smoke = smokeObject.GetComponent<Smoke>();
                Debug.Log("연막탄 장착!");
            }
        }
    }

    // 스킬이 직접적으로 사용되는 함수
    public override void invokeSkill()
    {
        base.invokeSkill();
        smoke.Throwing();
    }

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
            invokeSkill();
        }
        else
        {
            Debug.LogWarning("Grenade가 초기화되지 않았습니다.");
        }
    }
}
