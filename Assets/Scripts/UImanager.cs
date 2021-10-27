using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour
{

    public GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            // Quit the application
            SlideUIBack();
        }
    }

    public void SlideUI()
    {
        LeanTween.moveX(mainMenu, - Screen.width/2, .25f);
    }

    public void SlideUIBack()
    {
        LeanTween.moveLocalX(mainMenu, 0, .25f);
    }
}
