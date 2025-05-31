using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.Heat;

public class KeyRebindManager : MonoBehaviour
{
    [Header("Rebind 관련")]
    private PlayerInput playerInput;
    public GameObject rebindPanel;
    public Dictionary<string, TMP_Text> actionTextMap = new();

    [Header("Pannel Manger 연결 - 할당 필요 X")]
    [SerializeField] private PanelManager panelManager;

    [Header("UI 연결")]
    public TMP_Text keyText_Forward, keyText_Backward, keyText_Left, keyText_Right;
    public TMP_Text keyText_Reload, keyText_Sprint, keyText_Skill1, keyText_Skill2, keyText_HandleGun;

    [Header("감도/사운드 관련 UI")]
    public SliderManager xSensitivitySlider;
    public SliderManager ySensitivitySlider;
    public SliderManager soundSlider;
    public TMP_InputField xSensitivityField;
    public TMP_InputField ySensitivityField;
    public TMP_InputField soundField;

    private float xSensitivity;
    private float ySensitivity;
    private float soundVolume;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private string savePath => Path.Combine(Application.persistentDataPath, "keybinds_settings.json");

    [System.Serializable]
    public class SettingsData
    {
        public string rebindsJson;
        public float xSensitivity;
        public float ySensitivity;
        public float soundVolume;
    }

    private void Awake()
    {
        if (rebindPanel != null)
        {
            rebindPanel.SetActive(false);
        }

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

    public void SetPlayerInput(PlayerInput newPlayerInput)
    {
        if (newPlayerInput == null)
        {
            Debug.LogError("KeyRebindManager: PlayerInput이 null입니다!");
            return;
        }

        playerInput = newPlayerInput;
        
        // 저장된 설정 불러오기
        LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
        if (loadSettings != null)
        {
            loadSettings.LoadSettingsToUI(this);
        }
        // UI 업데이트
        UpdateAllUI();
    }

    private void Start()
    {
        // Action과 UI 텍스트 매핑
        actionTextMap["Move_up"] = keyText_Forward;
        actionTextMap["Move_down"] = keyText_Backward;
        actionTextMap["Move_left"] = keyText_Left;
        actionTextMap["Move_right"] = keyText_Right;

        actionTextMap["Reload"] = keyText_Reload;
        actionTextMap["Sprint"] = keyText_Sprint;
        actionTextMap["Skill1"] = keyText_Skill1;
        actionTextMap["Skill2"] = keyText_Skill2;
        actionTextMap["HandleGun"] = keyText_HandleGun;

        // 리스너 등록
        xSensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivitySliderChanged(); });
        ySensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivitySliderChanged(); });
        soundSlider.onValueChanged.AddListener(delegate { OnSoundSliderChanged(); });

        // TMP_InputField는 onEndEdit 이벤트 사용
        xSensitivityField.onEndEdit.AddListener(delegate { OnSensitivityInputChanged(); });
        ySensitivityField.onEndEdit.AddListener(delegate { OnSensitivityInputChanged(); });
        soundField.onEndEdit.AddListener(delegate { OnSoundInputChanged(); });

        // 현재 씬 확인
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // PlayerInput 찾기
        if (currentScene == "Lobby")
        {
            // Lobby 씬에서는 KeyRebindManager 오브젝트의 PlayerInput 사용
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("KeyRebindManager: Lobby 씬에서 PlayerInput 컴포넌트를 찾을 수 없습니다!");
            }
        }
        else
        {
            // Game 씬에서는 Player 태그로 찾기
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInput = player.GetComponent<PlayerInput>();
                if (playerInput == null)
                {
                    Debug.LogError("KeyRebindManager: Player 오브젝트에서 PlayerInput 컴포넌트를 찾을 수 없습니다!");
                }
            }
            else
            {
                Debug.LogError("KeyRebindManager: Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
            }
        }

        // 초기 UI 업데이트
        UpdateAllUI();
    }

    private System.Collections.IEnumerator ApplySettingsNextFrame()
    {
        yield return null; // 다음 프레임까지 대기
        UpdateAllUI();
    }

    #region 🔁 키 리바인딩

    public void StartRebind(string actionName, string bindingName = null)
    {
        // 현재 씬 확인
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // Game 씬에서만 Player 태그로 PlayerInput 찾기
        if (currentScene != "Lobby")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInput = player.GetComponent<PlayerInput>();
            }
        }

        if (playerInput == null)
        {
            Debug.LogError("KeyRebindManager: PlayerInput을 찾을 수 없습니다!");
            return;
        }

        InputAction action = playerInput.inputActions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"KeyRebindManager: 액션을 찾을 수 없습니다: {actionName}");
            return;
        }

        int bindingIndex = -1;
        if (actionName == "Move" && !string.IsNullOrEmpty(bindingName))
        {
            bindingIndex = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => b.binding.name == bindingName && b.binding.isPartOfComposite)?.index ?? -1;
        }
        else
        {
            bindingIndex = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => !b.binding.isPartOfComposite)?.index ?? -1;
        }

        if (bindingIndex == -1)
        {
            Debug.LogError($"바인딩을 찾을 수 없습니다: {actionName} ({bindingName})");
            return;
        }

        rebindPanel.SetActive(true);
        bool wasActionEnabled = action.enabled;
        if (wasActionEnabled) action.Disable();

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                action.ApplyBindingOverride(bindingIndex, op.selectedControl.path);
                op.Dispose();

                if (wasActionEnabled) action.Enable();
                UpdateAllUI();
                rebindPanel.SetActive(false);
            })
            .OnCancel(op =>
            {
                if (wasActionEnabled) action.Enable();
                op.Dispose();
                rebindPanel.SetActive(false);
            })
            .Start();
    }

    private System.Collections.IEnumerator DelayedUIUpdate()
    {
        yield return null;
        UpdateAllUI();
    }

    public void CancelRebinding()
    {
        if (rebindingOperation != null)
        {
            rebindingOperation.Cancel();
            rebindingOperation.Dispose();
        }

        rebindPanel.SetActive(false);
        playerInput.enabled = true;
    }

    #endregion

    #region 🎚 감도 및 사운드 설정

    private void OnSensitivitySliderChanged()
    {
        xSensitivity = xSensitivitySlider.mainSlider.value;
        ySensitivity = ySensitivitySlider.mainSlider.value;

        xSensitivityField.text = xSensitivity.ToString("F2");
        ySensitivityField.text = ySensitivity.ToString("F2");

        // 실시간으로 UI 업데이트
        xSensitivitySlider.UpdateUI();
        ySensitivitySlider.UpdateUI();
    }

    private void OnSensitivityInputChanged()
    {
        if (float.TryParse(xSensitivityField.text, out float xVal))
        {
            xSensitivity = Mathf.Clamp(xVal, xSensitivitySlider.mainSlider.minValue, xSensitivitySlider.mainSlider.maxValue);
            xSensitivitySlider.mainSlider.value = xSensitivity;
        }
        else
        {
            xSensitivityField.text = xSensitivity.ToString("F2");
        }

        if (float.TryParse(ySensitivityField.text, out float yVal))
        {
            ySensitivity = Mathf.Clamp(yVal, ySensitivitySlider.mainSlider.minValue, ySensitivitySlider.mainSlider.maxValue);
            ySensitivitySlider.mainSlider.value = ySensitivity;
        }
        else
        {
            ySensitivityField.text = ySensitivity.ToString("F2");
        }

        // 실시간으로 UI 업데이트
        xSensitivitySlider.UpdateUI();
        ySensitivitySlider.UpdateUI();
    }

    private void OnSoundSliderChanged()
    {
        soundVolume = soundSlider.mainSlider.value;
        soundField.text = soundVolume.ToString("F2");

        // 실시간으로 UI 업데이트
        soundSlider.UpdateUI();
    }

    private void OnSoundInputChanged()
    {
        if (float.TryParse(soundField.text, out float val))
        {
            soundVolume = Mathf.Clamp(val, soundSlider.mainSlider.minValue, soundSlider.mainSlider.maxValue);
            soundSlider.mainSlider.value = soundVolume;
        }
        else
        {
            soundField.text = soundVolume.ToString("F2");
        }

        // 실시간으로 UI 업데이트
        soundSlider.UpdateUI();
    }

    #endregion

    #region 💾 저장 및 불러오기

    private void SaveSettings()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            SettingsData data = new SettingsData
            {
                rebindsJson = playerInput != null ? playerInput.inputActions.SaveBindingOverridesAsJson() : "",
                xSensitivity = xSensitivitySlider.mainSlider.value,
                ySensitivity = ySensitivitySlider.mainSlider.value,
                soundVolume = soundSlider.mainSlider.value
            };

            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, jsonData);
            Debug.Log($"설정이 저장되었습니다. 경로: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"설정 저장 중 오류 발생: {e.Message}");
        }
    }

    private void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string jsonData = File.ReadAllText(savePath);
                Debug.Log($"설정 파일을 읽었습니다. 경로: {savePath}\n내용: {jsonData}");
                
                var data = JsonUtility.FromJson<SettingsData>(jsonData);

                // 키 바인딩 설정 로드
                if (playerInput != null)
                {
                    playerInput.inputActions.LoadBindingOverridesFromJson(data.rebindsJson);
                }

                // 감도 및 소리 설정 로드
                xSensitivity = data.xSensitivity;
                ySensitivity = data.ySensitivity;
                soundVolume = data.soundVolume;

                Debug.Log($"설정을 로드했습니다. X감도={xSensitivity}, Y감도={ySensitivity}, 소리={soundVolume}");

                // UI 업데이트
                if (xSensitivitySlider != null) xSensitivitySlider.mainSlider.value = xSensitivity;
                if (ySensitivitySlider != null) ySensitivitySlider.mainSlider.value = ySensitivity;
                if (soundSlider != null) soundSlider.mainSlider.value = soundVolume;
                if (xSensitivityField != null) xSensitivityField.text = xSensitivity.ToString("F2");
                if (ySensitivityField != null) ySensitivityField.text = ySensitivity.ToString("F2");
                if (soundField != null) soundField.text = soundVolume.ToString("F2");

                UpdateAllUI();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"설정 파일을 로드하는 중 오류가 발생했습니다: {e.Message}");
                SetDefaultSettings();
            }
        }
        else
        {
            Debug.Log("설정 파일이 없습니다. 기본 설정을 적용합니다.");
            SetDefaultSettings();
        }
    }

    private void SetDefaultSettings()
    {
        xSensitivity = 1f;
        ySensitivity = 1f;
        soundVolume = 0.2f;

        // UI 업데이트
        if (xSensitivitySlider != null) xSensitivitySlider.mainSlider.value = xSensitivity;
        if (ySensitivitySlider != null) ySensitivitySlider.mainSlider.value = ySensitivity;
        if (soundSlider != null) soundSlider.mainSlider.value = soundVolume;
        if (xSensitivityField != null) xSensitivityField.text = xSensitivity.ToString("F2");
        if (ySensitivityField != null) ySensitivityField.text = ySensitivity.ToString("F2");
        if (soundField != null) soundField.text = soundVolume.ToString("F2");

        Debug.Log($"기본 설정이 적용되었습니다. X감도={xSensitivity}, Y감도={ySensitivity}, 소리={soundVolume}");
    }

    private void OnApplicationQuit()
    {
        if (playerInput != null && playerInput.inputActions != null)
        {
            string rebindsJson = playerInput.inputActions.SaveBindingOverridesAsJson();
            Debug.Log($"게임 종료 시 키 바인딩 저장: {rebindsJson}");
            
            // LoadSettings를 통해 설정 저장
            LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
            if (loadSettings != null)
            {
                loadSettings.SaveSettingsToJson(xSensitivity, ySensitivity, soundVolume);
            }
        }
        Debug.Log("게임 종료: 설정이 저장되었습니다.");
    }

    #endregion

    #region 🔄 UI 동기화

    public void UpdateAllUI()
    {
        if (playerInput == null) return;

        // 키 바인딩 UI 업데이트
        foreach (var pair in actionTextMap)
        {
            string actionName = pair.Key;
            TMP_Text textField = pair.Value;

            string[] parts = actionName.Split('_');
            string actionBase = parts[0];
            string bindingName = parts.Length > 1 ? parts[1] : null;

            InputAction action = playerInput.inputActions.FindAction(actionBase);
            if (action != null)
            {
                var binding = bindingName != null
                    ? action.bindings.FirstOrDefault(b => b.name == bindingName && b.isPartOfComposite)
                    : action.bindings.FirstOrDefault(b => !b.isPartOfComposite);

                if (binding != null && !string.IsNullOrEmpty(binding.effectivePath))
                {
                    textField.text = InputControlPath.ToHumanReadableString(
                        binding.effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                }
                else
                {
                    textField.text = "None";
                }
            }
        }

        // 감도 및 소리 UI 업데이트
        xSensitivitySlider.mainSlider.value = xSensitivity;
        ySensitivitySlider.mainSlider.value = ySensitivity;
        soundSlider.mainSlider.value = soundVolume;

        xSensitivityField.text = xSensitivity.ToString("F2");
        ySensitivityField.text = ySensitivity.ToString("F2");
        soundField.text = soundVolume.ToString("F2");

        // HeatUI 슬라이더 UI 업데이트
        xSensitivitySlider.UpdateUI();
        ySensitivitySlider.UpdateUI();
        soundSlider.UpdateUI();
    }

    #endregion

    #region 🔘 OnClick 함수

    public void OnClickForward() => StartRebind("Move", "up");
    public void OnClickBackward() => StartRebind("Move", "down");
    public void OnClickLeft() => StartRebind("Move", "left");
    public void OnClickRight() => StartRebind("Move", "right");

    public void OnClickReload() => StartRebind("Reload");
    public void OnClickCrouch() => StartRebind("Crouch");
    public void OnClickSprint() => StartRebind("Sprint");
    public void OnClickSkill1() => StartRebind("Skill1");
    public void OnClickSkill2() => StartRebind("Skill2");
    public void OnClickHandleGun() => StartRebind("HandleGun");

    #endregion

    // 설정 패널 활성화
    public void OnSettingsButtonClick()
    {
        if (panelManager != null)
        {
            panelManager.OpenPanelByIndex(1); // Settings 패널로 전환
            
            // 현재 씬 확인
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Game 씬에서만 Player 태그로 PlayerInput 찾기
            if (currentScene != "Lobby" && playerInput == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerInput = player.GetComponent<PlayerInput>();
                }
            }

            // LoadSettings를 통해 설정 로드
            LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
            if (loadSettings != null)
            {
                // Game 씬에서는 PlayerInput 설정
                if (currentScene != "Lobby" && playerInput != null)
                {
                    loadSettings.SetPlayerInput(playerInput);
                }
                
                // UI 업데이트
                loadSettings.LoadSettingsToUI(this);
                UpdateAllUI();
            }
        }
    }

    // 설정 패널 비활성화
    public void OnCloseButtonClick()
    {
        if (panelManager != null)
        {
            // 현재 설정 저장
            SaveSettings();

            // 현재 씬 확인
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Game 씬에서만 Player 태그로 PlayerInput 찾기
            if (currentScene != "Lobby")
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerInput = player.GetComponent<PlayerInput>();
                }

                // LoadSettings를 통해 설정 적용
                LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
                if (loadSettings != null && playerInput != null)
                {
                    loadSettings.SetPlayerInput(playerInput);
                    loadSettings.LoadSettingsToPlayer();
                }
            }

            // 현재 씬에 따라 적절한 패널로 전환
            if (currentScene == "SampleScene")
            {
                panelManager.OpenPanelByIndex(2); // Pause 패널로 전환
            }
            else if (currentScene == "Lobby")
            {
                panelManager.OpenPanelByIndex(0); // Main 패널로 전환
            }
        }
    }
}