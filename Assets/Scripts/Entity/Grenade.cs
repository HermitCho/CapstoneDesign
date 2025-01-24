using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public enum State
    {
        Empty,
        Ready,
        Cooking,
        Fire
    }

    public State state { get; protected set; } // 수류탄 파지 상태
    private PlayerInput playerInput; // 수류탄을 든 해당 캐릭터의 키인풋을 받아옴
    private PlayerMovement playerMovement; // 수류탄을 든 해당 캐릭터의 이동 컴포넌트
    private Rigidbody rigidbody; // 수류탄의 리지드바디
    private Collider collider; // 수류탄의 콜라이더
    [SerializeField] ParticleSystem explosionParticle; // 수류탄 폭발 파티클
    private bool alreadyThrown; // 던져진 상태인지 확인

    // Start is called before the first frame update

    int damage = 90; //수류탄 데미지
    float cookingTime = 9f; // 최대 수류탄 쿠킹 시간 + 삭제까지 대기시간
    float throwingPower = 20f; // 수류탄 투척 속도
    float throwingDelay = 2f; // 수류탄을 던질 때 너무 빨리 던지면 isKinematic이 off되기 전에 바닥을 뚫고 지나감, 그래서 약간의 딜레이를 넣음
    bool throwingDelayBool = false;

    void Start()
    {
        state = State.Ready;

        playerInput = GetComponentInParent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        alreadyThrown = false;
    }

    // Update is called once per frame
    void Update()
    {
        Explosion();
    }

    // 2번 키를 누르면 수류탄을 손에 들게 됨
    public void Handling()
    {
        if (state == State.Empty && playerInput.skill_1_Button && !alreadyThrown)
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

    // 수류탄 쿠킹 및 투척을 위한 메서드
    public void Throwing()
    {
        Debug.Log(cookingTime);
        if (state == State.Ready || state == State.Cooking)
        {
            if (Input.GetMouseButton(0) && !alreadyThrown)
            {
                state = State.Cooking;
            }
            if (Input.GetMouseButtonUp(0) & !alreadyThrown)
            {
                rigidbody.isKinematic = false;
                gameObject.transform.SetParent(null);

                Vector3 fireDirection = playerMovement.LocalPosToWorldDirection();
                rigidbody.AddForce(fireDirection * throwingPower, ForceMode.Impulse);
                state = State.Fire;
                alreadyThrown = true;
            }
        }
    }

    //수류탄이 터지는 것을 구현한 메서드
    void Explosion()
    {
        if (state == State.Cooking || state == State.Fire)
        {
            cookingTime -= Time.deltaTime;

            // explosionTime이 7초 근처에 도달했을 때 파티클 실행
            if (cookingTime <= 2f && cookingTime > 1.9f)
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
}




/*

 수류탄 들어 -> 핀 뽑고 -> 던지고 -> 터지고
 포물선을 그리면서 나가야되니까


*/