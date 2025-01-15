using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready, //발사 준비 완료
        Empty, //탄창이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총 상태 불러오기

    public Transform fireTransform; //총알이 발사될 위치
    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; //탄피 배출 효과

    private LineRenderer bulletLineRenderer; //탄알 궤적 렌더러
    private PlayerMovement PlayerMovement; //총알 발사 위치 찾기 위한 컴포넌티

    private AudioSource gunAudioPlayer; //총소리 재생
    public GunData gunData;// 총 데이터

    [HideInInspector] public int magAmmo; //현재 탄창에 남아있는 탄알 수

    private float lastFireTime; //총을 마지막에 발사한 시점

    private float fireDistance; //사정거리

    //사용 컴포넌트 가져오기
    private void Awake()
    {
        //사용 컴포넌트 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        PlayerMovement = GetComponentInParent<PlayerMovement>(); //Gun 오브젝트의 부모인 Player에서 컴포넌트 찾기

        //사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        //라인 렌터러 비활성화
        bulletLineRenderer.enabled = false;
         
    }

    //사용 변수 초기화
    private void OnEnable()
    {
        //현재 탄창 가득 채우기
        magAmmo = gunData.magCapacity;
        //사정거리 설정
        fireDistance = gunData.fireDistance;

        //총 상태를 준비상태로 변경
        state = State.Ready;
        //마지막 총을 쏜 시점 초기화
        lastFireTime = 0;


    }


    //효과 및 소리 재생 코루틴
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        //총구 화염 효과 재생
        muzzleFlashEffect.Play();
        //탄피 배출 효과 재생
        shellEjectEffect.Play();

        //총격 소리 재생
        gunAudioPlayer.PlayOneShot(gunData.shotClip);
        //선의 시작점: 총구의 위치
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        //선의 끝점: 입력으로 들어온 충돌위치
        bulletLineRenderer.SetPosition(1, hitPosition);
        //라인렌더러 활성화
        bulletLineRenderer.enabled = true;

        //0.03초동안 처리 대기
        yield return new WaitForSeconds(0.03f);

        //라인렌터러 비활성화
        bulletLineRenderer.enabled = false ;
    }

    public void Fire()
    {
        //현재 발사 가능 상태 && 마지막 발사 시점에서 gunData.timeBetFire 이상의 시간이 지남
        if(state == State.Ready && (Time.time >= lastFireTime + gunData.timeBetFire))
        {
            //마지막 총 발사 시점 업데이트
            lastFireTime = Time.time;
            //실제 발사 처리
            Shot();
        }
       
    }

    //실제 발사 처리 메서드
    private void Shot()
    {
        //레이캐스트 충돌 정보 저장 컨테이너
        RaycastHit hit;
        //탄알이 맞은 곳 저장 변수
        Vector3 hitPosition = Vector3.zero;

        Vector3 fireDirection = PlayerMovement.LocalPosToWorldDirection();

        //레이캐스트(시작지점, 방향, 충돌정보 컨테이너, 사정거리)
        if (Physics.Raycast(fireTransform.position, fireDirection, out hit, fireDistance))
        {
            //레이가 물체와 충돌한 경우
            //충돌한 물체에 대한 IDamageable 오브젝트 가져오기 시도
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            //상대방으로부터 IDamageable 오브젝트를 가져오는데 성공했다면
            if(target!= null)
            {
                //상대방의 OnDamage 함수 실행 시키기(데미지 주기)
                target.OnDamage(gunData.damage,hit.point,hit.normal);
            }

            //레이가 충돌한 위치 저장
            hitPosition = hit.point;
        }
        else
        {
            //레이가 물체와 충돌하지 않았다면
            //탄알이 최대 사정거리까지 날아갔을때의 위치를 충돌위치로 사용
            hitPosition = fireTransform.position + fireDirection * fireDistance; //fireTransform.forward 부분 수정 필요 
        }

        //발사 이펙트 시작
        StartCoroutine(ShotEffect(hitPosition));

        //남은 탄알 수 -1
        magAmmo--;
        if(magAmmo <= 0)
        {
            //탄창에 남은 탄알이 없다면 총 상태 Empty로 갱신
            state = State.Empty;
        }
    }


    public bool Reload()
    {
        if(state == State.Reloading || magAmmo >= gunData.magCapacity)
        {
            //재장전 중이거나 탄창에 총알이 가득 찬 경우 X
            return false;
        }

        //실질적인 재장전 처리
        StartCoroutine(ReloadRoutine());
        return true;
    }

    //실제 재장전 처리
    private IEnumerator ReloadRoutine()
    {
        //현재 총의 상태를 재장전 중으로 변경
        state = State.Reloading;
        //재장전 소리 재생
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);

        //재장전 시간만큼 쉬기
        yield return new WaitForSeconds(gunData.reloadTime);

        //탄창에 채울 탄알 개수 계산 
        int ammoToFill = gunData.magCapacity - magAmmo;

        //탄창 채우기
        magAmmo += ammoToFill;
        //총의 상태를 발사 준비로 변경
        state = State.Ready;
    }
}
