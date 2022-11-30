using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : Singleton<Pause>
{
    [SerializeField] GameObject pauseUI;

    bool isPaused = false;

    public bool paused
    {
        get { return isPaused; }
        set
        {
            isPaused = value;
            //pauseUI.SetActive(isPaused);
            Time.timeScale = (isPaused) ? 0 : 1;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }
    }
}