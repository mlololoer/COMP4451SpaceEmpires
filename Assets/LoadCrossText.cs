using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadCrossText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Text>().text = CrossSceneManager.CrossText;
    }
	 public   void press() {
    	SceneManager.LoadScene("Main");
    	CrossSceneManager.battleFinished = true;
    }
}
