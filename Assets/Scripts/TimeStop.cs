using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStop : MonoBehaviour
{
    private float Speed;
    private bool RestoreTime;

    private void Start()
    {
        RestoreTime = false;
		if (RestoreTime) {}
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.timeScale < 1f)
        {
            Time.timeScale += Time.deltaTime*Speed;
        }
        else
        {
            Time.timeScale = 1f;
            RestoreTime = false;
        }
    }

    public void StopTime(float changeTime, int RestoreSpeed, float Delay)
    {
        Speed = RestoreSpeed;

        if(Delay > 0)
        {
            StopCoroutine(StartTimeAgain(Delay));
            StartCoroutine(StartTimeAgain(Delay));
        }
        else
        {
            RestoreTime = true;
        }

        Time.timeScale = changeTime;
    }

    IEnumerator StartTimeAgain(float amt)
    {
        yield return new WaitForSecondsRealtime(amt);
        RestoreTime = true;
    }
}
