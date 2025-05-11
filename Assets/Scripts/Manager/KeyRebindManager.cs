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
    public TMP_Text rebindWaitText;
    public Dictionary<string, TMP_Text> actionTextMap = new();

    [Header("UI 연결")]
    public TMP_Text keyText_Forward, keyText_Backward, keyText_Left, keyText_Right;
    public TMP_Text keyText_Reload, keyText_Crouch, keyText_Sprint, keyText_Skill1, keyText_Skill2, keyText_HandleGun;

    [Header("감도/사운드 관련 UI")]
    public PlayerMovement playerMovement;
    public AudioSource playerAudioSource;
    public AudioSource gunAudioSource;
    public Slider xSensitivitySlider;
    public Slider ySensitivitySlider;
    public TMP_InputField xSensitivityField;
    public TMP_InputField ySensitivityField;
    public Slider soundSlider;
    public TMP_Text soundTextTMP;

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
        rebindPanel.SetActive(false);
    }

    // PlayerInput 설정을 위한 public 메서드
    public void SetPlayerInput(PlayerInput newPlayerInput)
    {
        playerInput = newPlayerInput;
        if (playerInput != null)
        {
            LoadSettings(); // PlayerInput이 설정되면 설정 로드
            UpdateAllUI(); // UI 텍스트 업데이트
        }
    }

    private void Start()
    {
        // Action과 UI 텍스트 매핑
        actionTextMap["Move_up"] = keyText_Forward;
        actionTextMap["Move_down"] = keyText_Backward;
        actionTextMap["Move_left"] = keyText_Left;
        actionTextMap["Move_right"] = keyText_Right;

        actionTextMap["Reload"] = keyText_Reload;
        actionTextMap["Crouch"] = keyText_Crouch;
        actionTextMap["Sprint"] = keyText_Sprint;
        actionTextMap["Skill1"] = keyText_Skill1;
        actionTextMap["Skill2"] = keyText_Skill2;
        actionTextMap["HandleGun"] = keyText_HandleGun;

        // 리스너 등록 (초기화 이후에 연결)
        xSensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivitySliderChanged(); });
        ySensitivitySlider.onValueChanged.AddListener(delegate { OnSensitivitySliderChanged(); });
        xSensitivityField.onEndEdit.AddListener(delegate { OnSensitivityInputChanged(); });
        ySensitivityField.onEndEdit.AddListener(delegate { OnSensitivityInputChanged(); });

        soundSlider.onValueChanged.AddListener(delegate { OnSoundSliderChanged(); });

        //한 프레임 뒤에 적용
        StartCoroutine(ApplySettingsNextFrame());
        Debug.Log(xSensitivity + "+" + ySensitivity);

        UpdateAllUI();
    }

    private System.Collections.IEnumerator ApplySettingsNextFrame()
    {
        yield return null; // 다음 프레임까지 대기

        OnSetValue();
    }

    #region 🔁 키 리바인딩

    public void StartRebind(string actionName, string bindingName = null)
    {
        InputAction action = playerInput.inputActions.FindAction(actionName);
        if (action == null) return;

        int bindingIndex = -1;

        if (string.IsNullOrEmpty(bindingName))
        {
            bindingIndex = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => !b.binding.isPartOfComposite)?.index ?? -1;
        }
        else
        {
            bindingIndex = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => b.binding.name == bindingName && b.binding.isPartOfComposite)?.index ?? -1;
        }

        if (bindingIndex == -1)
        {
            Debug.LogError($"바인딩을 찾을 수 없습니다: {actionName} ({bindingName})");
            return;
        }

        rebindPanel.SetActive(true);
        playerInput.enabled = false;

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                action.ApplyBindingOverride(bindingIndex, op.selectedControl.path);
                op.Dispose();

                SaveSettings();
                StartCoroutine(DelayedUIUpdate());

                rebindPanel.SetActive(false);
                playerInput.enabled = false;
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

        SaveSettings();
    }

    public void OnSensitivityInputChanged()
    {
        if (float.TryParse(xSensitivityField.text, out float xVal))
            xSensitivitySlider.value = Mathf.Clamp(xVal, xSensitivitySlider.minValue, xSensitivitySlider.maxValue);
        else
            xSensitivityField.text = xSensitivity.ToString("F2");

        if (float.TryParse(ySensitivityField.text, out float yVal))
            ySensitivitySlider.value = Mathf.Clamp(yVal, ySensitivitySlider.minValue, ySensitivitySlider.maxValue);
        else
            ySensitivityField.text = ySensitivity.ToString("F2");

        SaveSettings();
    }

    private void OnSoundSliderChanged()
    {
        soundVolume = soundSlider.value;
        soundTextTMP.text = soundVolume.ToString("F2");
        SaveSettings();
    }

    #endregion

    #region 💾 저장 및 불러오기

    private void SaveSettings()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

        SettingsData data = new SettingsData
        {
            rebindsJson = playerInput.inputActions.SaveBindingOverridesAsJson(),
            xSensitivity = xSensitivity,
            ySensitivity = ySensitivity,
            soundVolume = soundVolume
        };

        File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
    }

    private void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            var data = JsonUtility.FromJson<SettingsData>(File.ReadAllText(savePath));

            playerInput.inputActions.LoadBindingOverridesFromJson(data.rebindsJson);
            xSensitivity = data.xSensitivity;
            ySensitivity = data.ySensitivity;
            soundVolume = data.soundVolume;
        }
        else
        {
            xSensitivity = 1f;
            ySensitivity = 1f;
            soundVolume = 0.2f;
        }

        xSensitivitySlider.value = xSensitivity;
        ySensitivitySlider.value = ySensitivity;
        soundSlider.value = soundVolume;

        UpdateAllUI();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    #endregion

    #region 🔄 UI 동기화

    private void UpdateAllUI()
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
        soundTextTMP.text = soundVolume.ToString("F2");
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

    public void OnSetValue()
    {
        playerMovement.xMouseSensitivity = xSensitivity;
        playerMovement.yMouseSensitivity = ySensitivity;

        playerAudioSource.volume = soundVolume;
        gunAudioSource.volume = soundVolume;
    }
}