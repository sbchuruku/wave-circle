using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroButton : MonoBehaviour {

    public Image button;

    float alpha = 0.0f;
    bool isUp = true;

    public GameObject creditObj;

	void Update () {
        if (alpha >= 1)
        {
            isUp = false;
        }

        Color btnColor = button.color;
        if (isUp)
        {
            alpha += Time.deltaTime;
            btnColor.a = alpha;
            button.color = btnColor;
        } 
        else
        {
            alpha -= Time.deltaTime;
            btnColor.a = alpha;
            button.color = btnColor;
            if (alpha < 0)
            {
                isUp = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (creditObj.activeSelf)
            {
                creditObj.SetActive(false);
                return;
            }
            SceneManager.LoadScene("Main");
        }
	}

    public void CreditClick()
    {
        Debug.Log("Credit click");
        creditObj.SetActive(true);
    }
}
