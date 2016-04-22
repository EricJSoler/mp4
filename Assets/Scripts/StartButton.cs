using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {
	
	public void ChangeScene(string sceneName) {
        Application.LoadLevel(sceneName); // load the game scene
        //Application.LoadLevel("level2"); // code correctly loads the game scene
    }
}
