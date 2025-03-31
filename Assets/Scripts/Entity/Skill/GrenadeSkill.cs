using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSkill : Skill
{
    [SerializeField] GameObject grenadePrefab; //수류탄 프리팹
    GameObject grenadeObject; // 수류탄 오브젝트
    float coolTime = 3f; // 수류탄 스킬 쿨타임
    private int count = 3; // 수류탄 스킬 개수
    private PlayerInput playerInput; // 수류탄을 가진 캐릭터의 키인풋 컴포넌트
    Grenade grenade; //수류탄 프리팹
    HandlingWeapon handlingWeapon; //손에 든 무기에 관한 컴포넌트
    bool showGrenade = false;

    public GameObject grenadePivot; // 수류탄 피벗

    // 수류탄 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolTime = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;


        playerInput = GetComponent<PlayerInput>();
        grenadePivot = transform.GetChild(4).gameObject; //q스킬 자리의 투척류 피벗
        handlingWeapon = GetComponent<HandlingWeapon>();
    }

    // 스킬 키 입력 시
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        if (checkSkill == true)
        {
            //수류탄을 처음 꺼냄(스킬키를 처음 누름) 혹은 수류탄을 던진 후 스킬 개수가 남아있을 때 스킬키 입력 시
            if (grenadePivot.transform.childCount == 0)
            {
                grenadeObject = Instantiate(grenadePrefab, grenadePivot.transform.position, transform.rotation);
                grenadeObject.transform.parent = grenadePivot.transform;
                grenade = grenadeObject.GetComponent<Grenade>();
                Debug.Log("수류탄 장착!");

                handlingWeapon.showGun = false;
                handlingWeapon.controlPlayerShooter(false);
            }
            //수류탄 피벗에 이미 수류탄이 있을 경우
            else if (grenadePivot.transform.childCount > 0)
            {
                grenadeObject.transform.parent = grenadePivot.transform;
                handlingWeapon.showGun = false;
                handlingWeapon.controlPlayerShooter(false);
            }
        }
    }

    // 스킬이 직접적으로 사용되는 함수
    public override void invokeSkill()
    {
        base.invokeSkill();
    }

    //스킬 쿨타임 제어 + 수류탄 제어
    void Update()
    {
        countCoolTime();

        if (currentCoolTime <= 0f && playerInput.skill_1_Button)
        {
            inputSkillKey();
        }

        if (grenade != null) // grenade가 null이 아닌 경우에만 Handling 호출
        {
            grenade.Handling();
            grenade.Throwing();
            if (grenade.state == Grenade.State.Fire)
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
