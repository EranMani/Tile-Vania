using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int playerScore = 0;

    [SerializeField] Text playerLivesTxt;
    [SerializeField] Text playerScoreTxt;

    private void Awake()
    {
        int numOfSessions = FindObjectsOfType<GameSession>().Length;
        if (numOfSessions > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerLivesTxt.text = playerLives.ToString();
        playerScoreTxt.text = playerScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int amount)
    {
        playerScore += amount;
        playerScoreTxt.text = playerScore.ToString();
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetGameSession();
        }
    }

    private void TakeLife()
    {
        playerLives--;
        playerLivesTxt.text = playerLives.ToString();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }
}
