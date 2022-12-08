using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuButton : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    public void PlayClip()
    {
        AudioManager.Instance.PlaySFX(clip);
    }

    public void LoadScene(string scene)
    {
        GameManager.Instance.OnLoadScene(scene);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
