using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 해당 스크립트는 '레트로의 유니티 게임 프로그래밍 에센스 개정판'의 스크립트를 수정함

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity
{
    public Skill[] skills; // 플레이어 캐릭터의 스킬
    public PlayerCharacter playerCharacter; //플레이어 캐릭터 종류
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더
    public Slider shieldSlider; // 추가 방어막을 표시할 UI 슬라이더
    public Slider energySlider; // 기력을 표시할 UI 슬라이더

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerInput playerInput; // 플레이어 키 바인드 컴포넌트
    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트

    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake()
    {
        // 사용할 컴포넌트를 가져오기
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);

        startingHealth = playerCharacter.maxHealth;
        health = startingHealth;
        startingShield = playerCharacter.maxShield;
        shield = startingShield;


        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
        shieldSlider.maxValue = startingShield;
        shieldSlider.value = shield;

        moveSpeed = playerCharacter.defaultMoveSpeed;
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);
        healthSlider.value = health;
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            playerAudioPlayer.PlayOneShot(playerCharacter.hitClip);
        }
        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection);
        healthSlider.value = health;
        shieldSlider.value = shield;
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();
        healthSlider.gameObject.SetActive(false);
        shieldSlider.gameObject.SetActive(false);
        energySlider.gameObject.SetActive(false);
        playerAudioPlayer.PlayOneShot(playerCharacter.deathClip);
        playerAnimator.SetTrigger("Die");

        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    private Vector3 lastScreenPosition;

    void LateUpdate()
    {
        Vector3 targetPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2.4f, 0));

        // 카메라가 급격히 회전하면 즉시 이동, 아니면 Lerp 적용
        if (Vector3.Distance(lastScreenPosition, targetPosition) > Screen.width * 0.07f)
        {
            healthSlider.transform.position = targetPosition; // 즉시 반응
        }
        else
        {
            healthSlider.transform.position = Vector3.Lerp(healthSlider.transform.position, targetPosition, Time.deltaTime * 20f);
        }

        lastScreenPosition = targetPosition; // 현재 위치 저장
    }
}