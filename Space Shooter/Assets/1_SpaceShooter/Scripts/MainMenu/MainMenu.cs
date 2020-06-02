using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {


    /***************************************************
    * Title:          Play Game
    * 
    * Description:    Permet de passer à la prochaine
    *                 scène simplement en incrémentant.
    *                 De cette façon, on atteint l'index
    *                 de la prochaine scène. 
    *                 
    *                 Scène Menu --> 0
    *                 Scène Jeu  --> 1
    *                                 
    ***************************************************/
    public void PlayGame()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (sceneIndex)
        {
            case 0: // MainMenu --> Game
                SceneManager.LoadScene(sceneIndex + 1);
                break;

            case 1: // Game --> new Game
                SceneManager.LoadScene(sceneIndex);
                break;
         
            default: //ERROR
                Debug.Log("ERROR - Back to MainMenu");
                SceneManager.LoadScene(0);
                break;
        }    
    }

    /***************************************************
    * Title:          Exit Game
    * 
    * Description:    Permet à l'utilisateur de quitter
    *                 le jeu.
    *                                  
    ***************************************************/
    public void ExitGame()
    {
        Application.Quit();
    }
}
