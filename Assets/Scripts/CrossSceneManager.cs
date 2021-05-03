using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossSceneManager {
	public static string CrossText = "";
	public static PureUnit playerUnit {get;set;}
	public static PureUnit aiUnit {get;set;}
	public static bool battleFinished = false;
	public static bool battleOutcome = false;

	public static string EmpireName = "";

	public static bool humanWon = false;
}
