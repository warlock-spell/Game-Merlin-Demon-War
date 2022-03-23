using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text score, kills;

    public void Awake()
    {
        score.text = "Score: " + GameController.instance.score.ToString();
        kills.text = "Demons killed: " + GameController.instance.kills.ToString();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    
}
