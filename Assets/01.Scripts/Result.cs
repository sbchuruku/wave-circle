using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour {
    
    public class PlayerInfo
    {
        public string name;
        public int score;

        public PlayerInfo(string name, int score)
        {
            this.name = name;
            this.score = score;
        }

    }

    public enum ResultType
    {
        NONE = -1,
        INPUT,
        BOARD
    }

    public GameObject resultObj;
    public GameObject inputObj;
    public InputField inputField;
    public GameObject boardObj;
    public List<Text> nameTextList;
    public List<Text> scoreTextList;
    public Text scoreText;
    public GameObject retryObj;
    public GameObject gameOverObj;

    ResultType currentResultType = ResultType.NONE;

    int currentScore = 0;

    bool isSaved = false;
    bool isLoaded = false;

    [SerializeField]
    List<PlayerInfo> playerInfoList;

    public AudioSource audioSource;

    public AudioClip gameoverAudioClip;
    bool isGameOverPlayed = false;

    void Start () {
        playerInfoList = new List<PlayerInfo>();
        for (int i=0; i<5; i++)
        {
            playerInfoList.Add(new PlayerInfo(
                PlayerPrefs.GetString("top"+(i+1)+"name","-----"), 
                PlayerPrefs.GetInt("top"+(i+1)+"score", 0)));
        }
    }
	
	void Update () {
        
        switch (currentResultType)
        {
            case ResultType.NONE:
                resultObj.SetActive(false);
                break;

            case ResultType.INPUT:
                if (!isGameOverPlayed)
                {
                    audioSource.volume = 0.7f;
                    audioSource.PlayOneShot(gameoverAudioClip);
                    isGameOverPlayed = true;
                }
                
                gameOverObj.SetActive(true);
                resultObj.SetActive(true);
                inputObj.SetActive(true);
                inputField.ActivateInputField();
                inputField.Select();
                
                if (Input.GetKeyDown(KeyCode.Return)||Input.GetKey(KeyCode.Return))
                {
                    if (!isSaved)
                    {
                        SaveTopPlayer(new PlayerInfo(
                        inputField.text.ToString(),
                        currentScore));
                    }
                }

                break;

            case ResultType.BOARD:
                boardObj.SetActive(true);
                retryObj.SetActive(true);

                if (!isLoaded)
                {
                    for(int i=0; i<5; i++)
                    {
                        nameTextList[i].text = playerInfoList[i].name;
                        scoreTextList[i].text = playerInfoList[i].score.ToString();
                        PlayerPrefs.SetString("top" + (i + 1) + "name", playerInfoList[i].name);
                        PlayerPrefs.SetInt("top" + (i + 1) + "score", playerInfoList[i].score);

                    }
                    isLoaded = true;
                }
                
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    gameOverObj.SetActive(false);
                    boardObj.SetActive(false);
                    retryObj.SetActive(false);
                    CGameMgr gameMgr = GameObject.Find("GameMgr").GetComponent<CGameMgr>();
                    gameMgr.Reset();
                    currentResultType = ResultType.NONE;
                    isSaved = false;
                    isLoaded = false;
                    isGameOverPlayed = false;
                }
                break;
        }
    }

    void SaveTopPlayer(PlayerInfo playerInfo)
    {
        for (int i=0; i<playerInfoList.Count; i++)
        {
            if(playerInfoList[i].score <= playerInfo.score)
            {
                playerInfoList.Insert(i, playerInfo);
                PlayerPrefs.SetString("top" + (i+1) + "name", playerInfo.name);
                PlayerPrefs.SetInt("top" + (i+1) + "score", playerInfo.score);
                break;
            }
        }

        inputField.text = "";
        PlayerPrefs.Save();
        isSaved = true;
        inputObj.SetActive(false);
        currentResultType = ResultType.BOARD;
    }

    public void ScoreRefresh(int score)
    {
        currentScore = score;
        scoreText.text = currentScore.ToString();
    }

    public void GameOverProcess(int score)
    {
        currentResultType = ResultType.INPUT;
        currentScore = score;
        scoreText.text = score.ToString();
    }

}
