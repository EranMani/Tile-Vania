using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float loadNewSceneTime = 0.5f;
    [SerializeField] float slowMotionAmount = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        Time.timeScale = slowMotionAmount;
        yield return new WaitForSeconds(loadNewSceneTime);
        Time.timeScale = 1f;

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
