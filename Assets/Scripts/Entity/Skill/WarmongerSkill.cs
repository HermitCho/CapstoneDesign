using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 이름은 전쟁광 총기 연사 속도, 재장전 속도를 빠르게 하는 스킬임.
/// </summary>
public class WarmongerSkill : Skill
{

    public WarmongerSkill()
    {
        skillType = SkillType.instantCooldown;
    }

    //스킬 직접 관련
    float coolTime = 10f; // 전쟁광 스킬 사용 시 쿨타임
    float currentCoolDown = 0; // 전쟁광 스킬 현재 쿨타임

    bool onSkill = false; // 전쟁광 스킬 사용 중인지 확인
    float skillDuration = 5f; // 전쟁광 스킬 지속 시간
    float nowSkillDuration = 0f; // 전쟁광 스킬 현재 지속 시간

    ParticleSystem particleSystem; // 전쟁광 스킬 사용 파티클


    // 스킬 간접 관련
    PlayerInput playerInput; // 캐릭터의 키인풋 컴포넌트
    PlayerMovement playerMovement; // 캐릭터 움직임 컴포넌트

    [SerializeField] Gun gun; // 사용 캐릭터의 총기
    GunData gunData; // 전쟁광 스킬로 변할 캐릭터 총기 데이터



    // 전쟁광 스킬 초기화
    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolDown = coolTime; // 최대 쿨타임 설정


        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();

        gunData = gun.gunData;

        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.time = skillDuration;
    }

    // 스킬 키 입력 시
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        if (checkSkill == true)
        {
            invokeSkill();
        }
    }

    // 스킬이 직접적으로 사용되는 함수
    public override void invokeSkill()
    {
        base.invokeSkill();

        UIManager.Instance.CoolDownButtonInput(2); // 아이콘 업데이트

        onSkill = true;
        gunData.reloadTime = gunData.reloadTime / 2;
        playerMovement.verticalMoveSpeed = playerMovement.verticalMoveSpeed * 2;
        playerMovement.horizontalMoveSpeed = playerMovement.horizontalMoveSpeed * 2;
        playerMovement.sprintSpeed = playerMovement.sprintSpeed * 2;
        particleSystem.Play();
    }

    //스킬 쿨타임 관리와 스킬 지속 시간 관리 + 키 입력 인식
    void Update()
    {
        skillCoolDownCheck();
        SkillDurating();

        if (currentCoolDown >= 0f && playerInput.skill_2_Button)
        {
            inputSkillKey();
        }
    }
    
    //스킬 지속 관리 함수
    void SkillDurating()
    {
        if (nowSkillDuration < skillDuration && onSkill)
        {
            nowSkillDuration += Time.deltaTime;
        }
        else if(nowSkillDuration >= skillDuration)
        {
            Debug.Log("전쟁광 스킬 종료!");
            
            gunData.reloadTime = gunData.reloadTime * 2;
            playerMovement.verticalMoveSpeed = playerMovement.verticalMoveSpeed / 2;
            playerMovement.horizontalMoveSpeed = playerMovement.horizontalMoveSpeed / 2;
            playerMovement.sprintSpeed = playerMovement.sprintSpeed / 2;

            Debug.Log(gunData.reloadTime);
            Debug.Log(playerMovement.verticalMoveSpeed);
            Debug.Log(playerMovement.horizontalMoveSpeed);
            Debug.Log(playerMovement.sprintSpeed);

            nowSkillDuration = 0f;
            onSkill = false;
        }
    }
}