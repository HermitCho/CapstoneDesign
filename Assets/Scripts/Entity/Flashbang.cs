using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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

    bool alreadyThrown; // 던져진 상태인지 확인
    float cookingTime = 15f; // 최대 섬광탄 쿠킹 시간 + 삭제까지 대기시간
    float throwingPower = 20f; // 섬광탄 투척 속도
    float throwingDelay = 2f; // 섬광탄을 던질 때 너무 빨리 던지면 isKinematic이 off되기 전에 바닥을 뚫고 지나감, 그래서 약간의 딜레이를 넣음
    bool exploded = false; // 섬광탄이 한 번 터지면 다시 함수가 반복되지 않도록 조절

    Image whiteImage;
    [SerializeField] AudioClip WhiteNoise;
    [SerializeField] AudioClip Bang;
    AudioSource audioSource;


    LineRenderer lineRenderer; //섬광탄 투척 궤적을 그리기 위한 라인렌더러
    Transform throwingposition; //섬광탄 투척 위치


    void Start()
    {
        state = State.Ready;

        playerInput = GetComponentInParent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        alreadyThrown = false;
        animator = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>();
        whiteImage = GameObject.FindGameObjectWithTag("WhiteImage").GetComponent<Image>();

        lineRenderer = GetComponentInParent<LineRenderer>();
        throwingposition = transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Explosion();
    }

    // 2번 키를 누르면 섬광탄을 손에 들게 됨
    public void Handling()
    {
        if (state == State.Empty && playerInput.skill_2_Button && !alreadyThrown)
        {
            state = State.Ready;
            animator.SetBool("isHandleFlashbang", true);
            gameObject.SetActive(true);

        }
        else if (state == State.Ready && (playerInput.skill_1_Button || playerInput.handleGunButton) && !alreadyThrown)
        {
            state = State.Empty;
            animator.SetBool("isHandleFlashbang", false);
            gameObject.SetActive(false);
        }
    }

    // 섬광탄 쿠킹 및 투척을 위한 메서드
    public void Throwing()
    {
        ///////////////Debug.Log(cookingTime);
        if (state == State.Ready || state == State.Cooking)
        {
            if (Input.GetMouseButton(0) && !alreadyThrown)
            {
                state = State.Cooking;
                animator.SetTrigger("PullOut");

                Vector3 grenadeVelocity = (throwingposition.forward).normalized * throwingPower;
                ShowTrajectLine(throwingposition.position + throwingposition.forward + throwingposition.up / 4, grenadeVelocity);
            }
            if (Input.GetMouseButtonUp(0) & !alreadyThrown)
            {
                lineRenderer.enabled = false;

                rigidbody.isKinematic = false;
                gameObject.transform.SetParent(null);
                animator.SetTrigger("Throwing");

                Vector3 fireDirection = transform.forward + transform.up / 4; //섬광탄이 날아갈 방향
                rigidbody.AddForce(fireDirection * throwingPower, ForceMode.Impulse);
                state = State.Fire;
                alreadyThrown = true;
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

                            if (dotProduct < 0) // 0보다 크면 앞쪽에 있음
                            {
                                
                                Debug.Log("플레이어가 섬광탄 앞쪽에 있음! 더 강한 효과");
                                StartCoroutine(WhiteFade(iAudio));
                            }
                            else
                            {
                                Debug.Log("플레이어가 섬광탄 뒤쪽에 있음! 더 약한 효과");
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

    private IEnumerator WhiteFade(AudioSource audio)
    {
        whiteImage.color = new Vector4(1, 1, 1, 1);
        audio.clip = WhiteNoise;
        audio.Play();

        float FadeSpeed = 1f;
        float Modifier = 0.01f;
        float WaitTime = 0;

        for (int i = 0; whiteImage.color.a > 0; i++)
        {
            whiteImage.color = new Vector4(1, 1, 1, FadeSpeed);
            FadeSpeed = FadeSpeed - 0.025f;
            Modifier = Modifier * 1.5f;
            WaitTime = 0.5f - Modifier;
            if (WaitTime < 0.1f) WaitTime = 0.1f;
            audio.volume -= 0.05f;
            yield return new WaitForSeconds(WaitTime);
        }

        audio.Stop();
        audio.volume = 1;
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