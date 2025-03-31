using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public enum State
    {
        Empty,
        Ready,
        Cooking,
        Fire
    }

    public State state { get; protected set; } // 연막탄 파지 상태
    private PlayerInput playerInput; // 연막탄을 든 해당 캐릭터의 키인풋을 받아옴
    private PlayerMovement playerMovement; // 연막탄을 든 해당 캐릭터의 이동 컴포넌트
    private Rigidbody rigidbody; // 연막탄의 리지드바디
    private Collider collider; // 연막탄의 콜라이더
    [SerializeField] ParticleSystem explosionParticle; // 연막탄 폭발 파티클

    // Start is called before the first frame update

    float cookingTime = 16f; // 최대 연막탄 쿠킹 시간 + 삭제까지 대기시간
    float throwingPower = 20f; // 연막탄 투척 속도
    float throwingDelay = 2f; // 연막탄을 던질 때 너무 빨리 던지면 isKinematic이 off되기 전에 바닥을 뚫고 지나감, 그래서 약간의 딜레이를 넣음
    bool throwingDelayBool = false;
    private bool alreadyThrown; // 던져진 상태인지 확인

    LineRenderer lineRenderer; //연막탄 투척 궤적을 그리기 위한 라인렌더러
    Transform throwingposition; //연막탄 투척 위치

    void Start()
    {
        state = State.Ready;

        playerInput = GetComponentInParent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        alreadyThrown = false;

        lineRenderer = GetComponentInParent<LineRenderer>();
        throwingposition = transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Explosion();
        Debug.Log(state);
    }

    // 2번 키를 누르면 연막탄을 손에 들게 됨
    public void Handling()
    {
        if (state == State.Empty && playerInput.skill_2_Button && !alreadyThrown)
        {
            state = State.Ready;
            Debug.Log(state);
            gameObject.SetActive(true);
        }
        else if (state == State.Ready && (playerInput.skill_1_Button || playerInput.handleGunButton) && !alreadyThrown)
        {
            state = State.Empty;
            Debug.Log(state);
            gameObject.SetActive(false);
        }
    }

    // 연막탄 쿠킹 및 투척을 위한 메서드
    public void Throwing()
    {
        Debug.Log(cookingTime);
        if (state == State.Ready || state == State.Cooking)
        {
            if (Input.GetMouseButton(0) && !alreadyThrown)
            {
                state = State.Cooking;

                Vector3 grenadeVelocity = (throwingposition.forward).normalized * throwingPower;
                ShowTrajectLine(throwingposition.position + throwingposition.forward + throwingposition.up / 4, grenadeVelocity);
            }
            if (Input.GetMouseButtonUp(0))
            {
                rigidbody.isKinematic = false;
                gameObject.transform.SetParent(null);

                Vector3 fireDirection = transform.forward + transform.up / 4; //연막탄이 날아갈 방향
                rigidbody.AddForce(fireDirection * throwingPower, ForceMode.Impulse);
                state = State.Fire;
                alreadyThrown = true;
            }
        }
    }

    //연막탄이 터지는 것을 구현한 메서드
    void Explosion()
    {
        if (state == State.Cooking || state == State.Fire)
        {
            cookingTime -= Time.deltaTime;

            // explosionTime이 7초 근처에 도달했을 때 파티클 실행
            if (cookingTime <= 9f && cookingTime > 8.9f)
            {
                explosionParticle.Play();
            }
            // explosionTime이 9초 근처에 도달했을 때 오브젝트 파괴
            if (cookingTime <= 0f)
            {
                Destroy(gameObject); // 이 오브젝트를 파괴
            }
        }
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