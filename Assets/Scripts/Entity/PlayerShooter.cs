using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Gun ������Ʈ�� ��ų� ������ 
//IK ���
public class PlayerShooter : MonoBehaviour
{
    public Gun gun;
    public Transform gunPivot;
    public Transform LeftHandMount;
    public Transform RightHandMount;

    private PlayerInput playerInput;
    private Animator playerAnimator;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        gun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }

    public void OnOffGun(bool onoff)
    {
        if (onoff)
        {
            gun.gameObject.SetActive(true);
            gun.state = Gun.State.Ready;
        }
        else if (!onoff)
        {
            gun.state = Gun.State.Empty;
            gun.gameObject.SetActive(false);
        }
    }

    private void Update()
    {

        if (playerInput.fireButton)
        {
            gun.Fire();
        }
        else if (playerInput.reloadButton)
        {
            if (gun.Reload())
            {
                //������ �����ÿ��� �ִϸ��̼� ���
                playerAnimator.SetTrigger("Reload");
            }
        }

        //UI����
        //UpdateUI();
    }

    private bool IsPlayingAnimation(string animationName, int layerIndex = 0)
    {
    return playerAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationName);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (gun == null)
            return;

        bool isReloading = IsPlayingAnimation("Reload", 1);
        bool isThrowingGrenade = IsPlayingAnimation("ThrowGrenade", 1);
        // 오른손 IK
        if (RightHandMount != null)
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, RightHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, RightHandMount.rotation);
        }

        // 왼손 IK - 특정 상태에서만 활성화
        if (!isReloading && !isThrowingGrenade && LeftHandMount != null)
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandMount.rotation);
        }
        else
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
        }
    }
}

