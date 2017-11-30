using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //to load scenes

public class MainMenu : MonoBehaviour {

	static public int playerTotal = 2;
	static public int playerSettings = 0;

	void Update()
	{
		// At any point, hitting the ESC key will quit the Application
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	public void LoadScenePvP(int num)
	{
		playerTotal = num;
		playerSettings = 0;
		SceneManager.LoadScene (1);
	}

	public void LoadScenePvE(int num)
	{
		playerTotal = num;
		playerSettings = 1;
		SceneManager.LoadScene (1);
	}

    public void LoadSceneEvE(int num)
    {
        playerTotal = num;
        playerSettings = 2;
        SceneManager.LoadScene(1);
    }

}
