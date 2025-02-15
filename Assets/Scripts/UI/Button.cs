using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject optionPanel; //Option Panel Object 
    public GameObject pausePanel; //Pause Panel Object

    private bool isOptionClick; //bool Option Panel 
    private bool isPauseClick; //bool Pause Panel

    private GameObject player;
    private void Awake()
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

    public void OnClickGameStart()
    {
        //GmaeManager.cs 메서드 불러오기
        //photon 사용 필요
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


    //test code
    public void Pause()
    {
  
        if (Input.GetKeyDown(KeyCode.Escape) && !isPauseClick)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = false;

            isPauseClick = true;
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            pausePanel.SetActive(isPauseClick);
            



        }

        else if (Input.GetKeyDown(KeyCode.Escape) && isPauseClick && !isOptionClick ) 
        {
            isPauseClick = false;
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            pausePanel.SetActive(isPauseClick);

            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }
    }
}
