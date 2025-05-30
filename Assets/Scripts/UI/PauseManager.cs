using UnityEngine;
using UnityEngine.InputSystem;
using Michsky.UI.Heat;

public class PauseManager : MonoBehaviour
{
    [Header("패널 관리")]
    [SerializeField] private PanelManager panelManager;
    private bool isPaused = false;

    [Header("플레이어 입력")]
    private PlayerInput playerInput;

    private void Awake()
    {
        // PanelManager 찾기
        if (panelManager == null)
        {
            panelManager = FindObjectOfType<PanelManager>();
            if (panelManager == null)
            {
                Debug.LogError("PanelManager를 찾을 수 없습니다!");
            }
        }
    }

    private void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Player 태그를 가진 오브젝트의 PlayerInput 컴포넌트 찾기
    private PlayerInput FindPlayerInput()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return player.GetComponent<PlayerInput>();
        }
        return null;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            // 일시정지 패널로 전환 (인덱스 2)
            if (panelManager != null)
            {
                panelManager.OpenPanelByIndex(2);
            }
            
            // 마우스 커서 표시
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // PlayerInput 비활성화
            playerInput = FindPlayerInput();
            if (playerInput != null)
            {
                playerInput.enabled = false;
                Debug.Log("플레이어 캐릭터의 PlayerInput 비활성화됨");
            }
        }
        else
        {
            // 게임 플레이 패널로 전환 (인덱스 0)
            if (panelManager != null)
            {
                panelManager.OpenPanelByIndex(0);
            }
            
            // 마우스 커서 숨기기
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // PlayerInput 활성화
            if (playerInput != null)
            {
                playerInput.enabled = true;
                Debug.Log("플레이어 캐릭터의 PlayerInput 활성화됨");
            }
        }
    }

    // 옵션 버튼 클릭 시 호출될 메서드
    public void OnClickOption()
    {
        if (panelManager != null)
        {
            panelManager.OpenPanelByIndex(1); // 옵션 패널로 전환

            // PlayerInput 비활성화
            playerInput = FindPlayerInput();
            if (playerInput != null)
            {
                playerInput.enabled = false;
                Debug.Log("플레이어 캐릭터의 PlayerInput 비활성화됨 (옵션 메뉴)");
            }
        }
    }

    // 일시정지 메뉴에서 나가기 버튼 클릭 시 호출될 메서드
    public void ExitPause()
    {
        isPaused = false;
        
        // 게임 플레이 패널로 전환 (인덱스 0)
        if (panelManager != null)
        {
            panelManager.OpenPanelByIndex(0);
        }
        
        // 마우스 커서 숨기기
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // PlayerInput 활성화
        if (playerInput != null)
        {
            playerInput.enabled = true;
            Debug.Log("플레이어 캐릭터의 PlayerInput 활성화됨");
        }
    }
} 