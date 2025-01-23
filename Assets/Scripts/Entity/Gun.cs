using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready, //�߻� �غ� �Ϸ�
        Empty, //źâ�� ��
        Reloading // ������ ��
    }

    public State state { get; private set; } // ���� �� ���� �ҷ�����

    public Transform fireTransform; //�Ѿ��� �߻�� ��ġ
    public ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    public ParticleSystem shellEjectEffect; //ź�� ���� ȿ��

    private LineRenderer bulletLineRenderer; //ź�� ���� ������
    private PlayerMovement PlayerMovement; //�Ѿ� �߻� ��ġ ã�� ���� ������Ƽ
    private PlayerInput playerInput; // 해당 총을 사용하는 캐릭터의 키인풋을 받아옴

    private AudioSource gunAudioPlayer; //�ѼҸ� ���
    public GunData gunData;// �� ������

    [HideInInspector] public int magAmmo; //���� źâ�� �����ִ� ź�� ��

    private float lastFireTime; //���� �������� �߻��� ����

    private float fireDistance; //�����Ÿ�

    //��� ������Ʈ ��������
    private void Awake()
    {
        //��� ������Ʈ ��������
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        PlayerMovement = GetComponentInParent<PlayerMovement>(); //Gun ������Ʈ�� �θ��� Player���� ������Ʈ ã��
        playerInput = GetComponentInParent<PlayerInput>();

        //����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        //���� ���ͷ� ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;

    }

    //��� ���� �ʱ�ȭ
    private void OnEnable()
    {
        //���� źâ ���� ä���
        magAmmo = gunData.magCapacity;
        //�����Ÿ� ����
        fireDistance = gunData.fireDistance;

        //�� ���¸� �غ���·� ����
        state = State.Ready;
        //������ ���� �� ���� �ʱ�ȭ
        lastFireTime = 0;

    }
    public void Handling()
    {
        if (playerInput.handleGunButton)
        {
            state = State.Ready;
            gameObject.SetActive(true);
        }
        else if (playerInput.skill_2_Button || playerInput.skill_1_Button)
        {
            state = State.Empty;
            gameObject.SetActive(false);
        }
    }


    //ȿ�� �� �Ҹ� ��� �ڷ�ƾ
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        //�ѱ� ȭ�� ȿ�� ���
        muzzleFlashEffect.Play();
        //ź�� ���� ȿ�� ���
        shellEjectEffect.Play();

        //�Ѱ� �Ҹ� ���
        gunAudioPlayer.PlayOneShot(gunData.shotClip);
        //���� ������: �ѱ��� ��ġ
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        //���� ����: �Է����� ���� �浹��ġ
        bulletLineRenderer.SetPosition(1, hitPosition);
        //���η����� Ȱ��ȭ
        bulletLineRenderer.enabled = true;

        //0.03�ʵ��� ó�� ���
        yield return new WaitForSeconds(0.03f);

        //���η��ͷ� ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }

    public void Fire()
    {
        //���� �߻� ���� ���� && ������ �߻� �������� gunData.timeBetFire �̻��� �ð��� ����
        if (state == State.Ready && (Time.time >= lastFireTime + gunData.timeBetFire))
        {
            //������ �� �߻� ���� ������Ʈ
            lastFireTime = Time.time;
            //���� �߻� ó��
            Shot();
        }

    }

    //���� �߻� ó�� �޼���
    private void Shot()
    {
        //����ĳ��Ʈ �浹 ���� ���� �����̳�
        RaycastHit hit;
        //ź���� ���� �� ���� ����
        Vector3 hitPosition = Vector3.zero;

        Vector3 fireDirection = PlayerMovement.LocalPosToWorldDirection();

        //����ĳ��Ʈ(��������, ����, �浹���� �����̳�, �����Ÿ�)
        if (Physics.Raycast(fireTransform.position, fireDirection, out hit, fireDistance))
        {
            //���̰� ��ü�� �浹�� ���
            //�浹�� ��ü�� ���� IDamageable ������Ʈ �������� �õ�
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            //�������κ��� IDamageable ������Ʈ�� �������µ� �����ߴٸ�
            if (target != null)
            {
                //������ OnDamage �Լ� ���� ��Ű��(������ �ֱ�)
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }

            //���̰� �浹�� ��ġ ����
            hitPosition = hit.point;
        }
        else
        {
            //���̰� ��ü�� �浹���� �ʾҴٸ�
            //ź���� �ִ� �����Ÿ����� ���ư������� ��ġ�� �浹��ġ�� ���
            hitPosition = fireTransform.position + fireDirection * fireDistance; //fireTransform.forward �κ� ���� �ʿ� 
        }

        //�߻� ����Ʈ ����
        StartCoroutine(ShotEffect(hitPosition));

        //���� ź�� �� -1
        magAmmo--;
        if (magAmmo <= 0)
        {
            //źâ�� ���� ź���� ���ٸ� �� ���� Empty�� ����
            state = State.Empty;
        }
    }


    public bool Reload()
    {
        if (state == State.Reloading || magAmmo >= gunData.magCapacity)
        {
            //������ ���̰ų� źâ�� �Ѿ��� ���� �� ��� X
            return false;
        }

        //�������� ������ ó��
        StartCoroutine(ReloadRoutine());
        return true;
    }

    //���� ������ ó��
    private IEnumerator ReloadRoutine()
    {
        //���� ���� ���¸� ������ ������ ����
        state = State.Reloading;
        //������ �Ҹ� ���
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);

        //������ �ð���ŭ ����
        yield return new WaitForSeconds(gunData.reloadTime);

        //źâ�� ä�� ź�� ���� ��� 
        int ammoToFill = gunData.magCapacity - magAmmo;

        //źâ ä���
        magAmmo += ammoToFill;
        //���� ���¸� �߻� �غ�� ����
        state = State.Ready;
    }
}
