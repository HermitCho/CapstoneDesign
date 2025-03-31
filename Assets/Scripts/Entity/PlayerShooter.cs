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

    private void OnAnimatorIK(int layerIndex)
    {
        //���� ������ gunPivot�� 3D���� ������ �Ȳ�ġ ��ġ�� �̵�
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK�� �̿��Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� �����̿� ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandMount.rotation);

        //IK�� �̿��Ͽ� �������� ��ġ�� ȸ���� ���� ���� �����̿� ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, RightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, RightHandMount.rotation);
    }
}
