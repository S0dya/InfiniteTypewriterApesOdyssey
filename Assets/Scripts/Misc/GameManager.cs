using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static LeanTween;

public class GameManager : SingletonMonobehaviour<GameManager>, ISaveable
{
    [SerializeField] TMP_InputField inputTextField;

    [SerializeField] TextMeshProUGUI[] stats;//0 totalIterationAmount 1 curTotalIteration 2 totalGuessedAmount 3 curTotalGuessedAmount 4 totalGuessedTexts 5 longestTextGuessed

    [SerializeField] CanvasGroup startButtonCanvasGroup;
    [SerializeField] CanvasGroup stopButtonCanvasGroup;

    [SerializeField] CanvasGroup infoTabCanvasGroup;
    [SerializeField] TextMeshProUGUI infoTextHead;
    [SerializeField] TextMeshProUGUI infoTextBody;
    [SerializeField] GameObject dialogCloudObject;
    bool inInfo;
    int infoType;

    [SerializeField] CanvasGroup settingsTabCanvasGroup;
    [SerializeField] Image musicButtonImage;
    Color musicOnColor = new Color(0f, 0f, 0f, 1f);
    Color musicOffColor = new Color(0f, 0f, 0f, 0.588f);

    string curText;
    int[] curArr;
    List<int> curIndexArr;
    int curL;

    Coroutine guessCor;

    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();
        GameObjectSave = new GameObjectSave();

    }

    void Start()
    {
        SaveManager.I.LoadDataFromFile();
        Clear();

        if (Settings.firstTime)
        {
            infoType = 0;
            ToggleInfoTab(true);
            Settings.firstTime = false;
        }


        UpdateStats();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inInfo)
            {
                OnCloseInfoButton();
            }
            else
            {
                Application.Quit();
            }
        }
    }

    //buttons
    public void OnStartButton()
    {
        ConverText(inputTextField.text);

    }
    public void OnStopButton()
    {
        if (guessCor != null)
        {
            StopCoroutine(guessCor);
        }
        infoType = 3;
        ToggleInfoTab(true);
        Clear();
    }

    public void OnCloseInfoButton()
    {
        ToggleInfoTab(false);
    }
    
    public void OnOpenSettingsButton()
    {
        ToggleSettingsTab(true);
    }
    public void OnMusicButton()
    {
        bool cur = !Settings.isMusicEnabled;
        Settings.isMusicEnabled = cur;
        AudioManager.I.ToggleSFX(cur);
        if (cur)
        {
            musicButtonImage.color = musicOnColor;
        }
        else
        {
            musicButtonImage.color = musicOffColor;
        }
    }
    public void onQuitGameButton()
    {
        Application.Quit();
    }
    public void OnCloseSettingsButton()
    {
        ToggleSettingsTab(false);
    }


    public void OnButtonSound()
    {
        AudioManager.I.PlayOneShot("Button");
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

        ToggleStartButton(false);

        curText = text;
        curL = curText.Length;
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

    void UpdateStat(int i)
    {
        string s = null;
        switch(i)
        {
            case 0:
                s = $"total iterations: <color=yellow>{Settings.totalIteration}</color>";
                break;
            case 1:
                s = $"total iterations on this run: <color=yellow>{Settings.curTotalIteration}</color>";
                break;
            case 2:
                s = $"total guessed letters: <color=yellow>{Settings.totalGuessedAmount}</color>";
                break;
            case 3:
                s = $"guessed letters on this run: <color=yellow>{Settings.curTotalGuessedAmount}</color>";
                break;
            case 4:
                s = $"total guessed texts: <color=yellow>{Settings.totalGuessedTexts}</color>";
                break;
            case 5:
                s = $"longest guessed text: <color=yellow>{Settings.longestTextGuessed}</color>";
                break;
            default:
                Debug.Log("switch");
                break;
        }
        stats[i].text = s;
    }
    void UpdateStats()
    {
        for (int i = 0; i < 6; i++)
        {
            UpdateStat(i);
        }
    }

    void Clear()
    {
        Settings.totalIteration += Settings.curTotalIteration;
        Settings.totalGuessedAmount += Settings.curTotalGuessedAmount;

        Settings.curGuessedVal = 0;
        Settings.curGuessedAmount = 0;
        Settings.curTotalIteration = 0;
        Settings.curTotalGuessedAmount = 0;

        curArr = null;
        curText = null;
        curL = 0;
        curIndexArr = new List<int>();

        inputTextField.text = null;
        inputTextField.interactable = true;

        UpdateStats();
        ToggleStartButton(true);
    }

    void ShowError()
    {
        infoType = 2;
        ToggleInfoTab(true);
        Clear();
    }

    void CompleteGuessing()
    {
        AudioManager.I.PlayOneShot("Finished");
        Settings.totalGuessedTexts++;
        infoType = 1;
        ToggleInfoTab(true);
        Clear();
    }

    void UpdateOutput()
    {
        int amount = curIndexArr[Settings.curGuessedAmount]+1;
        string guessed = curText.Substring(0, amount);
        string colored = $"<color=red>{curText.Substring(0, amount)}</color>";
        string end = curText.Substring(amount, curL - amount);

        //Debug.Log("COLORED "+colored);
        //Debug.Log(curText);

        if (amount > Settings.longestTextGuessed.Length)
        {
            Settings.longestTextGuessed = guessed;
            UpdateStat(5);
        }

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
                    Settings.curTotalGuessedAmount++;
                    UpdateStat(3);
                }
            }
            else
            {
                Debug.Log("d");
                i = 0;
                Settings.curGuessedVal = 0;
            }
            Settings.curTotalIteration++;
            UpdateStat(1);
            yield return null;
        }

        CompleteGuessing();
    }

    void ToggleStartButton(bool val)
    {
        startButtonCanvasGroup.blocksRaycasts = val;
        stopButtonCanvasGroup.blocksRaycasts = !val;
        LeanTween.alphaCanvas(startButtonCanvasGroup, (val ? 1f : 0), 0.2f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.alphaCanvas(stopButtonCanvasGroup, (!val ? 1f : 0), 0.2f).setEase(LeanTweenType.easeInOutQuad);
    }
    void ToggleInfoTab(bool val)
    {
        infoTabCanvasGroup.blocksRaycasts = val;
        if (val)
        {
            AudioManager.I.PlayOneShot("Dialoge");
            infoTextHead.text = Settings.infoStringsHead[infoType];
            switch(infoType)
            {
                case 1:
                    infoTextBody.text = $"{Settings.infoStringsBody[infoType]}<color=blue>{curText}</color>";
                    break;
                case 3:
                    infoTextBody.text = $"{Settings.infoStringsBody[infoType]}<color=blue>{curText.Substring(0, curIndexArr[Settings.curGuessedAmount])}</color>";
                    break;
                default:
                    infoTextBody.text = $"{Settings.infoStringsBody[infoType]}";
                    break;
            }
        }

        LeanTween.scale(dialogCloudObject, (val? Vector2.one : Vector2.zero), 1.3f).setEase(LeanTweenType.easeOutBack);
        LeanTween.alphaCanvas(infoTabCanvasGroup, (val ? 1f : 0), 0.5f).setEase(LeanTweenType.easeInOutQuad);
    }

    void ToggleSettingsTab(bool val)
    {
        settingsTabCanvasGroup.blocksRaycasts = val;
        LeanTween.alphaCanvas(settingsTabCanvasGroup, (val ? 1f : 0), 0.2f).setEase(LeanTweenType.easeInOutQuad);
    }

    //save
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveManager.I.SaveDataToFile();
        }
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveManager.I.SaveDataToFile();
        }
    }
    void OnApplicationQuit()
    {
        Clear();
        SaveManager.I.SaveDataToFile();
    }

    void OnEnable()
    {
        ISaveableRegister();
    }
    void OnDisable()
    {
        ISaveableDeregister();
    }

    public void ISaveableRegister()
    {
        SaveManager.I.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveManager.I.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.GameScene);

        SceneSave sceneSave = new SceneSave();

        sceneSave.longDictionary = new Dictionary<string, long>();
        sceneSave.intDictionary = new Dictionary<string, int>();
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.longDictionary.Add("totalIteration", Settings.totalIteration);
        sceneSave.intDictionary.Add("totalGuessedAmount", Settings.totalGuessedAmount);
        sceneSave.intDictionary.Add("totalGuessedTexts", Settings.totalGuessedTexts);

        sceneSave.stringDictionary.Add("longestTextGuessed", Settings.longestTextGuessed);

        sceneSave.boolDictionary.Add("firstTime", Settings.firstTime);
        sceneSave.boolDictionary.Add("isMusicEnabled", Settings.isMusicEnabled);

        GameObjectSave.sceneData.Add(Settings.GameScene, sceneSave);
        return GameObjectSave;
    }


    public void ISaveableLoad(GameObjectSave gameObjectSave)
    {
        if (gameObjectSave.sceneData.TryGetValue(Settings.GameScene, out SceneSave sceneSave))
        {
            if (sceneSave.longDictionary != null)
            {
                if (sceneSave.longDictionary.TryGetValue("totalIteration", out long totalIteration))
                {
                    Settings.totalIteration = totalIteration;
                }
            }
            if (sceneSave.intDictionary != null)
            {
                if (sceneSave.intDictionary.TryGetValue("totalGuessedAmount", out int totalGuessedAmount))
                {
                    Settings.totalGuessedAmount = totalGuessedAmount;
                }
                if (sceneSave.intDictionary.TryGetValue("totalGuessedTexts", out int totalGuessedTexts))
                {
                    Settings.totalGuessedTexts = totalGuessedTexts;
                }
            }
            if (sceneSave.stringDictionary != null)
            {
                if (sceneSave.stringDictionary.TryGetValue("longestTextGuessed", out string longestTextGuessed))
                {
                    Settings.longestTextGuessed = longestTextGuessed;
                }
            }
            if (sceneSave.boolDictionary != null)
            {
                if (sceneSave.boolDictionary.TryGetValue("firstTime", out bool firstTime))
                {
                    Settings.firstTime = firstTime;
                }
                if (sceneSave.boolDictionary.TryGetValue("isMusicEnabled", out bool isMusicEnabled))
                {
                    Settings.isMusicEnabled = isMusicEnabled;
                }
            }
        }
    }
}
