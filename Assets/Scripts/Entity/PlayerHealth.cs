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

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerInput playerInput; // 플레이어 키 바인드 컴포넌트
    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트

    /// <summary>
    /// 플레이어 슈터는 구현되면 연동
    /// </summary>
    //private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake()
    {
        // 사용할 컴포넌트를 가져오기
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        //playerShooter = GetComponent<PlayerShooter>(); //구현 x라 주석처리
    }

    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);

        startingHealth = playerCharacter.maxHealth;
        health = startingHealth;

        healthSlider.maxValue = health;
        healthSlider.value = health;
        shieldSlider.maxValue = shield;
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
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();
        healthSlider.gameObject.SetActive(false);
        playerAudioPlayer.PlayOneShot(playerCharacter.deathClip);
        playerAnimator.SetTrigger("Die");

        playerMovement.enabled = false;
        //playerShooter.enabled = false;//구현 x라 주석처리
    }

    //업데이트
    void Update()
    {

    }
}