
using UnityEngine;

public class GunShake : MonoBehaviour
{
    public Transform gunShakePivot;
    public Animator animator;
    public bool isRunning;

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isReloading = stateInfo.IsName("Reload");

        if (isRunning || isReloading)
        {
            float shakeAmount = 0.02f;
            float speed = 10f;

            Vector3 shake = new Vector3(
                Mathf.Sin(Time.time * speed) * shakeAmount,
                Mathf.Cos(Time.time * speed * 0.5f) * shakeAmount, 0f
            );

            gunShakePivot.localPosition = shake;
        }
        else
        {
            gunShakePivot.localPosition = Vector3.zero;
        }
    }
}
