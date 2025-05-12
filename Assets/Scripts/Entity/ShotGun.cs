using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 해당 코드는 '레트로의 유니티 게임 프로그래밍 에센스 개정판'의 Gun.cs 스크립트를 기반으로 작성되었습니다.
/// </summary>
/// 
public class ShotGun : Gun
{
    // 발사 효과 코루틴 (총구 화염, 탄피, 궤적, 사운드)
    protected IEnumerator ShotEffect(Vector3 start, Vector3 end)
    {
        muzzleFlashEffect.Play();       // 총구 화염 효과
        shellEjectEffect.Play();        // 탄피 배출 효과
        gunAudioPlayer.PlayOneShot(gunData.shotClip); // 총 발사 소리

        // 새로운 LineRenderer 오브젝트 생성
        GameObject lineObj = new GameObject("PelletTrail");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.material = bulletLineRenderer.material;
        line.startWidth = bulletLineRenderer.startWidth;
        line.endWidth = bulletLineRenderer.endWidth;
        line.startColor = bulletLineRenderer.startColor;
        line.endColor = bulletLineRenderer.endColor;
        line.useWorldSpace = true;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        yield return new WaitForSeconds(0.03f);

        Destroy(lineObj);
    }

    // 실제 발사 로직
    protected override void Shot()
    {
        int pelletCount = 8; // 샷건 탄알 수
        float spreadAngle = 10f; // 퍼짐 각도

        for (int i = 0; i < pelletCount; i++)
        {
            RaycastHit hit;
            Vector3 shootDirection = fireTransform.forward;

            // 샷건 퍼짐 구현
            shootDirection = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f
            ) * shootDirection;

            Vector3 hitPosition = fireTransform.position + shootDirection * fireDistance;

            if (Physics.Raycast(fireTransform.position, shootDirection, out hit, fireDistance))
            {
                hitPosition = hit.point;

                if (hit.collider.TryGetComponent<IDamageable>(out var target))
                {
                    target.OnDamage(gunData.damage, hit.point, hit.normal);
                }
            }
            StartCoroutine(ShotEffect(fireTransform.position, hitPosition));
        }

        magAmmo--;
        if (magAmmo <= 0)
        {
            state = State.Empty;
        }
    }
}
