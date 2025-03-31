using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

public class ViewSliderUI : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth; // 플레이어의 체력을 가져올 대상
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더
    public Slider shieldSlider; // 추가 방어막을 표시할 UI 슬라이더
    public Slider energySlider; // 기력을 표시할 UI 슬라이더

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>(); // 부모 오브젝트의 PlayerHealth 컴포넌트를 가져옴
    }

    private void OnEnable()
    {
        healthSlider.gameObject.SetActive(true);
    }


}
