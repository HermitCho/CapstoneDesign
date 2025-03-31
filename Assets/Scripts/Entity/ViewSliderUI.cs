using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� �ڵ�

public class ViewSliderUI : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth; // �÷��̾��� ü���� ������ ���
    public Slider healthSlider; // ü���� ǥ���� UI �����̴�
    public Slider shieldSlider; // �߰� ���� ǥ���� UI �����̴�
    public Slider energySlider; // ����� ǥ���� UI �����̴�

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>(); // �θ� ������Ʈ�� PlayerHealth ������Ʈ�� ������
    }

    private void OnEnable()
    {
        healthSlider.gameObject.SetActive(true);
    }


}
