using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Gun 오브젝트를 쏘거나 재장전 
//IK 사용
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
                //재장전 성공시에만 애니메이션 재생
                playerAnimator.SetTrigger("Reload");
            }
        }

        //UI갱신
        //UpdateUI();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //총의 기준점 gunPivot을 3D모델의 오른쪽 팔꿈치 위치로 이동
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK를 이용하여 왼손의 위치와 회전을 총의 왼쪽 손잡이에 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandMount.rotation);

        //IK를 이용하여 오른손의 위치와 회전을 총의 왼쪽 손잡이에 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, RightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, RightHandMount.rotation);
    }

}
