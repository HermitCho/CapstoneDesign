using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using JetBrains.Annotations;



//Canvas에 연결하여 Slider 및 InputField 설정
public class OptionSetting : MonoBehaviour
{
    [Header("좌우 감도 설정 슬라이더")]
    public UnityEngine.UI.Slider xMouseSensitivitySlider;

    [Header("상하 감도 설정 슬라이더")]
    public UnityEngine.UI.Slider yMouseSensitivitySlider;

    [Header("현재 감도 InputField")]
    public TMP_InputField xMouseSensitivityField;
    public TMP_InputField yMouseSensitivityField;

    [Header("소리 크기 설정 슬라이더")]
    public UnityEngine.UI.Slider soundSlider;

    [Header("소리 크기 텍스트")]
    public TMP_Text soundTextTMP;

    private float xMouseSensitivity;//실제 변경할 좌우 감도 변수
    private float yMouseSensitivity; //실제 변경할 상하 감도 변수

    private float yParsedValue; //out 값 저장 변수
    private float xParsedValue; //out 값 저장 변수

    private float sound; //실제 변경할 소리 크기 변수


    private AudioSource playerAudioSource; //플레이어 오디오소스 컴포넌트
    private AudioSource gunAudioSource; //총기 오디오소스 컴포넌트
    private PlayerMovement playerMovement; //플레이어 움직임 컴포넌트

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        gunAudioSource = GameObject.FindGameObjectWithTag("Gun").GetComponent<AudioSource>();

        //슬라이더 상요작용 여부
        xMouseSensitivitySlider.interactable = true;
        yMouseSensitivitySlider.interactable = true;
        soundSlider.interactable = true;

        //슬라이더의 최솟값
        xMouseSensitivitySlider.minValue = 0f;
        yMouseSensitivitySlider.minValue = 0f;
        soundSlider.minValue = 0f;

        //감도 설정 슬라이더의 최댓값
        xMouseSensitivitySlider.maxValue = 20f;
        yMouseSensitivitySlider.maxValue = 20f;
        soundSlider.maxValue = 1f;

        // 슬라이더의 값 실수로 사용
        xMouseSensitivitySlider.wholeNumbers = false;
        yMouseSensitivitySlider.wholeNumbers = false;
        soundSlider.wholeNumbers = false;

        //슬라이더 값 초기 설정, 나중에 로컬저장으로 값 불러올 예정
        xMouseSensitivitySlider.value = 1f;
        yMouseSensitivitySlider.value = 1f;
        soundSlider.value = 0.2f;

        //감도 설정 inputField 값 초기 설정
        xMouseSensitivityField.text = xMouseSensitivitySlider.value.ToString();
        yMouseSensitivityField.text = yMouseSensitivitySlider.value.ToString();

        //소리 크기 텍스트(TMP) 값 초기 설정
        soundTextTMP.text = soundSlider.value.ToString();

        sound = soundSlider.value;
    }


    private void OnEnable()
    {
        //오브젝트 활성화시 초기화
        xMouseSensitivitySlider.value = playerMovement.xMouseSensitivity;
        yMouseSensitivitySlider.value = playerMovement.yMouseSensitivity;
        xParsedValue = playerMovement.xMouseSensitivity;
        yParsedValue = playerMovement.yMouseSensitivity;

        soundSlider.value = sound;
        soundTextTMP.text = sound.ToString();


    }

    private void Update()
    {
        SensitivityChangeToSlider();
        SensitivityChangeToInputField();
        SetSoundToSlider();

      //  SetValue();
    }


    //슬라이더로 감도 조절
    private void SensitivityChangeToSlider()
    {
        //소수점 두자리까지만 
        xMouseSensitivitySlider.value = float.Parse(xMouseSensitivitySlider.value.ToString("N2"));
        yMouseSensitivitySlider.value = float.Parse(yMouseSensitivitySlider.value.ToString("N2"));

/*        //감도 값을 슬라이더값으로 설정
        xMouseSensitivity = xMouseSensitivitySlider.value;
        yMouseSensitivity = yMouseSensitivitySlider.value; */
    }

    //사용자 입력값으로 감도 조절
    private void SensitivityChangeToInputField()
    {

        // 입력된 값이 float으로 변환 가능한지 확인
        if (float.TryParse(xMouseSensitivityField.text, out xParsedValue) && float.TryParse(yMouseSensitivityField.text, out yParsedValue))
        {
            // 감도 값 설정
            xMouseSensitivityField.text = xParsedValue.ToString();
            yMouseSensitivityField.text = yParsedValue.ToString();


        }
    }

    //소리 크기 설정, 슬라이더로 조절
    //텍스트로 현재 소리 크기 표시
    public void SetSoundToSlider()
    {
        //소수점 두자리까지만 
        soundSlider.value = float.Parse(soundSlider.value.ToString("N2"));
        soundSlider.value = float.Parse(soundSlider.value.ToString("N2"));

/*        //총소리 크기 변수에 슬라이더 값 저장
        sound = soundSlider.value;*/
    }

    //실제 설정값을 적용하기 위한 메서드
    public void SetValue() 
    {
        //감도값을 실제로 적용
        playerMovement.xMouseSensitivity = xMouseSensitivitySlider.value;
        playerMovement.yMouseSensitivity = yMouseSensitivitySlider.value;

        //소리크기값을 실제로 적용
        playerAudioSource.volume = soundSlider.value;
        gunAudioSource.volume = soundSlider.value;

        sound = soundSlider.value;
    }



    //InputField의 On Deselect(InputField를 나갈때)에서 사용
    //동기화
    public void SensitivitySyncInputField()
    {
        //InputField의 값을 Slider에 동기화
        xMouseSensitivitySlider.value = xParsedValue;
        yMouseSensitivitySlider.value = yParsedValue;
    }

    //Slider의 On Value Changed(값이 바뀔때)에서 사용
    //동기화
    public void SensitivitySyncSlider()
    {
        //Slider의 값을 InputField에 동기화
        xMouseSensitivityField.text = xMouseSensitivitySlider.value.ToString();
        yMouseSensitivityField.text = yMouseSensitivitySlider.value.ToString();
    }

    //Slider의 On Value Changed(값이 바뀔때)에서 사용
    //동기화
    public void SoundSyncSlider()
    {
        //Slider의 값을 TMP_text에 동기화
        soundTextTMP.text = soundSlider.value.ToString();
    }




}
