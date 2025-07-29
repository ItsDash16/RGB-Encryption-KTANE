using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class RGBEncryption : MonoBehaviour {

    public KMBombInfo bomb;
    public KMAudio kmAudio;

    public KMSelectable[] Buttons;

    private string flashingMorse = "";
    private string flashingColors = "";

    private string charStringMorse = "";
    private string charStringColors = "";

    private string fullStringStep2 = "";

    private string replacedStringStep3 = "";
    private string convertedStringStep3 = "";

    private bool isCorrectButtonHZ = false;
    private bool isCorrectButtonTX = false;

    private int whenToPressButton;


    private char[,] TableStep1 =
    {
        {'R', 'B', 'G', 'R', 'G', 'f'},
        {'B', 'G', 'R', 'B', 'e', '.'},
        {'R', 'B', 'G', 'd', '.', '.'},
        {'G', 'R', 'c', '.', '.', '.'},
        {'B', 'b', '.', '.', '.', '.'},
        {'a', '.', '.', '.', '.', '.'}
    };

    // Led Colors
    public Material[] ledOptions;
    public Renderer led;

    // Button Colors
    public Renderer Button1;
    private int button1Index = 0;

    public Renderer Button2;
    private int button2Index = 0;

    public Renderer Button3;
    private int button3Index = 0;

    public Renderer Button4;
    private int button4Index = 0;

    public Renderer Button5;
    private int button5Index = 0;

    public Renderer Button6;
    private int button6Index = 0;

    public Renderer Button7;
    private int button7Index = 0;

    public Renderer Button8;
    private int button8Index = 0;

    public Renderer Button9;
    private int button9Index = 0;

    public Renderer Button10;
    private int button10Index = 0;

    private string inputtedSolution;

    // For Souvenir
    private string SouvenirMorseFlashes;
    private string SouvenirColorFlashes;

    //Logging
    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    void Awake() {
        ModuleId = ModuleIdCounter++;

        // Flash Module Lights
        StartCoroutine(FlashSequenceOnModule());

        foreach (KMSelectable button in Buttons) {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
        }


    }

    void Start() {
        // Module Maker
        PickLEDColorsAndFreq();

        // Module Solver
        Step1Solving();
        Step2Solving();
        Step3Solving();
        Step4Solving();
        SpecialNotes();

        // For Souvenir
        SouvenirMorseFlashes = flashingMorse;
        SouvenirColorFlashes = flashingColors;

        // Print Solution on Log
        if (isCorrectButtonHZ)
        {
            Debug.LogFormat("[RGB-Encryption #{0}] Final Solution: String = {1}, Button to press = HZ, When to press = {2}", ModuleId, convertedStringStep3, whenToPressButton);
        }
        else
        {
            Debug.LogFormat("[RGB-Encryption #{0}] Final Solution: String = {1}, Button to press = TX, When to press = {2}", ModuleId, convertedStringStep3, whenToPressButton);
        }

        


    }

    void buttonPress(KMSelectable button) {
        button.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (button == Buttons[10])
        {        
            if (isCorrectButtonHZ)
            {
                inputtedSolution = button1Index.ToString() + button2Index.ToString() + button3Index.ToString() + button4Index.ToString() + button5Index.ToString() + button6Index.ToString() + button7Index.ToString() + button8Index.ToString() + button9Index.ToString() + button10Index.ToString();
                if (inputtedSolution == convertedStringStep3)
                {
                    string currentBombTime = bomb.GetFormattedTime();
                    if (currentBombTime.Contains((char)(whenToPressButton + '0')))
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Module Solved.", ModuleId);
                        ModuleSolved = true;
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected button to be pressed at {1} but that number wasn't in the timer.", ModuleId, whenToPressButton);
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                }
                else
                {
                    Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected {1} and you submitted {2}.", ModuleId, convertedStringStep3, inputtedSolution);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected TX and you pressed HZ.", ModuleId);
                GetComponent<KMBombModule>().HandleStrike();
            }

        }
        else if (button == Buttons[11])
        {
            if (isCorrectButtonTX)
            {
                inputtedSolution = button1Index.ToString() + button2Index.ToString() + button3Index.ToString() + button4Index.ToString() + button5Index.ToString() + button6Index.ToString() + button7Index.ToString() + button8Index.ToString() + button9Index.ToString() + button10Index.ToString();
                if (inputtedSolution == convertedStringStep3)
                {
                    string currentBombTime = bomb.GetFormattedTime();
                    if (currentBombTime.Contains((char)(whenToPressButton + '0')))
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Module Solved.", ModuleId);
                        ModuleSolved = true;
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected button to be pressed at {1} but that number wasn't in the timer.", ModuleId, whenToPressButton);
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                }
                else
                {
                    Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected {1} and you submitted {2}.", ModuleId, convertedStringStep3, inputtedSolution);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected HZ and you pressed TX.", ModuleId);
                GetComponent<KMBombModule>().HandleStrike();
            }

        }

        // 0 - 1 Buttons
        else
        {
            if (button == Buttons[0])
            {
                if (button1Index == 0)
                {
                    button1Index = 1;
                    Button1.material = ledOptions[4];
                }
                else
                {
                    button1Index = 0;
                    Button1.material = ledOptions[0];
                }
            }
            else if (button == Buttons[1])
            {
                if (button2Index == 0)
                {
                    button2Index = 1;
                    Button2.material = ledOptions[4];
                }
                else
                {
                    button2Index = 0;
                    Button2.material = ledOptions[0];
                }
            }
            else if (button == Buttons[2])
            {
                if (button3Index == 0)
                {
                    button3Index = 1;
                    Button3.material = ledOptions[4];
                }
                else
                {
                    button3Index = 0;
                    Button3.material = ledOptions[0];
                }
            }
            else if (button == Buttons[3])
            {
                if (button4Index == 0)
                {
                    button4Index = 1;
                    Button4.material = ledOptions[4];
                }
                else
                {
                    button4Index = 0;
                    Button4.material = ledOptions[0];
                }
            }
            else if (button == Buttons[4])
            {
                if (button5Index == 0)
                {
                    button5Index = 1;
                    Button5.material = ledOptions[4];
                }
                else
                {
                    button5Index = 0;
                    Button5.material = ledOptions[0];
                }
            }
            else if (button == Buttons[5])
            {
                if (button6Index == 0)
                {
                    button6Index = 1;
                    Button6.material = ledOptions[4];
                }
                else
                {
                    button6Index = 0;
                    Button6.material = ledOptions[0];
                }
            }
            else if (button == Buttons[6])
            {
                if (button7Index == 0)
                {
                    button7Index = 1;
                    Button7.material = ledOptions[4];
                }
                else
                {
                    button7Index = 0;
                    Button7.material = ledOptions[0];
                }
            }
            else if (button == Buttons[7])
            {
                if (button8Index == 0)
                {
                    button8Index = 1;
                    Button8.material = ledOptions[4];
                }
                else
                {
                    button8Index = 0;
                    Button8.material = ledOptions[0];
                }
            }
            else if (button == Buttons[8])
            {
                if (button9Index == 0)
                {
                    button9Index = 1;
                    Button9.material = ledOptions[4];
                }
                else
                {
                    button9Index = 0;
                    Button9.material = ledOptions[0];
                }
            }
            else if (button == Buttons[9])
            {
                if (button10Index == 0)
                {
                    button10Index = 1;
                    Button10.material = ledOptions[4];
                }
                else
                {
                    button10Index = 0;
                    Button10.material = ledOptions[0];
                }
            }
        }

    }

    void Step1Solving()
    {
        bool morseRunning = true;
        bool colorsRunning = true;
        int indexMorse = 0;
        int indexColor = 0;
        int down = 0;
        int right = 0;

        // Morse String
        while (morseRunning)
        {
            if (indexMorse == 29)
            {
                charStringMorse = charStringMorse + TableStep1[down, right];
                morseRunning = false;
                charStringMorse = charStringMorse.ToUpper();
            }
            else if (flashingMorse[indexMorse] == '.')
            {
                down++;
            }
            else if (flashingMorse[indexMorse] == '-')
            {
                right++;
            }
            else
            {
                charStringMorse = charStringMorse + TableStep1[down, right];
                down = 0;
                right = 0;
            }
            indexMorse++;
        }

        down = 0;
        right = 0;

        //Color String
        while (colorsRunning)
        {
            if (indexColor == 29)
            {
                charStringColors = charStringColors + TableStep1[down, right];
                colorsRunning = false;
                charStringColors = charStringColors.ToUpper();
            }
            else if (flashingColors[indexColor] == TableStep1[down, right])
            {
                right++;
            }
            else
            {
                if (flashingColors[indexColor] == ' ')
                {
                    charStringColors = charStringColors + TableStep1[down, right];
                    down = 0;
                    right = 0;
                }
                else
                {
                    down++;
                }
            }
            indexColor++;
        }
        Debug.LogFormat("[RGB-Encryption #{0}] (Step 1) Morse Code String: {1}", ModuleId, charStringMorse);
        Debug.LogFormat("[RGB-Encryption #{0}] (Step 1) Colors String: {1}", ModuleId, charStringColors);
    }

    void Step2Solving()
    {
        int A = flashingMorse.Count(x => x == '-');
        int B = flashingColors.Count(x => x == 'B');
        Debug.LogFormat("[RGB-Encryption #{0}] (Step 2) A = {1}, B = {2}", ModuleId, A, B);

        if (A > B)
        {
            fullStringStep2 = charStringMorse + charStringColors;
            Debug.LogFormat("[RGB-Encryption #{0}] (Step 2) A > B: {1}", ModuleId, fullStringStep2);
        }
        else if (A < B)
        {
            fullStringStep2 = charStringColors + charStringMorse;
            Debug.LogFormat("[RGB-Encryption #{0}] (Step 2) A < B: {1}", ModuleId, fullStringStep2);
        }
        else if (A == B)
        {
            fullStringStep2 = charStringMorse + charStringMorse;
            Debug.LogFormat("[RGB-Encryption #{0}] (Step 2) A = B: {1}", ModuleId, fullStringStep2);
        }
    }

    void Step3Solving()
    {
        // Characters
        IEnumerable<int> serialNumbers = bomb.GetSerialNumberNumbers();
        List<int> e = serialNumbers.ToList<int>();
        char a = bomb.GetSerialNumber()[5];

        int A = (int)char.GetNumericValue(a);
        int B = 1 + ((bomb.GetModuleIDs().Count() - 1) % 9);
        int C;
        int D;
        int E = e[0];
        int F;


        if (bomb.GetPortCount() == 0)
        {
            C = bomb.GetPortCount();
        }
        else
        {
            C = 1 + ((bomb.GetPortCount() - 1) % 9);
        }

        if (bomb.GetBatteryHolderCount() == 0)
        {
            D = bomb.GetBatteryHolderCount();
        }
        else
        {
            D = 1 + ((bomb.GetBatteryHolderCount() - 1) % 9);
        }

        if (bomb.GetIndicators().Count<string>() == 0)
        {
            F = bomb.GetIndicators().Count<string>();
        }
        else
        {
            F = 1 + ((bomb.GetIndicators().Count<string>() - 1) % 9);
        }
        Debug.LogFormat("[RGB-Encryption #{0}] (Step 3) Numbers from table: A = {1}, B = {2}, C = {3}, D = {4}, E = {5}, F = {6}", ModuleId, A, B, C, D, E, F);
        
        // Replace to string
        for (int i = 0; i <= 9; i++)
        {
            if (fullStringStep2[i] == 'A')
            {
                replacedStringStep3 = replacedStringStep3 + A;
            }
            else if (fullStringStep2[i] == 'B')
            {
                replacedStringStep3 = replacedStringStep3 + B;
            }
            else if (fullStringStep2[i] == 'C')
            {
                replacedStringStep3 = replacedStringStep3 + C;
            }
            else if (fullStringStep2[i] == 'D')
            {
                replacedStringStep3 = replacedStringStep3 + D;
            }
            else if (fullStringStep2[i] == 'E')
            {
                replacedStringStep3 = replacedStringStep3 + E;
            }
            else if (fullStringStep2[i] == 'F')
            {
                replacedStringStep3 = replacedStringStep3 + F;
            }
        }
        Debug.LogFormat("[RGB-Encryption #{0}] (Step 3) Replaced String: {1}", ModuleId, replacedStringStep3);

        // Convert String
        for (int i = 0; i <= 9; i++)
        {
            if (Convert.ToInt32(replacedStringStep3[i]) % 2 == 0)
            {
                convertedStringStep3 = convertedStringStep3 + 1;
            }
            else
            {
                convertedStringStep3 = convertedStringStep3 + 0;
            }
        }
        Debug.LogFormat("[RGB-Encryption #{0}] (Step 3) Solution: {1}", ModuleId, convertedStringStep3);

    }

    void Step4Solving()
    {
        char serialNumberLastCh = bomb.GetSerialNumber()[5];
        int LastSNChar = (int)char.GetNumericValue(serialNumberLastCh);
        int DigitIndex;

        if (LastSNChar == 0)
        {
            DigitIndex = 10;
        }
        else
        {
            DigitIndex = LastSNChar;
        }
        DigitIndex--;
        char SequenceDigit = convertedStringStep3[DigitIndex];
        if (SequenceDigit == '0')
        {
            isCorrectButtonHZ = true;
            Debug.LogFormat("[RGB-Encryption #{0}] (Step 4) Digit {1} from sequence is a 0. Correct button: HZ", ModuleId, DigitIndex + 1);   
        }
        else
        {
            isCorrectButtonTX = true;
            Debug.LogFormat("[RGB-Encryption #{0}] (Step 4) Digit {1} from sequence is a 1. Correct button: TX", ModuleId, DigitIndex + 1);
        }
        int TotalOnes = convertedStringStep3.Count(ch => ch == '1');
        whenToPressButton = 1 + (((TotalOnes * LastSNChar) - 1) % 9);
        Debug.LogFormat("[RGB-Encryption #{0}] (Step 4) Press button when timer has a {1}.", ModuleId, whenToPressButton);
    }

    void SpecialNotes()
    {
        // Note 1
        if (flashingMorse[6] == '.' && flashingMorse[7] == '-' && flashingMorse[8] == '.' && flashingMorse[9] == '-' && flashingMorse[10] == '.')
        {
            if (isCorrectButtonHZ)
            {
                isCorrectButtonHZ = false;
                isCorrectButtonTX = true;
                Debug.LogFormat("[RGB-Encryption #{0}] Special Note 1 is true. New button: TX", ModuleId);
            }
            else
            {
                isCorrectButtonHZ = true;
                isCorrectButtonTX = false;
                Debug.LogFormat("[RGB-Encryption #{0}] Special Note 1 is true. New button: HZ", ModuleId);
            }
        }

        // Note 2
        string backupStringSolution = convertedStringStep3;
        if (flashingColors[17] == 'R' && flashingColors[18] == 'G' && flashingColors[19] == 'B' && flashingColors[20] == 'R' && flashingColors[21] == 'B')
        {
            convertedStringStep3 = "";
            for (int i = 0; i <= 9; i++)
            {
                if (backupStringSolution[i] == '0')
                {
                    convertedStringStep3 = convertedStringStep3 + 1;
                }
                else
                {
                    convertedStringStep3 = convertedStringStep3 + 0;
                }
            }
            Debug.LogFormat("[RGB-Encryption #{0}] Special Note 2 is true. New solution: {1}", ModuleId, convertedStringStep3);
        }
    }


    void PickLEDColorsAndFreq()
    {

        // Colors
        for (int i = 0; i <= 28; i++)
        {
            if (i == 5 || i == 11 || i == 17 || i == 23)
            {
                flashingColors = flashingColors + " ";
            }
            else
            {
                int RandomColor = UnityEngine.Random.Range(0, 3);
                switch (RandomColor)
                {
                    case 0:
                        flashingColors = flashingColors + "R";
                        break;
                    case 1:
                        flashingColors = flashingColors + "G";
                        break;
                    case 2:
                        flashingColors = flashingColors + "B";
                        break;

                }
            }
        }
        Debug.LogFormat("[RGB-Encryption #{0}] Colors Flashing: {1}", ModuleId, flashingColors);

        // Morse
        for (int i = 0; i <= 28; i++)
        {
            if (i == 5 || i == 11 || i == 17 || i == 23)
            {
                flashingMorse = flashingMorse + " ";
            }
            else
            {
                int RandomMorse = UnityEngine.Random.Range(0, 2);
                switch (RandomMorse)
                {
                    case 0:
                        flashingMorse = flashingMorse + ".";
                        break;
                    case 1:
                        flashingMorse = flashingMorse + "-";
                        break;
                }
            }
        }
        Debug.LogFormat("[RGB-Encryption #{0}] Morse Flashing: {1}", ModuleId, flashingMorse);

    }

    IEnumerator FlashSequenceOnModule()
    {
        while (bomb.GetTime() <= 0)
        {
            yield return null;
        }            

        while (!ModuleSolved)
        {
            for (int i = 0; i < flashingMorse.Length; i++)
            {
                // If dot
                if (flashingMorse[i] == '.')
                {
                    if (ModuleSolved)
                    {
                        break;
                    }
                    else if (flashingColors[i] == 'R')
                    {
                        led.material = ledOptions[1];
                        yield return new WaitForSeconds(0.25f);
                        led.material = ledOptions[0];
                        yield return new WaitForSeconds(0.25f);
                    }
                    else if (flashingColors[i] == 'G')
                    {
                        led.material = ledOptions[2];
                        yield return new WaitForSeconds(0.25f);
                        led.material = ledOptions[0];
                        yield return new WaitForSeconds(0.25f);
                    }
                    else if (flashingColors[i] == 'B')
                    {
                        led.material = ledOptions[3];
                        yield return new WaitForSeconds(0.25f);
                        led.material = ledOptions[0];
                        yield return new WaitForSeconds(0.25f);
                    }
                }
                // If dash
                if (flashingMorse[i] == '-')
                {
                    if (ModuleSolved)
                    {
                        break;
                    }
                    if (flashingColors[i] == 'R')
                    {
                        led.material = ledOptions[1];
                        yield return new WaitForSeconds(0.75f);
                        led.material = ledOptions[0];
                        yield return new WaitForSeconds(0.25f);
                    }
                    else if (flashingColors[i] == 'G')
                    {
                        led.material = ledOptions[2];
                        yield return new WaitForSeconds(0.75f);
                        led.material = ledOptions[0];
                        yield return new WaitForSeconds(0.25f);
                    }
                    else if (flashingColors[i] == 'B')
                    {
                        led.material = ledOptions[3];
                        yield return new WaitForSeconds(0.75f);
                        led.material = ledOptions[0];
                        yield return new WaitForSeconds(0.25f);
                    }
                }

                // If blank
                if (flashingMorse[i] == ' ')
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
            yield return new WaitForSeconds(2.75f);
        }
    }

   void Update () {

   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"!{0} submit <binary> <button> <whenToPress>";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();

        // I want to thank ChatGPT for all this code. I love you, ChatGPT.
        Match match = Regex.Match(command, @"^submit\s+([01]{10})\s+(hz|tx)\s+([0-9])$");

        if (!match.Success)
            yield break;

        string binaryInput = match.Groups[1].Value;
        string targetButton = match.Groups[2].Value;
        int targetDigit = int.Parse(match.Groups[3].Value);

        // Input Binary
        for (int i = 0; i <= 9; i++)
        {
            if (binaryInput[i] == '1')
            {
                if (i == 0)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button1Index = 1;
                    Button1.material = ledOptions[4];
                }
                else if (i == 1)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button2Index = 1;
                    Button2.material = ledOptions[4];
                }
                else if (i == 2)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button3Index = 1;
                    Button3.material = ledOptions[4];
                }
                else if (i == 3)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button4Index = 1;
                    Button4.material = ledOptions[4];
                }
                else if (i == 4)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button5Index = 1;
                    Button5.material = ledOptions[4];
                }
                else if (i == 5)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button6Index = 1;
                    Button6.material = ledOptions[4];
                }
                else if (i == 6)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button7Index = 1;
                    Button7.material = ledOptions[4];
                }
                else if (i == 7)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button8Index = 1;
                    Button8.material = ledOptions[4];
                }
                else if (i == 8)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button9Index = 1;
                    Button9.material = ledOptions[4];
                }
                else if (i == 9)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button10Index = 1;
                    Button10.material = ledOptions[4];
                }
            }
            else if (binaryInput[i] == '0')
            {
                if (i == 0)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button1Index = 0;
                    Button1.material = ledOptions[0];
                }
                else if (i == 1)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button2Index = 0;
                    Button2.material = ledOptions[0];
                }
                else if (i == 2)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button3Index = 0;
                    Button3.material = ledOptions[0];
                }
                else if (i == 3)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button4Index = 0;
                    Button4.material = ledOptions[0];
                }
                else if (i == 4)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button5Index = 0;
                    Button5.material = ledOptions[0];
                }
                else if (i == 5)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button6Index = 0;
                    Button6.material = ledOptions[0];
                }
                else if (i == 6)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button7Index = 0;
                    Button7.material = ledOptions[0];
                }
                else if (i == 7)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button8Index = 0;
                    Button8.material = ledOptions[0];
                }
                else if (i == 8)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button9Index = 0;
                    Button9.material = ledOptions[0];
                }
                else if (i == 9)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button10Index = 0;
                    Button10.material = ledOptions[0];
                }
            }

        }

        // Wait for timer
        while (!bomb.GetFormattedTime().Contains((char)(targetDigit + '0')))
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;

        // Press Button
        if (targetButton == "hz" || targetButton == "HZ")
        {
            if (isCorrectButtonHZ)
            {
                inputtedSolution = button1Index.ToString() + button2Index.ToString() + button3Index.ToString() + button4Index.ToString() + button5Index.ToString() + button6Index.ToString() + button7Index.ToString() + button8Index.ToString() + button9Index.ToString() + button10Index.ToString();
                if (inputtedSolution == convertedStringStep3)
                {
                    string currentBombTime = bomb.GetFormattedTime();
                    if (currentBombTime.Contains((char)(whenToPressButton + '0')))
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Module Solved.", ModuleId);
                        ModuleSolved = true;
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected button to be pressed at {1} but that number wasn't in the timer.", ModuleId, whenToPressButton);
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                }
                else
                {
                    Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected {1} and you submitted {2}.", ModuleId, convertedStringStep3, inputtedSolution);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected TX and you pressed HZ.", ModuleId);
                GetComponent<KMBombModule>().HandleStrike();
            }

        }
        else if (targetButton == "tx" || targetButton == "TX")
        {
            if (isCorrectButtonTX)
            {
                inputtedSolution = button1Index.ToString() + button2Index.ToString() + button3Index.ToString() + button4Index.ToString() + button5Index.ToString() + button6Index.ToString() + button7Index.ToString() + button8Index.ToString() + button9Index.ToString() + button10Index.ToString();
                if (inputtedSolution == convertedStringStep3)
                {
                    string currentBombTime = bomb.GetFormattedTime();
                    if (currentBombTime.Contains((char)(whenToPressButton + '0')))
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Module Solved.", ModuleId);
                        ModuleSolved = true;
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected button to be pressed at {1} but that number wasn't in the timer.", ModuleId, whenToPressButton);
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                }
                else
                {
                    Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected {1} and you submitted {2}.", ModuleId, convertedStringStep3, inputtedSolution);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected HZ and you pressed TX.", ModuleId);
                GetComponent<KMBombModule>().HandleStrike();
            }

        }
    }

    IEnumerator TwitchHandleForcedSolve ()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (convertedStringStep3[i] == '1')
            {
                if (i == 0)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button1Index = 1;
                    Button1.material = ledOptions[4];
                }
                else if (i == 1)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button2Index = 1;
                    Button2.material = ledOptions[4];
                }
                else if (i == 2)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button3Index = 1;
                    Button3.material = ledOptions[4];
                }
                else if (i == 3)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button4Index = 1;
                    Button4.material = ledOptions[4];
                }
                else if (i == 4)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button5Index = 1;
                    Button5.material = ledOptions[4];
                }
                else if (i == 5)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button6Index = 1;
                    Button6.material = ledOptions[4];
                }
                else if (i == 6)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button7Index = 1;
                    Button7.material = ledOptions[4];
                }
                else if (i == 7)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button8Index = 1;
                    Button8.material = ledOptions[4];
                }
                else if (i == 8)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button9Index = 1;
                    Button9.material = ledOptions[4];
                }
                else if (i == 9)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button10Index = 1;
                    Button10.material = ledOptions[4];
                }
            }
            else if (convertedStringStep3[i] == '0')
            {
                if (i == 0)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button1Index = 0;
                    Button1.material = ledOptions[0];
                }
                else if (i == 1)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button2Index = 0;
                    Button2.material = ledOptions[0];
                }
                else if (i == 2)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button3Index = 0;
                    Button3.material = ledOptions[0];
                }
                else if (i == 3)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button4Index = 0;
                    Button4.material = ledOptions[0];
                }
                else if (i == 4)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button5Index = 0;
                    Button5.material = ledOptions[0];
                }
                else if (i == 5)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button6Index = 0;
                    Button6.material = ledOptions[0];
                }
                else if (i == 6)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button7Index = 0;
                    Button7.material = ledOptions[0];
                }
                else if (i == 7)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button8Index = 0;
                    Button8.material = ledOptions[0];
                }
                else if (i == 8)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button9Index = 0;
                    Button9.material = ledOptions[0];
                }
                else if (i == 9)
                {
                    yield return null;
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                    button10Index = 0;
                    Button10.material = ledOptions[0];
                }
            }

        }

        // Wait for timer
        while (!bomb.GetFormattedTime().Contains((char)(whenToPressButton + '0')))
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;

        // Press Button
        if (isCorrectButtonHZ)
        {
            if (isCorrectButtonHZ)
            {
                inputtedSolution = button1Index.ToString() + button2Index.ToString() + button3Index.ToString() + button4Index.ToString() + button5Index.ToString() + button6Index.ToString() + button7Index.ToString() + button8Index.ToString() + button9Index.ToString() + button10Index.ToString();
                if (inputtedSolution == convertedStringStep3)
                {
                    string currentBombTime = bomb.GetFormattedTime();
                    if (currentBombTime.Contains((char)(whenToPressButton + '0')))
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Module Solved.", ModuleId);
                        ModuleSolved = true;
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected button to be pressed at {1} but that number wasn't in the timer.", ModuleId, whenToPressButton);
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                }
                else
                {
                    Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected {1} and you submitted {2}.", ModuleId, convertedStringStep3, inputtedSolution);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected TX and you pressed HZ.", ModuleId);
                GetComponent<KMBombModule>().HandleStrike();
            }

        }
        else if (isCorrectButtonTX)
        {
            if (isCorrectButtonTX)
            {
                inputtedSolution = button1Index.ToString() + button2Index.ToString() + button3Index.ToString() + button4Index.ToString() + button5Index.ToString() + button6Index.ToString() + button7Index.ToString() + button8Index.ToString() + button9Index.ToString() + button10Index.ToString();
                if (inputtedSolution == convertedStringStep3)
                {
                    string currentBombTime = bomb.GetFormattedTime();
                    if (currentBombTime.Contains((char)(whenToPressButton + '0')))
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Module Solved.", ModuleId);
                        ModuleSolved = true;
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected button to be pressed at {1} but that number wasn't in the timer.", ModuleId, whenToPressButton);
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                }
                else
                {
                    Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected {1} and you submitted {2}.", ModuleId, convertedStringStep3, inputtedSolution);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                Debug.LogFormat("[RGB-Encryption #{0}] Strike! Expected HZ and you pressed TX.", ModuleId);
                GetComponent<KMBombModule>().HandleStrike();
            }

        }
    }
}