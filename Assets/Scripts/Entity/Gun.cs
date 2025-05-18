using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 해당 코드는 '레트로의 유니티 게임 프로그래밍 에센스 개정판'의 Gun.cs 스크립트를 기반으로 작성되었습니다.
/// </summary>
/// 
public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,      // 발사 준비 완료
        Empty,      // 탄창이 빔
        Reloading   // 재장전 중
    }

    public State state { get; set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치
    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect;  // 탄피 배출 효과

    protected LineRenderer bulletLineRenderer; // 총알 궤적을 그릴 라인 렌더러

    protected AudioSource gunAudioPlayer;      // 총 소리를 재생할 오디오 소스
    public GunData gunData;                  // 총의 데이터
    public Animator gunAnimator; // 총의 애니메이터

    [HideInInspector] public int magAmmo;    // 현재 탄창에 남아있는 총알 수

    protected float lastFireTime;              // 마지막으로 총을 발사한 시각

    protected float fireDistance;              // 총알 사거리

    // 컴포넌트 초기화
    protected void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        gunAnimator = GetComponent<Animator>(); // Animator 초기화

        bulletLineRenderer.positionCount = 2; // 궤적은 시작점과 끝점 두 개
        bulletLineRenderer.enabled = false;   // 초기에는 비활성화
    }

    // 총 활성화 시 초기화
    protected void OnEnable()
    {
        magAmmo = gunData.magCapacity;       // 탄창을 가득 채움
        fireDistance = gunData.fireDistance; // 사거리 설정
        state = State.Ready;                 // 상태를 준비 완료로 설정
        lastFireTime = 0;                    // 마지막 발사 시각 초기화
    }

    protected void OnDisable()
    {
        state = State.Empty; // 비활성화 시 상태를 Empty로
    }

    // 발사 효과 코루틴 (총구 화염, 탄피, 궤적, 사운드)
    protected virtual IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();       // 총구 화염 효과
        shellEjectEffect.Play();        // 탄피 배출 효과
        gunAudioPlayer.PlayOneShot(gunData.shotClip); // 총 발사 소리

        bulletLineRenderer.SetPosition(0, fireTransform.position); // 시작점
        bulletLineRenderer.SetPosition(1, hitPosition);            // 끝점
        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f); // 궤적 유지 시간

        bulletLineRenderer.enabled = false; // 궤적 비활성화
    }

    public void Fire()
    {
        if (state == State.Ready && (Time.time >= lastFireTime + gunData.timeBetFire))
        {
            lastFireTime = Time.time; // 발사 시간 갱신
            Shot();                   // 실제 발사 처리
        }
    }

    // 실제 발사 로직
    protected virtual void Shot()
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null)
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal); // 대상에 데미지 부여
            }

            hitPosition = hit.point;
        }
        else
        {
            // 명중하지 않으면 최대 사거리로 궤적 처리
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));

        magAmmo--; // 탄약 감소
        if (magAmmo <= 0)
        {
            state = State.Empty; // 탄약이 없으면 상태 변경
        }
    }

    public bool Reload()
    {
        if (state == State.Reloading || magAmmo >= gunData.magCapacity)
        {
            return false; // 이미 재장전 중이거나 탄창이 가득 찼으면 리턴
        }

        StartCoroutine(ReloadRoutine());
        Debug.Log(gunData.reloadTime); // 리로드 시간 디버깅
        return true;
    }

    // 재장전 처리 루틴
    protected IEnumerator ReloadRoutine()
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(gunData.reloadClip); // 재장전 소리

        // if (gunAnimator != null)
        // {
        //     gunAnimator.SetTrigger("Reload"); // 장전 애니메이션 트리거
        // }

        yield return new WaitForSeconds(gunData.reloadTime); // 리로드 대기

        int ammoToFill = gunData.magCapacity - magAmmo;
        magAmmo += ammoToFill; // 탄창 채움

        state = State.Ready; // 상태를 준비 완료로 변경
    }
}
