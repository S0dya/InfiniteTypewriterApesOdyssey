using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static long totalIteration;
    public static long curTotalIteration;
    public static int totalGuessedAmount;
    public static int curTotalGuessedAmount;
    public static int totalGuessedTexts;
    public static string longestTextGuessed = " ";

    public static int curGuessedVal;
    public static int curGuessedAmount;

    public static string[] infoStringsHead = { "WELCOME", "COMPLETE", "ERROR", "STOPPED", };
    public static string[] infoStringsBody = { "Hey! Let me guess what you typied!", 
        "Hey! I guessed what you typed! isnt it: ", 
        "Hey! there is an error! try typing text with at least one english letter!", 
        "Hey! I only guessed: ",
    };

    public static bool firstTime = true;
    public static bool isMusicEnabled = true;

    public static string GameScene = "MainScene";
}
