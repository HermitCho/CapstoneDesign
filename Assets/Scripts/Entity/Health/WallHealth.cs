using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallHealth : LivingEntity
{
    [SerializeField] Slider healthSlider;
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();
        startingHealth = 200;
        health = startingHealth;
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
    }

    // Update is called once per frame

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            //playerAudioPlayer.PlayOneShot();
        }
        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection);
        healthSlider.value = health;
    }

    // 사망 처리
    public override bool Die()
    {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();
        healthSlider.gameObject.SetActive(false);
        //playerAudioPlayer.PlayOneShot();

        Destroy(gameObject);
        return true;
    }

    private Vector3 lastScreenPosition;
    
    private void LateUpdate()
    {
        Vector3 targetPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2.4f, 0));
        healthSlider.transform.position = targetPosition;

        // 카메라가 급격히 회전하면 즉시 이동, 아니면 Lerp 적용
        if (Vector3.Distance(lastScreenPosition, targetPosition) > Screen.width * 0.07f)
        {
            healthSlider.transform.position = targetPosition; // 즉시 반응
        }
        else
        {
            healthSlider.transform.position = Vector3.Lerp(healthSlider.transform.position, targetPosition, Time.deltaTime * 20f);
        }

        lastScreenPosition = targetPosition; // 현재 위치 저장
    }
}
