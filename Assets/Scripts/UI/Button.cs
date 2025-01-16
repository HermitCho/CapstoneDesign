using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject optionCanvas; //Option Panel Object 

    private bool isOptionClick; //bool Option Panel 

    private void Awake()
    {
        optionCanvas.SetActive(false);
        isOptionClick = false;
    }



    private void Update()
    {
        TestCode();
    }

    public void OnClickGameStart()
    {
        //GmaeManager.cs 메서드 불러오기
        //photon 사용 필요
    }
    
    public void OnClickOption()
    {

        if (optionCanvas != null) 
        {
            isOptionClick = !isOptionClick;
            optionCanvas.SetActive(isOptionClick);     
        }
        
    }

    public void OnClickGameExit()
    {

    }


    //test code
    public void TestCode()
    {
  
        if (Input.GetKeyDown(KeyCode.Escape) && !isOptionClick)
        {
  
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            OnClickOption();

        }

        else if (Input.GetKeyDown(KeyCode.Escape) && isOptionClick)
        {

            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            OnClickOption();

        }
    }
}
