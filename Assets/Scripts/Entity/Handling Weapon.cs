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
        if(playerInput.handleGunButton && !showGun)
        {
            Debug.Log("Handling");
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

    //public void HandlingGrenade(bool onoff)
    //{
    //    if (onoff)
    //    {
    //        playerShooter.enabled = true;
    //    }
    //    else if (!onoff)
    //    {
    //        playerShooter.enabled = false;
    //    }
    //}
    //public void HandlingSmoke(bool onoff)
    //{
    //    if (onoff)
    //    {
    //        playerShooter.enabled = true;
    //    }
    //    else if (!onoff)
    //    {
    //        playerShooter.enabled = false;
    //    }
    //}
}
