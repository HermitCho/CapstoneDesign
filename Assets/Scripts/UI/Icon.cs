using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    Image image;
    public bool cooldownCheck = false;
    public float cooldownTime;
    // Start is called before the first frame update

    void Start()
    {
        image = GetComponent<Image>();
        image.type = Image.Type.Filled;
        image.fillOrigin = 2;
        image.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownCheck)
            CooltimeImageAmountFill(image);
    }


    void CooltimeImageAmountFill(Image image)
    {
        if (image.type == Image.Type.Filled)
        {
            if (image.fillAmount < 1)
            {
                FillImage(image);
            }
            else if (image.fillAmount >= 1)

                cooldownCheck = false;
        }
    }
    void FillImage(Image image)
    {
        image.fillAmount += Time.deltaTime / cooldownTime;
    }
}
