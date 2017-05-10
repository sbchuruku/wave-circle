using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CIntro : MonoBehaviour
{
    public GameObject objCreditPanel;

	void Update()
    {
		if( Input.GetKeyDown(KeyCode.Space) )
        {
            SceneManager.LoadScene("Main");
        }
	}

    public void OnClickCredit()
    {
        if(objCreditPanel.activeSelf)
        {
            objCreditPanel.SetActive( false );
        }
        else
        {
            objCreditPanel.SetActive( true );
        }
    }

    public void OnclickRankingReset()
    {
        PlayerPrefs.DeleteAll();
    }
}
