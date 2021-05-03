using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
	public static CanvasScript CS;
    //Singleton function
    static bool alrun = false;
    void Awake() {
        if (!alrun){CS = this;}
        alrun = true;
    }

}
