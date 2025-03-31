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

    //1번 버튼 클릭 시, 손에 총을 나타나게 함
    void Update()
    {
        if(playerInput.handleGunButton && !showGun)
        {
            Debug.Log("Handling");
            showGun = true;
            controlPlayerShooter(true);
        }
    }

    //playerShooter 컴포넌트 제어
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
