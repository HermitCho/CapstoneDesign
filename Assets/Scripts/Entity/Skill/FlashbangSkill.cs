using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbangSkill : Skill
{
    public FlashbangSkill()
    {
        skillType = SkillType.count;
    }

    [SerializeField] GameObject flashbangPrefab; //섬광탄 프리팹 데이터
    GameObject flashbangObject; //직접 던져질 섬광탄 오브젝트
    Flashbang flashbang; //섬광탄 스크립트

    private int count = 2; // 섬광탄 스킬 개수
    private PlayerInput playerInput; // 섬광탄을 가진 캐릭터의 키인풋 컴포넌트


    HandlingWeapon handlingWeapon; //손에 든 무기에 관한 컴포넌트

    public GameObject flashbangPivot; // 섬광탄 피벗

    // 섬광탄 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
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
            UIManager.Instance.SelectGunORSkillUI(2); // 인게임 UI에 수류탄 아이콘 표시, 스킬 2번 키를 눌렀으니 2 전송

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
        skillCountCheck();

        if (currentCoolDown >= 0f && playerInput.skill_2_Button)
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
