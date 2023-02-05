using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    [SerializeField] bool isPause = false;
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject PauseBtn;
    public Sprite Spr_Pause;
    public Sprite Spr_Resume;


    // Start is called before the first frame update
    void Start()
    {
        isPause = false;
        PausePanel = GameObject.Find("PausePanel");
        PauseBtn = GameObject.Find("Btn_Pause");
        PausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        Debug.Log("Resume");
        isPause = false;
        PausePanel.SetActive(isPause);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Restart()
    {
        //Do we need a restart button?
    }

    public void MainNenu()
    {
        //Return Main Menu or something
    }

    public void Btn_Pause()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;
            PauseBtn.GetComponent<Image>().sprite = Spr_Resume;
        }
        else
        {
            Time.timeScale = 1;
            PauseBtn.GetComponent<Image>().sprite = Spr_Pause;
        }
        PausePanel.SetActive(isPause);
    }
}
