using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class mainmenu : MonoBehaviour
{
    public string Ename;
    public void ReadStringInput(string s)
    { 
        Ename = s;
        Debug.Log(Ename);
    }
    public void Playgame()
    {
       if (Ename != "")
        {
            CrossSceneManager.EmpireName = Ename;
            SceneManager.LoadScene ("Main");
        }
       
    }
    public void Quitgame()
    {
        Application.Quit();
    }
}
