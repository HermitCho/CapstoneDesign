using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : Skill
{
    float coolTime = 5f; // 회복 스킬 쿨타임
    private int count = 3; // 회복 스킬 개수
    int recoverHealth = 10; // 회복 스킬로 회복할 체력
    PlayerMovement playerMovement; // 회복 스킬을 가진 캐릭터의 플레이어 무브먼트 컴포넌트
    PlayerInput playerInput; // 회복 스킬을 가진 캐릭터의 플레이어 인풋 컴포넌트
    PlayerHealth playerHealth; // 회복 스킬을 가진 캐릭터의 플레이어 헬스 컴포넌트

    [Header("particle")]
    ParticleSystem particleSystem; // 회복 스킬의 파티클 시스템
    [SerializeField] GameObject particlePrefab; // 회복 스킬의 파티클 프리팹
    float particleDuration = 2f; // 회복 스킬 파티클 유지 시간간
    Animator animator; // 애니메이터 컴포넌트

    public override void OnEnable()
    {
        base.OnEnable();
        maxCoolDown = coolTime;
        maxSkillCount = count;
        currentSkillCount = maxSkillCount;

        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        particleSystem = GetComponent<ParticleSystem>();

        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<Animator>();
    }

    public override void inputSkillKey()
    {
        base.inputSkillKey();
        UIManager.Instance.SelectGunORSkillUI(1); // 인게임 UI에 수류탄 아이콘 표시, 스킬 2번 키를 눌렀으니 2 전송

        invokeSkill();
    }

    public override void invokeSkill()
    {
        base.invokeSkill();

        // 애니메이션 트리거 실행
        if (animator != null)
        {
            animator.SetTrigger("HandReach"); // 손을 앞으로 뻗는 애니메이션
        }

        RaycastHit? hitInfo = playerMovement.LocalPosToWorldRaycast();
        if (hitInfo.HasValue && hitInfo.Value.collider != null && hitInfo.Value.collider.CompareTag("Team")) // null 체크 및 Tag 비교 개선
        {
            // 팀원에게 힐
            PlayerHealth teamPlayerHealth = hitInfo.Value.collider.GetComponent<PlayerHealth>();
            if (teamPlayerHealth != null)
            {
                teamPlayerHealth.RestoreHealth(recoverHealth);
                if (skillSound != null)
                {
                    hitInfo.Value.collider.GetComponent<AudioSource>()?.PlayOneShot(skillSound);
                }
                count -= 1;
                
                // 팀원 위치에 파티클 프리팹 인스턴스화 및 재생
                Vector3 targetPosition = hitInfo.Value.collider.transform.position;
                StartCoroutine(PlayParticlePrefabAtPosition(targetPosition));
            }
        }
        else
        {
            // 자신에게 힐
            playerHealth.RestoreHealth(recoverHealth);
            if (skillSound != null)
            {
                audioSource?.PlayOneShot(skillSound);
            }
            count -= 1;
            
            // 자신의 위치에 파티클 프리팹 인스턴스화 및 재생
            Vector3 selfPosition = transform.position;
            StartCoroutine(PlayParticlePrefabAtPosition(selfPosition));
        }
    }

    // 지정된 위치에 파티클 프리팹을 인스턴스화하고 0.5초 후 제거
    private IEnumerator PlayParticlePrefabAtPosition(Vector3 position)
    {
        if (particlePrefab != null)
        {
            GameObject particleInstance = Object.Instantiate(particlePrefab, position, Quaternion.identity);
            
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                ps.Play();
                yield return new WaitForSeconds(particleDuration);
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                yield return new WaitForSeconds(particleDuration);
            }
            Destroy(particleInstance);
        }
    }

    private void Update()
    {
        skillbothCheck();

        if (playerInput.skill_1_Button && checkSkill == true)
        {
            inputSkillKey();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            this.GetComponent<PlayerHealth>().OnDamage(10, Vector3.zero, Vector3.zero);
        }
    }
}