using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;



//Canvas에 연결하여 Slider 및 InputField 설정
public class OptionSetting : MonoBehaviour
{
    [Header("좌우 감도 설정 슬라이더")]
    public UnityEngine.UI.Slider xMouseSensitivitySlider;

    [Header("상하 감도 설정 슬라이더")]
    public UnityEngine.UI.Slider yMouseSensitivitySlider;

    [Header("현재 감도 텍스트")]
    public TMP_InputField xMouseSensitivityField;
    public TMP_InputField yMouseSensitivityField;

    private float xMouseSensitivity;//실제 변경할 좌우 감도 
    private float yMouseSensitivity; //실제 변경할 상하 감도

    private float yParsedValue;
    private float xParsedValue;

    private void Awake()
    {
        //슬라이더 상요작용 여부
        xMouseSensitivitySlider.interactable = true;
        yMouseSensitivitySlider.interactable = true;

        // 슬라이더의 최솟값
        xMouseSensitivitySlider.minValue = 0f;
        yMouseSensitivitySlider.minValue = 0f;

        // 슬라이더의 최댓값
        xMouseSensitivitySlider.maxValue = 10f;
        yMouseSensitivitySlider.maxValue = 10f;


        // 슬라이더의 값 실수로 사용
        xMouseSensitivitySlider.wholeNumbers = false;
        yMouseSensitivitySlider.wholeNumbers = false;




    }



    private void Update()
    {
        ChangeToSlider();
        ChangeToInputField();
    }



    //슬라이더로 감도 조절
    private void ChangeToSlider()
    {
        //소수점 두자리까지만 
        xMouseSensitivitySlider.value = float.Parse(xMouseSensitivitySlider.value.ToString("N2"));
        yMouseSensitivitySlider.value = float.Parse(yMouseSensitivitySlider.value.ToString("N2"));

        xMouseSensitivity = xMouseSensitivitySlider.value;//좌우 감도 값을 슬라이더값으로 설정
        yMouseSensitivity = yMouseSensitivitySlider.value; //상하 감도 값을 슬라이더값으로 설정 




    }

    //사용자 입력값으로 감도 조절
    private void ChangeToInputField()
    {

        // 입력된 값이 float으로 변환 가능한지 확인
        if (float.TryParse(xMouseSensitivityField.text, out xParsedValue) && float.TryParse(yMouseSensitivityField.text, out yParsedValue))
        {

          


            // 감도 값 설정
            xMouseSensitivity = xParsedValue;
            yMouseSensitivity = yParsedValue;

        }
    }


    //InputField의 On Deselect(InputField를 나갈때)에서 사용
    //동기화
    public void SyncInputField()
    {
        //동기화
        xMouseSensitivitySlider.value = xParsedValue;
        yMouseSensitivitySlider.value = yParsedValue;
    }

    //Slider의 On Value Changed(값이 바뀔때)에서 사용
    //동기화
    public void SyncSlider()
    {

        xMouseSensitivityField.text = xMouseSensitivitySlider.value.ToString();
        yMouseSensitivityField.text = yMouseSensitivitySlider.value.ToString();
    }


    //실제 감도를 설정하기 위한 값 반환 메서드
    public float GetSensitivityValue(int i)
    {
        if(i == 0)
        {
            return xMouseSensitivity;
        }
        else
        {
            return yMouseSensitivity;
        }
    }

}
