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
        if (playerInput.handleGunButton && !showGun)
        {
            UIManager.Instance.SelectGunORSkillUI(0); // 인게임 UI에 수류탄 아이콘 표시, 총기 키를 눌렀으니 0 전송

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
}
