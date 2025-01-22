using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using JetBrains.Annotations;



//Canvas�� �����Ͽ� Slider �� InputField ����
public class OptionSetting : MonoBehaviour
{
    [Header("�¿� ���� ���� �����̴�")]
    public UnityEngine.UI.Slider xMouseSensitivitySlider;

    [Header("���� ���� ���� �����̴�")]
    public UnityEngine.UI.Slider yMouseSensitivitySlider;

    [Header("���� ���� InputField")]
    public TMP_InputField xMouseSensitivityField;
    public TMP_InputField yMouseSensitivityField;

    [Header("�Ҹ� ũ�� ���� �����̴�")]
    public UnityEngine.UI.Slider soundSlider;

    [Header("�Ҹ� ũ�� �ؽ�Ʈ")]
    public TMP_Text soundTextTMP;

    private float xMouseSensitivity;//���� ������ �¿� ���� ����
    private float yMouseSensitivity; //���� ������ ���� ���� ����

    private float yParsedValue; //out �� ���� ����
    private float xParsedValue; //out �� ���� ����

    private float sound; //���� ������ �Ҹ� ũ�� ����


    private AudioSource playerAudioSource; //�÷��̾� ������ҽ� ������Ʈ
    private AudioSource gunAudioSource; //�ѱ� ������ҽ� ������Ʈ
    private PlayerMovement playerMovement; //�÷��̾� ������ ������Ʈ

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        gunAudioSource = GameObject.FindGameObjectWithTag("Gun").GetComponent<AudioSource>();

        //�����̴� ����ۿ� ����
        xMouseSensitivitySlider.interactable = true;
        yMouseSensitivitySlider.interactable = true;
        soundSlider.interactable = true;

        //�����̴��� �ּڰ�
        xMouseSensitivitySlider.minValue = 0f;
        yMouseSensitivitySlider.minValue = 0f;
        soundSlider.minValue = 0f;

        //���� ���� �����̴��� �ִ�
        xMouseSensitivitySlider.maxValue = 20f;
        yMouseSensitivitySlider.maxValue = 20f;
        soundSlider.maxValue = 1f;

        // �����̴��� �� �Ǽ��� ���
        xMouseSensitivitySlider.wholeNumbers = false;
        yMouseSensitivitySlider.wholeNumbers = false;
        soundSlider.wholeNumbers = false;

        //�����̴� �� �ʱ� ����, ���߿� ������������ �� �ҷ��� ����
        xMouseSensitivitySlider.value = 1f;
        yMouseSensitivitySlider.value = 1f;
        soundSlider.value = 0.2f;

        //���� ���� inputField �� �ʱ� ����
        xMouseSensitivityField.text = xMouseSensitivitySlider.value.ToString();
        yMouseSensitivityField.text = yMouseSensitivitySlider.value.ToString();

        //�Ҹ� ũ�� �ؽ�Ʈ(TMP) �� �ʱ� ����
        soundTextTMP.text = soundSlider.value.ToString();

        sound = soundSlider.value;
    }


    private void OnEnable()
    {
        //������Ʈ Ȱ��ȭ�� �ʱ�ȭ
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


    //�����̴��� ���� ����
    private void SensitivityChangeToSlider()
    {
        //�Ҽ��� ���ڸ������� 
        xMouseSensitivitySlider.value = float.Parse(xMouseSensitivitySlider.value.ToString("N2"));
        yMouseSensitivitySlider.value = float.Parse(yMouseSensitivitySlider.value.ToString("N2"));

/*        //���� ���� �����̴������� ����
        xMouseSensitivity = xMouseSensitivitySlider.value;
        yMouseSensitivity = yMouseSensitivitySlider.value; */
    }

    //����� �Է°����� ���� ����
    private void SensitivityChangeToInputField()
    {

        // �Էµ� ���� float���� ��ȯ �������� Ȯ��
        if (float.TryParse(xMouseSensitivityField.text, out xParsedValue) && float.TryParse(yMouseSensitivityField.text, out yParsedValue))
        {
            // ���� �� ����
            xMouseSensitivityField.text = xParsedValue.ToString();
            yMouseSensitivityField.text = yParsedValue.ToString();


        }
    }

    //�Ҹ� ũ�� ����, �����̴��� ����
    //�ؽ�Ʈ�� ���� �Ҹ� ũ�� ǥ��
    public void SetSoundToSlider()
    {
        //�Ҽ��� ���ڸ������� 
        soundSlider.value = float.Parse(soundSlider.value.ToString("N2"));
        soundSlider.value = float.Parse(soundSlider.value.ToString("N2"));

/*        //�ѼҸ� ũ�� ������ �����̴� �� ����
        sound = soundSlider.value;*/
    }

    //���� �������� �����ϱ� ���� �޼���
    public void SetValue() 
    {
        //�������� ������ ����
        playerMovement.xMouseSensitivity = xMouseSensitivitySlider.value;
        playerMovement.yMouseSensitivity = yMouseSensitivitySlider.value;

        //�Ҹ�ũ�Ⱚ�� ������ ����
        playerAudioSource.volume = soundSlider.value;
        gunAudioSource.volume = soundSlider.value;

        sound = soundSlider.value;
    }



    //InputField�� On Deselect(InputField�� ������)���� ���
    //����ȭ
    public void SensitivitySyncInputField()
    {
        //InputField�� ���� Slider�� ����ȭ
        xMouseSensitivitySlider.value = xParsedValue;
        yMouseSensitivitySlider.value = yParsedValue;
    }

    //Slider�� On Value Changed(���� �ٲ�)���� ���
    //����ȭ
    public void SensitivitySyncSlider()
    {
        //Slider�� ���� InputField�� ����ȭ
        xMouseSensitivityField.text = xMouseSensitivitySlider.value.ToString();
        yMouseSensitivityField.text = yMouseSensitivitySlider.value.ToString();
    }

    //Slider�� On Value Changed(���� �ٲ�)���� ���
    //����ȭ
    public void SoundSyncSlider()
    {
        //Slider�� ���� TMP_text�� ����ȭ
        soundTextTMP.text = soundSlider.value.ToString();
    }




}
