using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbangSkill : Skill
{
    [SerializeField] GameObject flashbangPrefab; //섬광탄 프리팹
    GameObject flashbangObject; // 섬광탄 오브젝트
    float coolTime = 3f; // 섬광탄 스킬 쿨타임
    float currentCoolTime = 0; // 현재 섬광탄 스킬 쿨타임
    private int count = 2; // 섬광탄 스킬 개수
    private PlayerInput playerInput; // 섬광탄을 가진 캐릭터의 키인풋 컴포넌트
    Flashbang flashbang; //섬광탄 프리팹
    HandlingWeapon handlingWeapon; //손에 든 무기에 관한 컴포넌트

    public GameObject flashbangPivot; // 섬광탄 피벗

    // 섬광탄 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolTime = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;
        playerInput = GetComponent<PlayerInput>();
        Debug.Log(transform.GetChild(4).name);
        flashbangPivot = transform.GetChild(5).gameObject; //e스킬 자리의 투척류 피벗
        handlingWeapon = GetComponent<HandlingWeapon>();
    }

    // 스킬 키 입력 시
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        if (checkSkill == true)
        {
            if (flashbangPivot.transform.childCount == 0)
            {
                flashbangObject = Instantiate(flashbangPrefab, flashbangPivot.transform.position, transform.rotation);
                flashbangObject.transform.parent = flashbangPivot.transform;
                flashbang = flashbangObject.GetComponent<Flashbang>();
                Debug.Log("섬광탄 장착!");

                handlingWeapon.showGun = false;
                handlingWeapon.controlPlayerShooter(false);
            }
            else if (flashbangPivot.transform.childCount > 0)
            {
                flashbangPivot.transform.parent = flashbangPivot.transform;
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

    void Update()
    {
        countCoolTime();

        if (currentCoolTime >= 0f && playerInput.skill_2_Button)
        {
            inputSkillKey();
        }

        if (flashbang != null) // flashbang가 null이 아닌 경우에만 Handling 호출
        {
            flashbang.Handling();
            flashbang.Throwing();
            if (flashbang.state == Flashbang.State.Fire)
            {
                invokeSkill();
            }
        }
        else
        {
            Debug.LogWarning("Flashbang가 초기화되지 않았습니다.");
        }
    }
}
