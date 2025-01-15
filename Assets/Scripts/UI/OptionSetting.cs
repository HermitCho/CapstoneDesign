using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;



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

    private void Awake()
    {
        //�����̴� ����ۿ� ����
        xMouseSensitivitySlider.interactable = true;
        yMouseSensitivitySlider.interactable = true;

        // �����̴��� �ּڰ�
        xMouseSensitivitySlider.minValue = 0f;
        yMouseSensitivitySlider.minValue = 0f;

        // �����̴��� �ִ�
        xMouseSensitivitySlider.maxValue = 10f;
        yMouseSensitivitySlider.maxValue = 10f;


        // �����̴��� �� �Ǽ��� ���
        xMouseSensitivitySlider.wholeNumbers = false;
        yMouseSensitivitySlider.wholeNumbers = false;




    }



    private void Update()
    {
        ChangeToSlider();
        ChangeToInputField();
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


    //���� ������ �����ϱ� ���� �� ��ȯ �޼���
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
