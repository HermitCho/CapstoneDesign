using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSway : MonoBehaviour
{
    public Transform gunShakePivot;
    public Animator animator;
    public bool isRunning;
    private Vector3 initialLocalPosition; // 초기 로컬 위치 저장

    void Start()
    {   
        // 시작 시 총기의 초기 로컬 위치 저장
        initialLocalPosition = gunShakePivot.localPosition;
    }
    void Update()
    {

        // Base Layer의 isRunning 파라미터 값을 가져옴
        bool isRunning = animator.GetBool("isRunning");

        // UpperBody 레이어에서 현재 애니메이션 상태가 Reload인지 확인
        AnimatorStateInfo upperBodyStateInfo = animator.GetCurrentAnimatorStateInfo(1);
        bool isReloading = upperBodyStateInfo.IsName("Reload");

        float shakeAmount = 0.2f; // 기본 흔들림 크기
        float speed = 20f;        // 기본 흔들림 속도

        if (isRunning)
        {
            shakeAmount = 0.05f;
            speed = 15f;
        }
        else if (isReloading)
        {
            shakeAmount = 0.05f;
            speed = 15f;
        }

        Vector3 shake = Vector3.zero; // 최종 흔들림 벡터 초기화

        if (isRunning)
        {
            // 달리는 중일 때 흔들림 적용
            shake = new Vector3(
                Mathf.Sin(Time.time * speed) * shakeAmount,        // x축
                Mathf.Cos(Time.time * speed * 0.5f) * shakeAmount, // y축
                Mathf.Sin(Time.time * speed * 0.3f) * shakeAmount  // z축
            );

            // 총기의 위치를 흔들림 위치로 부드럽게 이동
            gunShakePivot.localPosition = Vector3.Lerp(
                gunShakePivot.localPosition,
                initialLocalPosition + shake,
                Time.deltaTime * 5f
            );
        }
        else if (isReloading)
        {
            // 장전 중일 때 흔들림 적용
            shake = new Vector3(
                0f,                                                // x축
                Mathf.Cos(Time.time * speed * 0.7f) * shakeAmount, // y축
                Mathf.Sin(Time.time * speed * 0.2f) * shakeAmount  // z축
            );

            // 장전 중일 때 총기를 약간 뒤쪽으로 위치 이동
            Vector3 targetPosition = initialLocalPosition + new Vector3(0f, 0f, -0.2f);
            // 총기의 위치를 흔들림 위치로 부드럽게 이동
            gunShakePivot.localPosition = Vector3.Lerp(
                gunShakePivot.localPosition,
                targetPosition + shake,
                Time.deltaTime * 5f
            );
        }
        else
        {
            // 달리거나 장전 상태가 아니면 원래 위치로 부드럽게 이동
            gunShakePivot.localPosition = Vector3.Lerp(
                gunShakePivot.localPosition,
                initialLocalPosition,
                Time.deltaTime * 5f
            );
        }
    }
}