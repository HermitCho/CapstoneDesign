using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSkill : Skill
{
    // 스킬 타입을 카운트 타입으로 설정
    public GrenadeSkill()
    {
        skillType = SkillType.count;
    }

    [SerializeField] GameObject grenadePrefab; //수류탄 프리팹
    GameObject grenadeObject; // 수류탄 오브젝트
    private int count = 3; // 수류탄 스킬 개수
    private PlayerInput playerInput; // 수류탄을 가진 캐릭터의 키인풋 컴포넌트
    Grenade grenade; //수류탄 프리팹
    HandlingWeapon handlingWeapon; //손에 든 무기에 관한 컴포넌트

    public GameObject grenadePivot; // 수류탄 피벗

    // 수류탄 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;

        playerInput = GetComponent<PlayerInput>();
        handlingWeapon = GetComponent<HandlingWeapon>();
    }

    // 스킬 키 입력 시
    public override void inputSkillKey()
    {
        skillCountCheck();
        base.inputSkillKey();
        if (checkSkill == true)
        {
            UIManager.Instance.SelectGunORSkillUI(1); // 인게임 UI에 수류탄 아이콘 표시, 스킬 1번 키를 눌렀으니 1 전송

            //수류탄을 처음 꺼냄(스킬키를 처음 누름) 혹은 수류탄을 던진 후 스킬 개수가 남아있을 때 스킬키 입력 시
            if (grenadePivot.transform.childCount == 0)
            {
                grenadeObject = Instantiate(grenadePrefab, grenadePivot.transform.position, transform.rotation);
                grenadeObject.transform.parent = grenadePivot.transform;
                grenade = grenadeObject.GetComponent<Grenade>();

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
        if (currentSkillCount > 0 && playerInput.skill_1_Button)
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
