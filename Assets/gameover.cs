using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameover : MonoBehaviour
{
    public bool wewon = false;
    public Text dialogueText;
    // Start is called before the first frame update
    public void endgamecomment()
    {
        if (wewon)
        {
            dialogueText.text = "Congratulations you have consolidated your rule into one of greatness having conquered the solar system it is time to go beyond the boundaries and spread your rule to the entire galaxy";
        }
        else
        {
            dialogueText.text = "Though you were valiant and steadfast your opponents were stronger and they managed to crush you with their prowess. Now nothing stands in their way to total domination.";
        }
    }
    public void Mainmenugame()
    {
        SceneManager.LoadScene(0);
    }
    public void Quitgame()
    {
        Application.Quit();
    }
}
