using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField inputTextField;
    [SerializeField] TextMeshProUGUI inputText;

    string curText;
    int[] curArr;
    List<int> curIndexArr = new List<int>();
    int curL;


    Coroutine guessCor;

    public void Start()
    {

    }

    //buttons
    public void OnStartButton()
    {
        ConverText(inputText.text);

    }
    public void OnStopButton()
    {
        if (guessCor != null)
        {
            StopCoroutine(guessCor);
        }
    }

    //methods
    void ConverText(string text)
    {
        curArr = text.Where(char.IsLetter).Select(char.ToLower).Select(c => (int)c).ToArray();
        if (curArr.Length < 1)
        {
            ShowError();
            return;
        }

        curText = text;
        curL = curText.Length - 1;
        inputTextField.interactable = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsLetter(text[i]))
            {
                curIndexArr.Add(i); 
            }
        }
        
        guessCor = StartCoroutine(GuessCor());
    }

    void ShowError()
    {
        Debug.Log("Error");
    }

    void CompleteGuessing()
    {
        Debug.Log("FIN");
    }

    void UpdateOutput()
    {
        int amount = curIndexArr[Settings.curGuessedAmount]+1;
        string colored = $"<color=green>{curText.Substring(0, amount)}</color>";
        string end = curText.Substring(amount, curL - amount);

        Debug.Log("COLORED "+colored);
        Debug.Log(curText);

        inputTextField.text = $"{colored}{end}";
    }


    IEnumerator GuessCor()
    {
        int l = curArr.Length;
        int i = 0;

        while (Settings.curGuessedVal < l)
        {
            int random = Random.Range(97, 123);
            if (curArr[i] == random)
            {
                Settings.curGuessedVal++;
                if (Settings.curGuessedVal > Settings.curGuessedAmount)
                {
                    UpdateOutput();
                    Settings.curGuessedAmount++;
                }
            }
            else
            {
                Debug.Log("d");
                i = 0;
                Settings.curGuessedVal = 0;
            }
            yield return null;
        }

        CompleteGuessing();
    }
}
