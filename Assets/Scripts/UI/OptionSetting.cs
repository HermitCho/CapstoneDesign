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

    [Header("���� ���� �ؽ�Ʈ")]
    public TMP_InputField xMouseSensitivityField;
    public TMP_InputField yMouseSensitivityField;

    private float xMouseSensitivity;//���� ������ �¿� ���� 
    private float yMouseSensitivity; //���� ������ ���� ����

    private float yParsedValue;
    private float xParsedValue;


    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        //�����̴� ����ۿ� ����
        xMouseSensitivitySlider.interactable = true;
        yMouseSensitivitySlider.interactable = true;

        // �����̴��� �ּڰ�
        xMouseSensitivitySlider.minValue = 0f;
        yMouseSensitivitySlider.minValue = 0f;

        // �����̴��� �ִ�
        xMouseSensitivitySlider.maxValue = 20f;
        yMouseSensitivitySlider.maxValue = 20f;


        // �����̴��� �� �Ǽ��� ���
        xMouseSensitivitySlider.wholeNumbers = false;
        yMouseSensitivitySlider.wholeNumbers = false;

        xMouseSensitivitySlider.value = 1f;//�¿� ���� ���� �����̴������� ����
        yMouseSensitivitySlider.value = 1f; //���� ���� ���� �����̴������� ���� 

        xMouseSensitivityField.text = "1";
        yMouseSensitivityField.text = "1";
    }

    

    private void Update()
    {
        ChangeToSlider();
        ChangeToInputField();

        SetSensitivityValue();
    }



    //�����̴��� ���� ����
    private void ChangeToSlider()
    {
        //�Ҽ��� ���ڸ������� 
        xMouseSensitivitySlider.value = float.Parse(xMouseSensitivitySlider.value.ToString("N2"));
        yMouseSensitivitySlider.value = float.Parse(yMouseSensitivitySlider.value.ToString("N2"));

        xMouseSensitivity = xMouseSensitivitySlider.value;//�¿� ���� ���� �����̴������� ����
        yMouseSensitivity = yMouseSensitivitySlider.value; //���� ���� ���� �����̴������� ���� 




    }

    //����� �Է°����� ���� ����
    private void ChangeToInputField()
    {

        // �Էµ� ���� float���� ��ȯ �������� Ȯ��
        if (float.TryParse(xMouseSensitivityField.text, out xParsedValue) && float.TryParse(yMouseSensitivityField.text, out yParsedValue))
        {

          


            // ���� �� ����
            xMouseSensitivity = xParsedValue;
            yMouseSensitivity = yParsedValue;

        }
    }


    //InputField�� On Deselect(InputField�� ������)���� ���
    //����ȭ
    public void SyncInputField()
    {
        //����ȭ
        xMouseSensitivitySlider.value = xParsedValue;
        yMouseSensitivitySlider.value = yParsedValue;
    }

    //Slider�� On Value Changed(���� �ٲ�)���� ���
    //����ȭ
    public void SyncSlider()
    {

        xMouseSensitivityField.text = xMouseSensitivitySlider.value.ToString();
        yMouseSensitivityField.text = yMouseSensitivitySlider.value.ToString();
    }


    //���� ������ �����ϱ� ���� �޼���
    public void SetSensitivityValue() 
    {

        playerMovement.xMouseSensitivity = xMouseSensitivity;
        playerMovement.yMouseSensitivity = yMouseSensitivity;


    }


}
