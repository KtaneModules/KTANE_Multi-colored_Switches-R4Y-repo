using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;



public class Switch
{
    public char ColorSwitch;
    public char ColorSocket;
    public bool State;
    public bool R;
    public bool G;
    public bool B;
    public bool R_Socket;
    public bool G_Socket;
    public bool B_Socket;
    public GameObject Socket;
    public GameObject SwitchModel;
    public void RandomState()
    {
        int state = Random.Range(0, 2);
        switch (state)
        {
            case 0:
                this.State = false;
                this.SwitchModel.gameObject.transform.localEulerAngles = new Vector3(-55f, 0, 0);
                break;
            case 1:
                this.State = true;
                this.SwitchModel.gameObject.transform.localEulerAngles = new Vector3(55f, 0, 0);
                break;
        }
    }
}
public struct LED
{
    public char CharColor1;
    public char CharColor2;
    public bool R1;
    public bool G1;
    public bool B1;
    public bool R2;
    public bool G2;
    public bool B2;
    public GameObject LEDModel;
    public Material Color1;
    public Material Color2;
}
public class MultiColoredSwitches : MonoBehaviour
{
    public Material[] SwitchesAndSocketsColors;
    public Material[] LEDColors;
    private Switch[] Switches = new Switch[5];
    private LED[] LEDsUp = new LED[5];
    private LED[] LEDsDown = new LED[5];
    private string LEDsUpLog;
    private string LEDsDownLog;
    public GameObject LEDCycleChecker;
    public GameObject[] SwitchModels;
    public GameObject[] SocketModels;
    public GameObject[] LEDModelsUP;
    public GameObject[] LEDModelsDOWN;
    private Material[] LEDRow1Cycle1 = new Material[5];
    private Material[] LEDRow1Cycle2 = new Material[5];
    private Material[] LEDRow2Cycle1 = new Material[5];
    private Material[] LEDRow2Cycle2 = new Material[5];
    public KMBombModule module;
    private string LoggingSwitchesStates;
    private bool[] SwitchesStates = new bool[5];
    private bool[] SubmissionState = new bool[5];
    private bool[] RemovedState = new bool[5];
    private bool[] ExpectedSwitchesStates = new bool[5];
    string logstuff;
    static int ModuleIDCounter;
    int[] SwitchesNumbers = new int[3];
    int[] SocketNumbers = new int[3];
    int MinSwitch;
    int MaxSwitch;
    int MinSocket;
    int MaxSocket;
    int ChosenSet;
    int SolutionIndex;
    int RemovedIndex;
    int[] AllStatesNumbers = new int[12]; 
    bool[][] SwitchesColorStates = new bool[3][] { new bool[5], new bool[5], new bool[5]};
    bool[][] SocketsColorStates = new bool[3][] { new bool[5], new bool[5], new bool[5]};
    bool[][] AllStates = new bool[12][];
    bool Solved;
    bool cycle1;
    int moduleID;
    // Use this for initialization
    void Start()
    {
        moduleID = ModuleIDCounter++;
        string[] order = { " first flash of the first row ", " second flash of the first row ", " first flash of the second row ", " second flash of the second row " };
        string[] color = { " red ", " green ", " blue " };
        string[] thing = { "switches", "sockets" };
        NewLEDS();
        PickColorSwitchesAndSockets();
        FindLEDBoolsColors(LEDsUp);
        FindLEDBoolsColors(LEDsDown);
        Debug.LogFormat("[Multicolored Switches #{0}] The LED states from top to bottom and in red, green, blue order should be:", moduleID);
        while (CheckIfArrayHasSameValueInSelf(AllStates))
        {
            NewLEDS();
            FindLEDBoolsColors(LEDsUp);
            FindLEDBoolsColors(LEDsDown);
        }
        while (SwitchesSameAsAllStates(SwitchesStates))
        {
            for(int i = 0; i < 5; i++)
            {
                Switches[i].RandomState();
                SwitchesStates[i] = Switches[i].State;
            }
        }

        for (int i = 0; i < 12; i++)
        {
            logstuff = "The" + color[i % 3] +"set of the"+ order[i / 3] + "of LEDs should be";
            Debug.LogFormat("[Multicolored Switches #{2}] {0},{1}", logstuff, TurnBitsToString(AllStates[i]), moduleID);
        }
        StartCoroutine(FlickerLEDS());
        Debug.LogFormat("[Multicolored Switches #{1}] The initial state of the switches is : {0}", TurnBitsToString(SwitchesStates), moduleID);
        for(int i = 0; i < 5; i++)
        {
            SwitchesColorStates[0][i] = Switches[i].R;
            SwitchesColorStates[1][i] = Switches[i].G;
            SwitchesColorStates[2][i] = Switches[i].B;
            SocketsColorStates[0][i] = Switches[i].R_Socket;
            SocketsColorStates[1][i] = Switches[i].G_Socket;
            SocketsColorStates[2][i] = Switches[i].B_Socket;
        }
        for(int i = 0; i < 3; i++)
        {
            SwitchesNumbers[i] = CountNumberState(SwitchesColorStates[i]);
            Debug.LogFormat("[Multicolored Switches #{3}] The number of{0}coloring in the {1} is {2}", color[i%3],thing[0],SwitchesNumbers[i],moduleID);
        }
        for (int i = 0; i < 3; i++)
        {
            SocketNumbers[i] = CountNumberState(SocketsColorStates[i]);
            Debug.LogFormat("[Multicolored Switches #{3}] The number of{0}coloring in the {1} is {2}", color[i % 3], thing[1], SocketNumbers[i],moduleID);
        }
        MinSwitch = SwitchesNumbers.Min();
        MaxSwitch = SwitchesNumbers.Max();
        MinSocket = SocketNumbers.Min();
        MaxSocket = SocketNumbers.Max();
        Debug.LogFormat("[Multicolored Switches #{3}] The chosen set with the least number of {0} for the {1} is the{2}set", MinSwitch, thing[0], color[IndexMin(SwitchesNumbers)],moduleID);
        Debug.LogFormat("[Multicolored Switches #{3}] The chosen set with the most number of {0} for the {1} is the{2}set", MaxSwitch, thing[0], color[IndexMax(SwitchesNumbers)],moduleID);
        Debug.LogFormat("[Multicolored Switches #{3}] The chosen set with the least number of {0} for the {1} is the{2}set", MinSocket, thing[1], color[IndexMin(SocketNumbers)],moduleID);
        Debug.LogFormat("[Multicolored Switches #{3}] The chosen set with the most number of {0} for the {1} is the{2}set", MaxSocket, thing[1], color[IndexMax(SocketNumbers)],moduleID);
        for (int i = 0; i < 12; i++)
        {
            AllStatesNumbers[i] = CountNumberState(AllStates[i]);
        }
        Debug.LogFormat("[Multicolored Switches #{1}] The chosen minimum set is set number: {0}", SwitchReasoningMin()+1,moduleID);
        Debug.LogFormat("[Multicolored Switches #{1}] The chosen maximum set is set number: {0}", SwitchReasoningMax()+1,moduleID);
        string parity;
        if((CountNumberState(AllStates[SwitchReasoningMin()]) % 2)== (CountNumberState(AllStates[SwitchReasoningMax()]) % 2))
        {
            parity = "share";
            ChosenSet = SwitchReasoningMin();
        }
        else
        {
            parity = "not share";
            ChosenSet = SwitchReasoningMax();
        }
        Debug.LogFormat("[Multicolored Switches #{2}] The sets does {0} parity, so the chosen set is set number {1}", parity,ChosenSet+1,moduleID);
        SocketReasoning();
        Debug.LogFormat("[Multicolored Switches #{2}] The submission state is state {0} which is {1}", SolutionIndex + 1 , TurnBitsToString(SubmissionState), moduleID);
        Debug.LogFormat("[Multicolored Switches #{2}] The removed state is state {0} which is {1}", RemovedIndex + 1, TurnBitsToString(RemovedState), moduleID);
        for (int i = 0; i < 5; i++)
        {
            int j = i;
            Switches[j].SwitchModel.GetComponent<KMSelectable>().OnInteract += delegate() { CheckSwitchFlip(j); GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Switches[j].SwitchModel.transform); Switches[j].SwitchModel.GetComponent<KMSelectable>().AddInteractionPunch(.25f); return false; };
            ExpectedSwitchesStates[j] = Switches[j].State;
        }
    }
    StringBuilder SwitchColorsLog()
    {
        StringBuilder result = new StringBuilder("00000",5);
        for (int i = 0; i < 5; i++)
        {
            result[i] = Switches[i].ColorSwitch;
        }
        return result;
    }
    StringBuilder SocketColorsLog()
    {
        StringBuilder result = new StringBuilder("00000",5);
        for (int i = 0; i < 5; i++)
        {
            result[i] = Switches[i].ColorSocket;
        }
        return result;
    }
    void SocketReasoning()
    {
        int solutionindex = new int();
        int removedindex = new int();
        int group = ChosenSet / 3;
        int color = ChosenSet % 3;
        if (IndexMin(SocketNumbers) == IndexMax(SocketNumbers))
        {
            switch (color)
            {
                case 0:
                    solutionindex = group + 1;
                    removedindex = group + 2;
                    break;
                case 1:
                    solutionindex = group + 0;
                    removedindex = group + 2;
                    break;
                case 2:
                    solutionindex = group + 0;
                    removedindex = group + 1;
                    break;
            }
        }
        else
        {
            solutionindex = IndexMin(SocketNumbers);
            removedindex = IndexMax(SocketNumbers);
        }
        SolutionIndex = solutionindex;
        RemovedIndex = removedindex;
        SubmissionState = AllStates[solutionindex];
        RemovedState = AllStates[removedindex];
    }
    int SwitchReasoningMin()
    {
        int color_min = IndexMin(SwitchesNumbers);
        int chosen_min_index;
        int[] chosen_rows_values= new int[4];
        int[] chosen_rows_indexes = new int[4];
        for (int i = 0; i < 4; i++)
        {
            chosen_rows_values[i] = AllStatesNumbers[color_min + (i * 3)];
            chosen_rows_indexes[i] = color_min + (i * 3);
        }
        chosen_min_index = chosen_rows_indexes[IndexMin(chosen_rows_values)];
        
        return chosen_min_index;
    }
    int SwitchReasoningMax()
    {
        int color_max = IndexMax(SwitchesNumbers);
        int chosen_max_index;
        int[] chosen_rows_values_max = new int[4];
        int[] chosen_rows_indexes_max = new int[4];
        for (int i = 0; i < 4; i++)
        {
            chosen_rows_values_max[i] = AllStatesNumbers[color_max + (i * 3)];
            chosen_rows_indexes_max[i] = color_max + (i * 3);
        }
        chosen_max_index = chosen_rows_indexes_max[IndexMax(chosen_rows_values_max)];

        return chosen_max_index;
    }
    int IndexMin(int[] arrayint)
    {
        int intmin = arrayint[0];
        int indexmin = 0;
        for(int i = 0; i < arrayint.Length; i++)
        {
            if (arrayint[i] < intmin)
            {
                intmin = arrayint[i];
                indexmin = i;
            }
        }
        return indexmin;
    }
    int IndexMax(int[] arrayint)
    {
        int intmax = arrayint[0];
        int indexmax = 0;
        for (int i = 0; i < arrayint.Length; i++)
        {
            if (arrayint[i] > intmax)
            {
                intmax = arrayint[i];
                indexmax = i;
            }
        }
        return indexmax;
    }
    int CountNumberState(bool[] state)
    {
        int number = 0;
        for(int i = 0; i < 5; i++)
        {
            if (state[i])
            {
                number++;
            }
        }
        return number;
    }

    private float easeOutSine(float time, float duration, float from, float to)
    {
        return (to - from) * Mathf.Sin(time / duration * (Mathf.PI / 2)) + from;
    }
    void CheckSwitchFlip(int selectedswitch)
    {
        for(int i=0;i<5;i++)
            ExpectedSwitchesStates[i] = SwitchesStates[i];
        ExpectedSwitchesStates[selectedswitch] = !Switches[selectedswitch].State;
        if (CheckIfArraysSame(ExpectedSwitchesStates,SubmissionState))
        {
            Switches[selectedswitch].State = ExpectedSwitchesStates[selectedswitch];
            for (int i = 0; i < 5; i++)
                SwitchesStates[i] = ExpectedSwitchesStates[i];
            StartCoroutine(FlipSwitch(selectedswitch));
            Solved = true;
            module.HandlePass();
            Debug.LogFormat("[Multicolored Switches #{0}] Congratulations! You solved the module!", moduleID);
        }
        if (CheckIfArraysSame(ExpectedSwitchesStates,RemovedState))
        {
            Switches[selectedswitch].State = ExpectedSwitchesStates[selectedswitch];
            for (int i = 0; i < 5; i++)
                SwitchesStates[i] = ExpectedSwitchesStates[i];
            StartCoroutine(FlipSwitch(selectedswitch));
            return;
        }
        else if (SwitchesSameAsAllStates(ExpectedSwitchesStates)&&!Solved)
        {
            module.HandleStrike();
          Debug.LogFormat("[Multicolored Switches #{3}] Strike! You flipped switch {0} when the state was {1}, which will make the switches states {2}", selectedswitch,TurnBitsToString(SwitchesStates),TurnBitsToString(ExpectedSwitchesStates),moduleID);
          Debug.LogFormat("[Multicolored Switches #{0}] Which is one of the forbidden states.", moduleID);
        }
        else
        {
            Switches[selectedswitch].State = ExpectedSwitchesStates[selectedswitch];
            for (int i = 0; i < 5; i++)
                SwitchesStates[i] = ExpectedSwitchesStates[i];
            StartCoroutine(FlipSwitch(selectedswitch));
        }
    }
    string LEDLetters(LED[] array,int cycle)
    {
        string led = "";
        if (cycle == 1)
        {
            for (int i = 0; i < 5; i++)
            {
            }
        }
        return led;
    }
    IEnumerator FlipSwitch(int selected)
    {
        const float duration = .3f;
        var startTime = Time.fixedTime;
        if (Switches[selected].State)
        {
            do
            {
                Switches[selected].SwitchModel.transform.localEulerAngles = new Vector3(easeOutSine(Time.fixedTime - startTime, duration, -55f, 55f), 0, 0);
                yield return null;
            }
            while (Time.fixedTime < startTime + duration);
            Switches[selected].SwitchModel.transform.localEulerAngles = new Vector3(55f, 0, 0);
        }
        else if (!Switches[selected].State)
        {
            do
            {
                Switches[selected].SwitchModel.transform.localEulerAngles = new Vector3(easeOutSine(Time.fixedTime - startTime, duration, 55f, -55f), 0, 0);
                yield return null;
            }
            while (Time.fixedTime < startTime + duration);
            Switches[selected].SwitchModel.transform.localEulerAngles = new Vector3(-55f, 0, 0);
        }
    }
    bool SwitchesSameAsAllStates(bool[] switchstate)
    {
        for(int i = 0; i < 12; i++)
        {
            if (CheckIfArraysSame(switchstate, AllStates[i]))
            {
                return true;
            }
        }
        return false;
    }
    bool CheckIfArrayHasSameValueInSelf(bool[][] bigone)
    {
        for(int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 12; j++)
            {
                if (i == j)
                {
                    continue;
                }
                else
                {
                    if (!CheckIfArraysSame(bigone[i], bigone[j]))
                    {
                        continue;
                    }
                    else
                    {
                        return true;
                    }
                }
                
            }
        }
        return false;
    }
    bool CheckIfArraysSame(bool[] bool1, bool[] bool2)
    {
        for (int i = 0; i < 5; i++)
        {
            if (bool1[i] == bool2[i])
            {
                
            }
            else
            {
                return false;
            }
        }
        return true;
    }

        string TurnBitsToString(bool[] StateArray)
    {
        string states = "";

        for (int i = 0; i < 5; i++)
        {
            bool state = StateArray[i];
            if (state)
            {
                states = states.Insert(i, "▲");
            }
            else if (!state)
            {
                states = states.Insert(i, "▼");
            }
        }
        return states;
    }
    IEnumerator FlickerLEDS()
    {
        while (!Solved)
        {
            if (cycle1)
            {
                for (int i = 0; i < 5; i++)
                {
                    LEDsUp[i].LEDModel.GetComponent<MeshRenderer>().material = LEDRow1Cycle2[i];
                }
                for (int i = 0; i < 5; i++)
                {
                    LEDsDown[i].LEDModel.GetComponent<MeshRenderer>().material = LEDRow2Cycle2[i];
                }
                LEDCycleChecker.GetComponent<MeshRenderer>().material = LEDColors[7];
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    LEDsUp[i].LEDModel.GetComponent<MeshRenderer>().material = LEDRow1Cycle1[i];
                }
                for (int i = 0; i < 5; i++)
                {
                    LEDsDown[i].LEDModel.GetComponent<MeshRenderer>().material = LEDRow2Cycle1[i];
                }
                LEDCycleChecker.GetComponent<MeshRenderer>().material = LEDColors[6];
            }
            cycle1 = !cycle1;
            yield return new WaitForSeconds(2.5f);
        }
        if (Solved)
        {
            for(int i = 0; i < 5; i++)
            {
                LEDsUp[i].LEDModel.GetComponent<MeshRenderer>().material = LEDColors[7];
                LEDsDown[i].LEDModel.GetComponent<MeshRenderer>().material = LEDColors[7];
            }
            
        }
    }
        
    void NewLEDS()
    {
        for (int i = 0; i < 5; i++)
        {
            LED _led = new LED();
            _led.LEDModel = LEDModelsUP[i];
            for (int j = 0; j < 2; j++)
            {
                
                int randomcolorled = Random.Range(0, 8);
                if (j == 0)
                {
                    switch (randomcolorled)
                    {
                        case 0:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[0];
                            LEDRow1Cycle1[i] = LEDColors[0];
                            _led.Color1 = LEDColors[0];
                            _led.R1 = true;
                            _led.CharColor1 = 'R';
                            break;
                        case 1:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[1];
                            LEDRow1Cycle1[i] = LEDColors[1];
                            _led.Color1 = LEDColors[1];
                            _led.G1 = true;
                            _led.CharColor1 = 'G';
                            break;
                        case 2:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[2];
                            LEDRow1Cycle1[i] = LEDColors[2];
                            _led.Color1 = LEDColors[2];
                            _led.B1 = true;
                            _led.CharColor1 = 'B';
                            break;
                        case 3:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[3];
                            LEDRow1Cycle1[i] = LEDColors[3];
                            _led.Color1 = LEDColors[3];
                            _led.R1 = true;
                            _led.B1 = true;
                            _led.CharColor1 = 'M';
                            break;
                        case 4:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[4];
                            LEDRow1Cycle1[i] = LEDColors[4];
                            _led.Color1 = LEDColors[4];
                            _led.R1 = true;
                            _led.G1 = true;
                            _led.CharColor1 = 'Y';
                            break;
                        case 5:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[5];
                            LEDRow1Cycle1[i] = LEDColors[5];
                            _led.Color1 = LEDColors[5];
                            _led.G1 = true;
                            _led.B1 = true;
                            _led.CharColor1 = 'C';
                            break;
                        case 6:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[6];
                            LEDRow1Cycle1[i] = LEDColors[6];
                            _led.Color1 = LEDColors[6];
                            _led.R1 = true;
                            _led.G1 = true;
                            _led.B1 = true;
                            _led.CharColor1 = 'W';
                            break;
                        case 7:
                            LEDModelsUP[i].GetComponent<MeshRenderer>().material = LEDColors[7];
                            LEDRow1Cycle1[i] = LEDColors[7];
                            _led.Color1 = LEDColors[7];
                            _led.CharColor1 = 'K';
                            break;
                    }
                }
                else
                {
                    switch (randomcolorled)
                    {
                        case 0:
                            LEDRow1Cycle2[i] = LEDColors[0];
                            _led.Color2 = LEDColors[0];
                            _led.R2 = true;
                            _led.CharColor2 = 'R';
                            break;
                        case 1:
                            LEDRow1Cycle2[i] = LEDColors[1];
                            _led.Color2 = LEDColors[1];
                            _led.G2 = true;
                            _led.CharColor2 = 'G';
                            break;
                        case 2:
                            LEDRow1Cycle2[i] = LEDColors[2];
                            _led.Color2 = LEDColors[2];
                            _led.B2 = true;
                            _led.CharColor2 = 'B';
                            break;
                        case 3:
                            LEDRow1Cycle2[i] = LEDColors[3];
                            _led.Color2 = LEDColors[3];
                            _led.R2 = true;
                            _led.B2 = true;
                            _led.CharColor2 = 'M';
                            break;
                        case 4:
                            LEDRow1Cycle2[i] = LEDColors[4];
                            _led.Color2 = LEDColors[4];
                            _led.R2 = true;
                            _led.G2 = true;
                            _led.CharColor2 = 'Y';
                            break;
                        case 5:
                            LEDRow1Cycle2[i] = LEDColors[5];
                            _led.Color2 = LEDColors[5];
                            _led.G2 = true;
                            _led.B2 = true;
                            _led.CharColor2 = 'C';
                            break;
                        case 6:
                            LEDRow1Cycle2[i] = LEDColors[6];
                            _led.Color2 = LEDColors[6];
                            _led.R2 = true;
                            _led.G2 = true;
                            _led.B2 = true;
                            _led.CharColor2 = 'W';
                            break;
                        case 7:
                            LEDRow1Cycle2[i] = LEDColors[7];
                            _led.Color2 = LEDColors[7];
                            _led.CharColor2 = 'K';
                            break;
                    }
                }
                
            }
            LEDsUp[i] = _led;

        }
        for (int i = 0; i < 5; i++)
        {
            LED led = new LED();
            led.LEDModel = LEDModelsDOWN[i];
            for (int j = 0; j < 2; j++)
            {
                int randomcolorled = Random.Range(0, 8);
                if (j == 0)
                {
                    switch (randomcolorled)
                    {
                        case 0:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[0];
                            LEDRow2Cycle1[i] = LEDColors[0];
                            led.Color1 = LEDColors[0];
                            led.R1 = true;
                            led.CharColor1 = 'R';
                            break;
                        case 1:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[1];
                            LEDRow2Cycle1[i] = LEDColors[1];
                            led.Color1 = LEDColors[1];
                            led.G1 = true;
                            led.CharColor1 = 'G';
                            break;
                        case 2:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[2];
                            LEDRow2Cycle1[i] = LEDColors[2];
                            led.Color1 = LEDColors[2];
                            led.B1 = true;
                            led.CharColor1 = 'B';
                            break;
                        case 3:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[3];
                            LEDRow2Cycle1[i] = LEDColors[3];
                            led.Color1 = LEDColors[3];
                            led.R1 = true;
                            led.B1 = true;
                            led.CharColor1 = 'M';
                            break;
                        case 4:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[4];
                            LEDRow2Cycle1[i] = LEDColors[4];
                            led.Color1 = LEDColors[4];
                            led.R1 = true;
                            led.G1 = true;
                            led.CharColor1 = 'Y';
                            break;
                        case 5:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[5];
                            LEDRow2Cycle1[i] = LEDColors[5];
                            led.Color1 = LEDColors[5];
                            led.G1 = true;
                            led.B1 = true;
                            led.CharColor1 = 'C';
                            break;
                        case 6:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[6];
                            LEDRow2Cycle1[i] = LEDColors[6];
                            led.Color1 = LEDColors[6];
                            led.R1 = true;
                            led.G1 = true;
                            led.B1 = true;
                            led.CharColor1 = 'W';
                            break;
                        case 7:
                            LEDModelsDOWN[i].GetComponent<MeshRenderer>().material = LEDColors[7];
                            LEDRow2Cycle1[i] = LEDColors[7];
                            led.Color1 = LEDColors[7];
                            led.CharColor1 = 'R';
                            break;
                    }
                }
                else
                {
                    switch (randomcolorled)
                    {
                        case 0:
                            LEDRow2Cycle2[i] = LEDColors[0];
                            led.Color2 = LEDColors[0];
                            led.R2 = true;
                            led.CharColor2 = 'R';
                            break;
                        case 1:
                            LEDRow2Cycle2[i] = LEDColors[1];
                            led.Color2 = LEDColors[1];
                            led.G2 = true;
                            led.CharColor2 = 'G';
                            break;
                        case 2:
                            LEDRow2Cycle2[i] = LEDColors[2];
                            led.Color2 = LEDColors[2];
                            led.B2 = true;
                            led.CharColor2 = 'B';
                            break;
                        case 3:
                            LEDRow2Cycle2[i] = LEDColors[3];
                            led.Color2 = LEDColors[3];
                            led.R2 = true;
                            led.B2 = true;
                            led.CharColor2 = 'M';
                            break;
                        case 4:
                            LEDRow2Cycle2[i] = LEDColors[4];
                            led.Color2 = LEDColors[4];
                            led.R2 = true;
                            led.G2 = true;
                            led.CharColor2 = 'Y';
                            break;
                        case 5:
                            LEDRow2Cycle2[i] = LEDColors[5];
                            led.Color2 = LEDColors[5];
                            led.G2 = true;
                            led.B2 = true;
                            led.CharColor2 = 'C';
                            break;
                        case 6:
                            LEDRow2Cycle2[i] = LEDColors[6];
                            led.Color2 = LEDColors[6];
                            led.R2 = true;
                            led.G2 = true;
                            led.B2 = true;
                            led.CharColor2 = 'W';
                            break;
                        case 7:
                            LEDRow2Cycle2[i] = LEDColors[7];
                            led.Color2 = LEDColors[7];
                            led.CharColor2 = 'K';
                            break;
                    }
                }
            }
            LEDsDown[i] = led;
        }


    }
    void FindLEDBoolsColors(LED[] findingleds)
    {
        var startIndex = findingleds.SequenceEqual(LEDsDown) ? 6 : 0;
        bool[] selectedR1 = new bool[5];
        bool[] selectedG1 = new bool[5];
        bool[] selectedB1 = new bool[5];
        bool[] selectedR2 = new bool[5];
        bool[] selectedG2 = new bool[5];
        bool[] selectedB2 = new bool[5];
        for (int i = 0; i < 5; i++)
        {
            selectedR1[i] = findingleds[i].R1;
            selectedG1[i] = findingleds[i].G1;
            selectedB1[i] = findingleds[i].B1;
            selectedR2[i] = findingleds[i].R2;
            selectedG2[i] = findingleds[i].G2;
            selectedB2[i] = findingleds[i].B2;
        }
        AllStates[startIndex + 0] = selectedR1;
        AllStates[startIndex + 1] = selectedG1;
        AllStates[startIndex + 2] = selectedB1;
        AllStates[startIndex + 3] = selectedR2;
        AllStates[startIndex + 4] = selectedG2;
        AllStates[startIndex + 5] = selectedB2;
    }
    void PickColorSwitchesAndSockets()
    {
        for (int i = 0; i < 5; i++)
        {
            Switch _switch = new Switch();
            Switches[i] = _switch;
            _switch.SwitchModel = SwitchModels[i];
            _switch.Socket = SocketModels[i];
            _switch.RandomState();
            SwitchesStates[i] = _switch.State;
            int randomcolorswitch = Random.Range(0, 8);
            switch (randomcolorswitch)
            {
                case 0:
                    _switch.ColorSwitch = 'R';
                    _switch.R = true;
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[0];
                    break;
                case 1:
                    _switch.ColorSwitch = 'G';
                    _switch.G = true;
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[1];
                    break;
                case 2:
                    _switch.ColorSwitch = 'B';
                    _switch.B = true;
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[2];
                    break;
                case 3:
                    _switch.ColorSwitch = 'M';
                    _switch.R = true;
                    _switch.B = true;
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[3];
                    break;
                case 4:
                    _switch.ColorSwitch = 'Y';
                    _switch.R = true;
                    _switch.G = true;
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[4];
                    break;
                case 5:
                    _switch.ColorSwitch = 'C';
                    _switch.G = true;
                    _switch.B = true;
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[5];
                    break;
                case 6:
                    _switch.ColorSwitch = 'W';
                    _switch.R = true;
                    _switch.G = true;
                    _switch.B = true;
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[6];
                    break;
                case 7:
                    _switch.ColorSwitch = 'K';
                    _switch.SwitchModel.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[7];
                    break;
            }
            int randomcolorsocket = Random.Range(0, 8);
            switch (randomcolorsocket)
            {
                case 0:
                    _switch.ColorSocket = 'R';
                    _switch.R_Socket = true;
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[0];
                    break;
                case 1:
                    _switch.ColorSocket = 'G';
                    _switch.G_Socket = true;
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[1];
                    break;
                case 2:
                    _switch.ColorSocket = 'B';
                    _switch.B_Socket = true;
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[2];
                    break;
                case 3:
                    _switch.ColorSocket = 'M';
                    _switch.R_Socket = true;
                    _switch.B_Socket = true;
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[3];
                    break;
                case 4:
                    _switch.ColorSocket = 'Y';
                    _switch.R_Socket = true;
                    _switch.G_Socket = true;
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[4];
                    break;
                case 5:
                    _switch.ColorSocket = 'C';
                    _switch.G_Socket = true;
                    _switch.B_Socket = true;
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[5];
                    break;
                case 6:
                    _switch.ColorSocket = 'W';
                    _switch.R_Socket = true;
                    _switch.G_Socket = true;
                    _switch.B_Socket = true;
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[6];
                    break;
                case 7:
                    _switch.ColorSocket = 'K';
                    _switch.Socket.GetComponent<MeshRenderer>().material = SwitchesAndSocketsColors[7];
                    break;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}
