using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Flashbang : MonoBehaviour
{
    public enum State
    {
        Empty,
        Ready,
        Cooking,
        Fire
    }

    public State state { get; protected set; } // 섬광탄 파지 상태
    private PlayerInput playerInput; // 섬광탄을 든 해당 캐릭터의 키인풋을 받아옴
    private PlayerMovement playerMovement; // 섬광탄을 든 해당 캐릭터의 이동 컴포넌트
    private Rigidbody rigidbody; // 섬광탄의 리지드바디
    private Collider collider; // 섬광탄의 콜라이더
    [SerializeField] ParticleSystem explosionParticle; // 섬광탄 폭발 파티클
    private Animator animator; // 섬광탄을 던지는 캐릭터의 애니메이터

    public bool alreadyThrown; // 던져진 상태인지 확인
    float cookingTime = 15f; // 최대 섬광탄 쿠킹 시간 + 삭제까지 대기시간
    float throwingPower = 20f; // 섬광탄 투척 속도
    float throwingDelay = 2f; // 섬광탄을 던질 때 너무 빨리 던지면 isKinematic이 off되기 전에 바닥을 뚫고 지나감, 그래서 약간의 딜레이를 넣음
    bool exploded = false; // 섬광탄이 한 번 터지면 다시 함수가 반복되지 않도록 조절

    [SerializeField] private Image whiteImage; // 화면 전체를 덮는 흰색 이미지
    [SerializeField] private float flashDuration = 2f; // 섬광 효과 지속 시간
    [SerializeField] private float flashIntensity = 1f; // 섬광 효과 강도
    [SerializeField] private AudioClip Bang;
    AudioSource audioSource;

    LineRenderer lineRenderer; //섬광탄 투척 궤적을 그리기 위한 라인렌더러
    Transform throwingposition; //섬광탄 투척 위치

    void Start()
    {
        state = State.Empty;

        playerInput = GetComponentInParent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        alreadyThrown = false;
        animator = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>();

        lineRenderer = GetComponentInParent<LineRenderer>();
        throwingposition = transform.parent.transform;

        // 흰색 이미지 초기화
        if (whiteImage != null)
        {
            whiteImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Explosion();
    }

    // 2번 키를 누르면 섬광탄을 손에 들게 됨
    public void HandleOn()
    {
        if (state == State.Empty && playerInput.skill_1_Button && !alreadyThrown)
        {
            state = State.Ready;
            gameObject.SetActive(true);
        }
        else if (state == State.Ready && (playerInput.handleGunButton || playerInput.skill_2_Button) && !alreadyThrown)
        {
            state = State.Empty;
            animator.SetBool("HandleGrenade", false);
            gameObject.SetActive(false);
        }
    }

    // 섬광탄 쿠킹 및 투척을 위한 메서드
    public void Throwing()
    {
        if (state == State.Ready || state == State.Cooking)
        {
            if (Input.GetMouseButtonDown(0) && !alreadyThrown)
            {
                state = State.Cooking;
                animator.SetTrigger("PullOut"); // 수류탄 핀 뽑는 애니메이션 추가
                lineRenderer.enabled = true;
            }
            if (Input.GetMouseButton(0) && !alreadyThrown)
            {
                Vector3 grenadeVelocity = (throwingposition.forward).normalized * throwingPower;
                ShowTrajectLine(throwingposition.position + throwingposition.forward + throwingposition.up / 4, grenadeVelocity);
            }
            if (Input.GetMouseButtonUp(0) & !alreadyThrown)
            {
                lineRenderer.enabled = false;

                rigidbody.isKinematic = false;
                gameObject.transform.SetParent(null);
                animator.SetTrigger("ThrowGrenade");

                Vector3 fireDirection = transform.forward + transform.up / 4; //섬광탄이 날아갈 방향
                rigidbody.AddForce(fireDirection * throwingPower, ForceMode.Impulse);
                state = State.Fire;
                alreadyThrown = true;

                animator.SetBool("HandleGrenade", false);
            }
        }
    }

    //섬광탄이 터지는 것을 구현한 메서드
    void Explosion()
    {
        bool damaged = false;
        if (state == State.Cooking || state == State.Fire)
        {
            cookingTime -= Time.deltaTime;

            // explosionTime이 7초 근처에 도달했을 때 파티클 실행
            if (cookingTime <= 8f && cookingTime > 7.99f)
            {
                explosionParticle.Play();
                if (!exploded)
                {
                    Collider[] colls = Physics.OverlapSphere(transform.position, 30f);

                    for (int i = 0; i < colls.Length; i++)
                    {
                        if (colls[i].TryGetComponent<LivingEntity>(out var damageable))
                        {
                            Vector3 toPlayer = (colls[i].transform.position - transform.position).normalized;
                            Vector3 playerForward = colls[i].transform.forward;

                            float dotProduct = Vector3.Dot(playerForward, toPlayer);

                            audioSource.clip = Bang;
                            audioSource.Play();
                            AudioSource iAudio = colls[i].GetComponent<AudioSource>();

                            // 플레이어의 움직임과 사격을 제한
                            PlayerMovement hitPlayerMovement = colls[i].GetComponent<PlayerMovement>();
                            PlayerShooter hitPlayerShooter = colls[i].GetComponent<PlayerShooter>();
                            HandlingWeapon hitHandlingWeapon = colls[i].GetComponent<HandlingWeapon>();
                            
                            if (hitPlayerMovement != null && hitPlayerShooter != null)
                            {
                                StartCoroutine(DisablePlayerActions(hitPlayerMovement, hitPlayerShooter, hitHandlingWeapon));
                                StartCoroutine(FlashEffect(dotProduct < 0));
                            }
                        }
                    }
                    exploded = true;
                }
            }
            // explosionTime이 9초 근처에 도달했을 때 오브젝트 파괴
            if (cookingTime <= 0f)
            {
                Destroy(gameObject); // 이 오브젝트를 파괴
            }
        }
    }

    private IEnumerator FlashEffect(bool isStrongEffect)
    {
        if (whiteImage == null) yield break;

        float duration = isStrongEffect ? flashDuration : flashDuration * 0.5f;
        float intensity = isStrongEffect ? flashIntensity : flashIntensity * 0.5f;
        float elapsed = 0f;

        // 페이드 인
        while (elapsed < duration * 0.2f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, intensity, elapsed / (duration * 0.2f));
            whiteImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < duration * 0.8f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(intensity, 0f, elapsed / (duration * 0.8f));
            whiteImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        whiteImage.color = new Color(1f, 1f, 1f, 0f);
    }

    private IEnumerator DisablePlayerActions(PlayerMovement movement, PlayerShooter shooter, HandlingWeapon handlingWeapon)
    {
        // 원래 상태 저장
        bool originalMovementEnabled = movement.enabled;
        bool originalShooterEnabled = shooter.enabled;
        bool originalHandlingWeaponEnabled = handlingWeapon.enabled;

        // 컴포넌트 비활성화
        movement.enabled = false;
        shooter.enabled = false;
        handlingWeapon.enabled = false;

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 컴포넌트 다시 활성화
        movement.enabled = originalMovementEnabled;
        shooter.enabled = originalShooterEnabled;
        handlingWeapon.enabled = originalHandlingWeaponEnabled;
    }

    void ShowTrajectLine(Vector3 origin, Vector3 speed)
    {
        Vector3[] points = new Vector3[100];
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;
            points[i] = origin + speed * time + Physics.gravity * time * time / 2f;
        }
        lineRenderer.SetPositions(points);
    }
}




/*

 섬광탄 들어 -> 핀 뽑고 -> 던지고 -> 터지고
 포물선을 그리면서 나가야되니까


*/