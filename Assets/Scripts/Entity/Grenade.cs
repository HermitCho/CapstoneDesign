using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    /// <summary>
    /// 수류탄 현 상태를 구분하기 위한 enum
    /// 
    /// Empty : 수류탄을 들지 않은 상태
    /// Ready : 수류탄을 들고 있는 상태
    /// Cooking : 수류탄을 던지기 위해 핀을 뽑은 상태
    /// Fire : 수류탄을 던진 상태
    /// 
    /// </summary>
    public enum State
    {
        Empty,
        Ready,
        Cooking,
        Fire
    }


    public State state { get; protected set; } // 현재 수류탄 파지 상태

    private Rigidbody rigidbody; // 수류탄의 리지드바디
    private Collider collider; // 수류탄의 콜라이더
    [SerializeField] ParticleSystem explosionParticle; // 수류탄 폭발 파티클
    private Animator animator; // 수류탄을 던지는 캐릭터의 애니메이터

    bool alreadyThrown = false; // 던져진 상태인지 확인
    int damage = 180; //수류탄 데미지
    float cookingTime = 9f; // 최대 수류탄 쿠킹 시간 + 삭제까지 대기시간
    float throwingPower = 25f; // 수류탄 투척 속도
    float throwingDelay = 2f; // 수류탄을 던질 때 너무 빨리 던지면 isKinematic이 off되기 전에 바닥을 뚫고 지나감, 그래서 약간의 딜레이를 넣음
    bool exploded = false; // 수류탄이 한 번 터지면 다시 함수가 반복되지 않도록 조절

    private PlayerInput playerInput; // 수류탄을 든 해당 캐릭터의 키인풋을 받아옴



    // 수류탄 투척 궤적
    LineRenderer lineRenderer; //수류탄 투척 궤적을 그리기 위한 라인렌더러
    Transform throwingposition; // 수튜탄 투척 위치

    void Start()
    {
        state = State.Ready;

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        animator = GetComponentInParent<Animator>();
        playerInput = GetComponentInParent<PlayerInput>();

        lineRenderer = GetComponentInParent<LineRenderer>();
        throwingposition = transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Explosion();
    }

    // '스킬 1' 키를 누르면 수류탄을 들게 됨 / '스킬 2' 혹은 '총' 키를 누르면 수류탄을 넣음
    public void Handling()
    {
        if (state == State.Empty && playerInput.skill_1_Button && !alreadyThrown)
        {
            state = State.Ready;
            //animator.SetBool("isHandleGrenade", true); 수류탄 드는 애니메이션 추가 시 사용
            gameObject.SetActive(true);

        }
        else if (state == State.Ready && (playerInput.skill_2_Button || playerInput.handleGunButton) && !alreadyThrown)
        {
            state = State.Empty;
            //animator.SetBool("isHandleGrenade", false); 수류탄 드는 애니메이션 추가 시 사용
            Debug.Log(state);
            gameObject.SetActive(false);

        }
    }

    // 수류탄 쿠킹 및 투척을 위한 메서드
    public void Throwing()
    {
        if (state == State.Ready || state == State.Cooking)
        {
            if (Input.GetMouseButton(0) && !alreadyThrown)
            {
                state = State.Cooking;
                //animator.SetTrigger("PullOut"); 수류탄 핀 뽑는 애니메이션 추가 시 사용
                lineRenderer.enabled = true;
                Debug.Log(state);

                Vector3 grenadeVelocity = (throwingposition.forward).normalized * throwingPower;
                ShowTrajectLine(throwingposition.position + throwingposition.forward + throwingposition.up / 4, grenadeVelocity);
            }

            if (Input.GetMouseButtonUp(0) & !alreadyThrown)
            {
                lineRenderer.enabled = false;

                rigidbody.isKinematic = false;
                gameObject.transform.SetParent(null);
                //animator.SetTrigger("Throwing"); 수류탄 던지는 애니메이션 추가 시 사용

                Vector3 fireDirection = transform.forward + transform.up / 4; //수류탄이 날아갈 방향
                rigidbody.AddForce(fireDirection * throwingPower, ForceMode.Impulse);

                state = State.Fire;
                alreadyThrown = true;
            }
        }
    }

    //수류탄이 터지는 것을 구현한 메서드
    void Explosion()
    {
        bool damaged = false; // 수류탄이 여러 번 대미지를 넣는 것을 방지

        // 수류탄의 핀이 이미 뽑힌 상태일 때 터질 시간 확인
        if (state == State.Cooking || state == State.Fire)
        {
            cookingTime -= Time.deltaTime;

            // explosionTime이 7초 근처에 도달했을 때 파티클 실행
            if (cookingTime <= 2f && cookingTime > 1.99f)
            {
                explosionParticle.Play();
                if (!exploded)
                {
                    Collider[] colls = Physics.OverlapSphere(transform.position, 7f);

                    for (int i = 0; i < colls.Length; i++)
                    {
                        if (colls[i].TryGetComponent<IDamageable>(out var damageable))
                        {
                            Vector3 hitPoint = colls[i].ClosestPoint(transform.position); // 충돌 지점 추정
                            Debug.Log(hitPoint);


                            //임의의 수류탄 파편(50개)이 플레이어에게 맞는지 확인하여 대미지를 줌
                            for (int j = 0; j < 50; j++)
                            {
                                if (damaged)
                                    break;
                                hitPoint.x = Random.Range(hitPoint.x - 0.18f, hitPoint.x + 0.18f);
                                hitPoint.y = Random.Range(hitPoint.y - 0.9f, hitPoint.y + 0.9f);
                                hitPoint.z = Random.Range(hitPoint.z - 0.18f, hitPoint.z + 0.18f);

                                Vector3 direction = (hitPoint - transform.position).normalized; // 폭발 지점 → 대상 방향
                                float distance = Vector3.Distance(transform.position, hitPoint); // 폭발 지점과 대상 사이 거리

                                // 벽이 있는지 Raycast로 체크 (LayerMask 사용 가능)
                                if (!Physics.Raycast(transform.position, direction, distance, LayerMask.GetMask("Wall")))
                                {
                                    Debug.DrawRay(transform.position, direction * distance, Color.red, 100f);
                                    Vector3 hitNormal = direction;

                                    // 거리별로 대미지 조절
                                    switch (distance)
                                    {
                                        case <= 1f:
                                            damageable.OnDamage(damage, hitPoint, hitNormal);
                                            damaged = true;
                                            break;
                                        case <= 2f:
                                            damageable.OnDamage(damage * 0.8f, hitPoint, hitNormal);
                                            damaged = true;
                                            break;
                                        case <= 3f:
                                            damageable.OnDamage(damage * 0.6f, hitPoint, hitNormal);
                                            damaged = true;
                                            break;
                                        case <= 4f:
                                            damageable.OnDamage(damage * 0.4f, hitPoint, hitNormal);
                                            damaged = true;
                                            break;
                                        case <= 5f:
                                            damageable.OnDamage(damage * 0.2f, hitPoint, hitNormal);
                                            damaged = true;
                                            break;
                                    }
                                }

                            }

                        }
                    }
                    exploded = true; //한번 터지면 다시 터지지 않도록 설정
                }
            }
            // explosionTime이 9초 근처에 도달했을 때 오브젝트 파괴
            if (cookingTime <= 0f)
            {
                Destroy(gameObject); // 수류탄 오브젝트 제거
            }
        }
    }

    //수류탄 투척 궤적을 그리기 위한 메서드
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