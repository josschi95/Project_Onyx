using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionPanel : MonoBehaviour
{
    #region - Singleton -
    public static SelectionPanel instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of ConfirmationPanel found");
            return;
        }

        instance = this;
    }
    #endregion

    public TMP_Text header, description;
    public Button button00, button01, button02, button03, button04;
    [SerializeField] private TMP_Text option00Text, option01Text, option02Text, option03Text, option04Text; 

    public void DisplayOptions(string option0, string option1, string option2, string option3, string option4)
    {
        ClearAll();

        option00Text.text = option0;
        option01Text.text = option1;
        option02Text.text = option2;
        option03Text.text = option3;
        option04Text.text = option4;

        if (option1 != "") button01.gameObject.SetActive(true);

        if (option2 != "") button02.gameObject.SetActive(true);

        if (option3 != "") button03.gameObject.SetActive(true);

        if (option4 != "") button04.gameObject.SetActive(true);
    }

    private void ClearAll()
    {
        header.text = "";
        description.text = "";
        option00Text.text = "";

        button01.gameObject.SetActive(false);
        option01Text.text = "";

        button02.gameObject.SetActive(false);
        option02Text.text = "";

        button03.gameObject.SetActive(false);
        option03Text.text = "";

        button04.gameObject.SetActive(false);
        option04Text.text = "";
    }
}
