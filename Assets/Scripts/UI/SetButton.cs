using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SetButton : MonoBehaviour
{
    public GameObject optionPanel; //Option Panel Object 
    public GameObject pausePanel; //Pause Panel Object

    private bool isOptionClick; //bool Option Panel 
    private bool isPauseClick; //bool Pause Panel

    private GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        optionPanel.SetActive(false);
        pausePanel.SetActive(false);
        isOptionClick = false;
        isPauseClick = false;
    }

    private void Update()
    {
        Pause();
        IsExitOption();
    }

    public void IsExitOption()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && optionPanel != null && isOptionClick == true)
        {
            OnClickOption();
        }
    }
    
    public void OnClickOption()
    {
        if (optionPanel != null) 
        {
            isOptionClick = !isOptionClick;
            optionPanel.SetActive(isOptionClick);     
        }
    }

    public void OnClickGameExit()
    {
    }

    public void ExitPause()
    {
        isPauseClick = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        pausePanel.SetActive(false);

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.enabled = true;
    }

    public void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pausePanel.activeSelf)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = false;

            isPauseClick = true;
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            pausePanel.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pausePanel.activeSelf && !optionPanel.activeSelf)
        {
            isPauseClick = false;
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            pausePanel.SetActive(false);

            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }
    }
}
