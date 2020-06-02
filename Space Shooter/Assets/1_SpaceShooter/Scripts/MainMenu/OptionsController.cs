using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum Difficulties { easy = 0, normal = 1, hard = 2 }

public class OptionsController : MonoBehaviour
{
    //Niveau de Difficulté
    public static Difficulties difficulty = Difficulties.normal; 
    public TMP_Dropdown difficulteDropdown;

    //Parametres du niveau de difficulté de depart
    public static int hazardsSize = 4;
    public static int hazardCount = 30;

    //Objets Accelerometre
    public Slider accelSlider;
    public Button accelBtn;
    public Text textSliderValue;
    CanvasGroup canvasGroup;

    //Etat Accelerometre
    public static float accelValue = 1;
    public static bool isAccelSelected = false;

    //Objet Tactile
    public Button tactileBtn;


    void Start()
    {
        // Valeur du dropdown (difficulté)
        difficulteDropdown.value = (int)difficulty;
        DropdownValueChanged((int)difficulty);

        // Valeur des objets de l'accelerometre 
        accelSlider.value = accelValue;
        canvasGroup = accelSlider.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;   
         
        // Initialise le mode de deplacement du hero
        if (isAccelSelected) accelBtn.onClick.Invoke();
        else tactileBtn.onClick.Invoke();
    }

    // Initialise le niveau de difficulté selon le choix de l'utilisateur.
    public void DropdownValueChanged(int index)
    {
        switch (index)
        {
            case 0: //Easy
                hazardsSize = 3;
                hazardCount = 20;
                difficulty = (Difficulties)index;
                EvasiveManeuver.difficultyManeuverTime = 1.0f;
                Mover.difficultySpeedMultiplier = 1.0f;
                GameController.scoreMinimum = 150;
                break;

            case 1: //Normal
                hazardsSize = 4;
                hazardCount = 40;
                difficulty = (Difficulties)index;
                EvasiveManeuver.difficultyManeuverTime = 2.0f;
                Mover.difficultySpeedMultiplier = 1.2f;
                GameController.scoreMinimum = 350;
                break;

            case 2: //Hard
                hazardsSize = 4;
                hazardCount = 60;
                difficulty = (Difficulties)index;
                EvasiveManeuver.difficultyManeuverTime = 4.0f;
                Mover.difficultySpeedMultiplier = 1.6f;
                GameController.scoreMinimum = 550;
                break;

            default: //ERROR
                Debug.Log("ERROR - Easy difficulty activated.");
                DropdownValueChanged(0);
                break;
        }
    }

    //Bouton Accelerometre
    public void AccelBtnOnClick()
    {
        //Rend utilisable le slider Accelerometre
        canvasGroup.alpha = 1.0f;
        accelSlider.enabled = true;
        isAccelSelected = true;

        //Modififie la couleur pour focusser sur le bouton accelerometre
        accelBtn.GetComponentInChildren<TextMeshProUGUI>().color = new Color(250.0f / 255.0f, 198.0f / 255.0f, 66.0f / 255.0f);
        accelBtn.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Bold;

        tactileBtn.GetComponentInChildren<TextMeshProUGUI>().color = new Color(200.0f / 255.0f, 200.0f / 255.0f, 200.0f / 255.0f);
        tactileBtn.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Normal;
    }

    //Bouton tactile
    public void TactileBtnOnClick()
    {
        //Rend inutilisable le slider Accelerometre
        canvasGroup.alpha = 0.3f;
        accelSlider.enabled = false;
        isAccelSelected = false;

        //Modififie la couleur pour focusser sur le bouton tactile
        tactileBtn.GetComponentInChildren<TextMeshProUGUI>().color = new Color(250.0f / 255.0f, 198.0f / 255.0f, 66.0f / 255.0f);
        tactileBtn.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Bold;

        accelBtn.GetComponentInChildren<TextMeshProUGUI>().color = new Color(200.0f / 255.0f, 200.0f / 255.0f, 200.0f / 255.0f);
        accelBtn.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Normal;
    }

    // Initialise la valeur de l'accelerometre changé par l'utilisateur
    public void accelerometerOnValueChanged(float value)
    {
        accelValue = value;
    }

    // Maintient a jour le label du slider accelerometre 
    public void ShowSliderValue()
    {
        textSliderValue.text = Math.Round(accelSlider.value, 1).ToString();
    }

    void Update()
    {
        if (isAccelSelected) ShowSliderValue();
    }
}
