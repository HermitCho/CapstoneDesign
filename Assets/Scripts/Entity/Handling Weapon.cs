using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlingWeapon : MonoBehaviour
{
    PlayerShooter playerShooter;
    PlayerInput playerInput;
    public bool showGun = true;

    void Awake()
    {
        playerShooter = GetComponent<PlayerShooter>();
        playerInput = GetComponent<PlayerInput>();
    }

    //1�� ��ư Ŭ�� ��, �տ� ���� ��Ÿ���� ��
    void Update()
    {
        if (playerInput.handleGunButton && !showGun)
        {
            UIManager.Instance.SelectGunORSkillUI(0); // �ΰ��� UI�� ����ź ������ ǥ��, �ѱ� Ű�� �������� 0 ����

            showGun = true;
            controlPlayerShooter(true);
        }
    }

    //playerShooter ������Ʈ ����
    public void controlPlayerShooter(bool onoff)
    {
        if (onoff)
        {
            playerShooter.enabled = true;
            playerShooter.OnOffGun(true);
        }
        else if (!onoff)
        {
            playerShooter.OnOffGun(false);
            playerShooter.enabled = false;
        }
    }
}
