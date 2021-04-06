using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
	public int health = 100;
	public string spaceshipName = "Untitled";
	public GameObject UIPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Clicked() {
    	Debug.Log("aaa");
    	UIPanel.SetActive(true);
    }
}