using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] Image image = null;
    [SerializeField] float time;
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;
    [SerializeField] bool startOnAwake = true;

    public bool isDone { get; set; } = false;

    void Start()
    {
        if (startOnAwake)
        {
            FadeIn();
        }
    }

	private void Update()
	{
        // only used for debugging
		if (Input.GetKeyDown(KeyCode.UpArrow)) FadeIn();
		if (Input.GetKeyDown(KeyCode.DownArrow)) FadeOut();
    }

    public void FadeIn()
	{
        StartCoroutine(FadeRoutine(startColor, endColor, time));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(endColor, startColor, time));
    }

    IEnumerator FadeRoutine(Color color1, Color color2, float time)
    {
        isDone = false;
        float timer = 0;
        while (timer < time)
        {
            timer = timer + Time.deltaTime;
            image.color = Color.Lerp(color1, color2, timer/time);

            yield return null;
        }

        isDone = true;
    }
}
