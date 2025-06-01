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
    // float nowSkillDuration = 0f; // 코루틴 사용 시 필요 없음

    [Header("Particle")]
    [SerializeField] GameObject skillParticlePrefab; // 전쟁광 스킬 파티클 프리팹
    private ParticleSystem skillParticleInstance; // 인스턴스화된 파티클 시스템


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
        audioSource = GetComponent<AudioSource>();

        gunData = gun.gunData;
    }

    // 스킬 키 입력 시
    public override void inputSkillKey()
    {
        base.inputSkillKey();
        // 스킬이 사용 가능하고 현재 스킬이 사용 중이 아닐 때만 발동
        if (checkSkill == true && !onSkill)
        {
            invokeSkill();
        }
    }

    // 스킬이 직접적으로 사용되는 함수
    public override void invokeSkill()
    {
        base.invokeSkill();

        UIManager.Instance.CoolDownButtonInput(2); // 아이콘 업데이트

        // onSkill 플래그는 코루틴 시작 시 설정
        // 능력치 변경
        gunData.reloadTime = gunData.reloadTime / 2;
        playerMovement.verticalMoveSpeed = playerMovement.verticalMoveSpeed * 1.2f;
        playerMovement.horizontalMoveSpeed = playerMovement.horizontalMoveSpeed * 1.2f;
        playerMovement.sprintSpeed = playerMovement.sprintSpeed * 1.2f;

        Debug.Log(skillSound);
        audioSource.PlayOneShot(skillSound);
        // 스킬 지속 시간 코루틴 시작
        StartCoroutine(WarmongerDurationCoroutine());
    }

    //스킬 쿨타임 관리와 스킬 지속 시간 관리 + 키 입력 인식
    void Update()
    {
        skillCoolDownCheck();

        // 쿨타임 체크는 기존대로 유지
        if (currentCoolDown >= 0f && playerInput.skill_2_Button)
        {
             // inputSkillKey() 안에서 onSkill 체크하여 발동 제어
             inputSkillKey();
        }
    }

    //스킬 지속 관리 코루틴
    private IEnumerator WarmongerDurationCoroutine()
    {
        onSkill = true; // 스킬 사용 중 설정

        // 파티클 인스턴스 생성 및 캐릭터에 고정
        if (skillParticlePrefab != null)
        {
            // 캐릭터의 자식으로 생성하여 위치 고정
            GameObject go = Instantiate(skillParticlePrefab, this.transform.position, Quaternion.identity, this.transform);
            skillParticleInstance = go.GetComponent<ParticleSystem>();
            if (skillParticleInstance != null)
                skillParticleInstance.Play();
        }

        // 스킬 지속 시간만큼 대기
        yield return new WaitForSeconds(skillDuration);

        // 스킬 종료
        Debug.Log("전쟁광 스킬 종료!");

        // 능력치 원상 복구
        gunData.reloadTime = gunData.reloadTime * 2;
        playerMovement.verticalMoveSpeed = playerMovement.verticalMoveSpeed / 1.2f;
        playerMovement.horizontalMoveSpeed = playerMovement.horizontalMoveSpeed / 1.2f;
        playerMovement.sprintSpeed = playerMovement.sprintSpeed / 1.2f;

        // Debug.Log(gunData.reloadTime);
        // Debug.Log(playerMovement.verticalMoveSpeed);
        // Debug.Log(playerMovement.horizontalMoveSpeed);
        // Debug.Log(playerMovement.sprintSpeed);

        // 파티클 정지 및 제거
        if (skillParticleInstance != null)
        {
            skillParticleInstance.Stop();
            Destroy(skillParticleInstance.gameObject);
            skillParticleInstance = null; // 참조 해제
        }

        onSkill = false; // 스킬 사용 종료 설정
    }
}