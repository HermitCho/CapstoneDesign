using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class KeyRebindManager : MonoBehaviour
{
    [Header("Rebind 관련")]
    private PlayerInput playerInput;
    public GameObject rebindPanel;
    public Dictionary<string, TMP_Text> actionTextMap = new();

    [Header("Settings 패널")]
    public GameObject settingsPanel;   // 설정 패널

    [Header("UI 연결")]
    public TMP_Text keyText_Forward, keyText_Backward, keyText_Left, keyText_Right;
    public TMP_Text keyText_Reload, keyText_Sprint, keyText_Skill1, keyText_Skill2, keyText_HandleGun;

    [Header("감도/사운드 관련 UI")]
    public Slider xSensitivitySlider;
    public Slider ySensitivitySlider;
    public TMP_InputField xSensitivityField;
    public TMP_InputField ySensitivityField;
    public Slider soundSlider;
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
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
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

        // 리스너 등록 (초기화 이후에 연결)
        xSensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivitySliderChanged(); });
        ySensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivitySliderChanged(); });
        soundSlider.onValueChanged.AddListener(delegate { OnSoundSliderChanged(); });

        xSensitivityField.onEndEdit.AddListener(delegate { OnSensitivityInputChanged(); });
        ySensitivityField.onEndEdit.AddListener(delegate { OnSensitivityInputChanged(); });
        soundField.onEndEdit.AddListener(delegate { OnSoundInputChanged(); });

        // 현재 활성화된 플레이어의 PlayerInput 찾기
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput input in playerInputs)
        {
            if (input.gameObject.activeInHierarchy)
            {
                playerInput = input;
                break;
            }
        }

        // LoadSettings를 통해 설정 로드
        LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
        if (loadSettings != null)
        {
            loadSettings.LoadSettingsToUI(this);
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
        // playerInput이 없으면 현재 활성화된 플레이어의 PlayerInput을 찾아서 설정
        if (playerInput == null)
        {
            PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
            foreach (PlayerInput input in playerInputs)
            {
                if (input.gameObject.activeInHierarchy)
                {
                    playerInput = input;
                    break;
                }
            }

            if (playerInput == null)
            {
                Debug.LogError("KeyRebindManager: 활성화된 PlayerInput을 찾을 수 없습니다!");
                return;
            }
        }

        // PlayerAction 맵에서 액션 찾기
        InputAction action = playerInput.inputActions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"KeyRebindManager: 액션을 찾을 수 없습니다: {actionName}");
            return;
        }

        int bindingIndex = -1;

        // Move 액션의 경우 composite 바인딩 처리
        if (actionName == "Move" && !string.IsNullOrEmpty(bindingName))
        {
            bindingIndex = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => b.binding.name == bindingName && b.binding.isPartOfComposite)?.index ?? -1;
        }
        // 다른 액션들의 경우 단일 바인딩 처리
        else
        {
            // 첫 번째 실제 바인딩 찾기 (composite가 아닌)
            bindingIndex = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => b.binding.path.StartsWith("<Keyboard>"))?.index ?? -1;
        }

        if (bindingIndex == -1)
        {
            Debug.LogError($"바인딩을 찾을 수 없습니다: {actionName} ({bindingName})");
            return;
        }

        Debug.Log($"리바인딩 시작: {actionName} ({bindingName}), 바인딩 인덱스: {bindingIndex}");

        rebindPanel.SetActive(true);

        // 리바인딩 전에 액션 비활성화
        bool wasActionEnabled = action.enabled;
        if (wasActionEnabled)
        {
            action.Disable();
        }

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                action.ApplyBindingOverride(bindingIndex, op.selectedControl.path);
                op.Dispose();

                // 리바인딩 완료 후 액션 다시 활성화
                if (wasActionEnabled)
                {
                    action.Enable();
                }

                // 현재 키 바인딩 상태 저장
                string rebindsJson = playerInput.inputActions.SaveBindingOverridesAsJson();
                Debug.Log($"저장된 키 바인딩: {rebindsJson}");

                // LoadSettings를 통해 설정 저장
                LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
                if (loadSettings != null)
                {
                    // LoadSettings의 playerInput 업데이트
                    loadSettings.SetPlayerInput(playerInput);
                    
                    // 현재 설정값으로 저장
                    loadSettings.SaveSettingsToJson(xSensitivity, ySensitivity, soundVolume);
                    
                    // 키 바인딩 상태를 다시 로드하여 확실하게 적용
                    loadSettings.LoadSettingsToPlayer();
                }

                StartCoroutine(DelayedUIUpdate());
                rebindPanel.SetActive(false);
                playerInput.enabled = false;
            })
            .OnCancel(op =>
            {
                // 취소 시에도 액션 다시 활성화
                if (wasActionEnabled)
                {
                    action.Enable();
                }
                op.Dispose();
                rebindPanel.SetActive(false);
                playerInput.enabled = true;
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
        xSensitivity = xSensitivitySlider.value;
        ySensitivity = ySensitivitySlider.value;

        xSensitivityField.text = xSensitivity.ToString("F2");
        ySensitivityField.text = ySensitivity.ToString("F2");

        // LoadSettings를 통해 설정 저장
        LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
        if (loadSettings != null)
        {
            loadSettings.SaveSettingsToJson(xSensitivity, ySensitivity, soundVolume);
        }
    }

    private void OnSensitivityInputChanged()
    {
        if (float.TryParse(xSensitivityField.text, out float xVal))
            xSensitivitySlider.value = Mathf.Clamp(xVal, xSensitivitySlider.minValue, xSensitivitySlider.maxValue);
        else
            xSensitivityField.text = xSensitivity.ToString("F2");

        if (float.TryParse(ySensitivityField.text, out float yVal))
            ySensitivitySlider.value = Mathf.Clamp(yVal, ySensitivitySlider.minValue, ySensitivitySlider.maxValue);
        else
            ySensitivityField.text = ySensitivity.ToString("F2");

        // LoadSettings를 통해 설정 저장
        LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
        if (loadSettings != null)
        {
            loadSettings.SaveSettingsToJson(xSensitivity, ySensitivity, soundVolume);
        }
    }

    private void OnSoundSliderChanged()
    {
        soundVolume = soundSlider.value;
        soundField.text = soundVolume.ToString("F2");
        
        // LoadSettings를 통해 설정 저장
        LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
        if (loadSettings != null)
        {
            loadSettings.SaveSettingsToJson(xSensitivity, ySensitivity, soundVolume);
        }
    }

    private void OnSoundInputChanged()
    {
        if (float.TryParse(soundField.text, out float val))
            soundSlider.value = Mathf.Clamp(val, soundSlider.minValue, soundSlider.maxValue);
        else
            soundField.text = soundVolume.ToString("F2");

        // LoadSettings를 통해 설정 저장
        LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
        if (loadSettings != null)
        {
            loadSettings.SaveSettingsToJson(xSensitivity, ySensitivity, soundVolume);
        }
    }

    #endregion

    #region 💾 저장 및 불러오기

    private void SaveSettings()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            // 현재 UI의 값을 가져옴
            if (xSensitivitySlider != null) xSensitivity = xSensitivitySlider.value;
            if (ySensitivitySlider != null) ySensitivity = ySensitivitySlider.value;
            if (soundSlider != null) soundVolume = soundSlider.value;

            SettingsData data = new SettingsData
            {
                rebindsJson = playerInput != null ? playerInput.inputActions.SaveBindingOverridesAsJson() : "",
                xSensitivity = xSensitivity,
                ySensitivity = ySensitivity,
                soundVolume = soundVolume
            };

            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, jsonData);
            Debug.Log($"설정이 저장되었습니다. 경로: {savePath}\n저장된 값: X감도={xSensitivity}, Y감도={ySensitivity}, 소리={soundVolume}");
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
                if (xSensitivitySlider != null) xSensitivitySlider.value = xSensitivity;
                if (ySensitivitySlider != null) ySensitivitySlider.value = ySensitivity;
                if (soundSlider != null) soundSlider.value = soundVolume;
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
        if (xSensitivitySlider != null) xSensitivitySlider.value = xSensitivity;
        if (ySensitivitySlider != null) ySensitivitySlider.value = ySensitivity;
        if (soundSlider != null) soundSlider.value = soundVolume;
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

        xSensitivityField.text = xSensitivity.ToString("F2");
        ySensitivityField.text = ySensitivity.ToString("F2");
        soundField.text = soundVolume.ToString("F2");
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
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            // LoadSettings를 통해 설정 로드
            LoadSettings loadSettings = FindObjectOfType<LoadSettings>();
            if (loadSettings != null)
            {
                // LoadSettings의 playerInput 업데이트
                loadSettings.SetPlayerInput(playerInput);
                loadSettings.LoadSettingsToUI(this);
                // 키 바인딩 텍스트 업데이트
                UpdateAllUI();
            }
        }
    }

    // 설정 패널 비활성화
    public void OnCloseButtonClick()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}