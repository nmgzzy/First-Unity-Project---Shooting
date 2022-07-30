using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{

    public Image fadePlane;
    public GameObject gameOverUI;

    void Start()
    {
        Player p = FindObjectOfType<Player>();
        if (p)
        {
            p.OnDeath += OnGameOver;
        }
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    void Update()
    {
        //退出游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameOverUI.ExitGame();
        }

        //重启游戏
        if (Input.GetKeyDown(KeyCode.F4))
        {
            GameOverUI.PrepareNewGame();
        }
    }

    // UI Input
    static public void PrepareNewGame()
    {
        SceneManager.LoadScene("MapGenerator");
    }

    static public void StartNewGame()
    {
        SceneManager.LoadScene("ShootingGame");
    }

    static public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
