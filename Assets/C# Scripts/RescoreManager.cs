using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RescoreManager : MonoBehaviour {

   //List<string> logWords;
    string[] logWords;

    string[] fileNames;

    public string[] answersList;
    public string[] validateList;

    public InputField inputDirField, outputDirField, valDirField, ansDirField;

    public string inputDirectory, outputDirectory, validateListDirectory, answersListDirectory;

    public string[] wordsToScore;

    public int stateTracker;

    private int numRight;
    private int incorrectCounter;

    public string correctLog;
    public string incorrectLog;
    public string unrecLog;

    public string stringToSave;

    public string fullPath;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetInputDirectory()
    {
        inputDirectory = inputDirField.text;
    }
    public void SetOutputDirectory()
    {
        outputDirectory = outputDirField.text;
    }
    public void SetValidateDirectory()
    {
        validateListDirectory = valDirField.text;
    }
    public void SetAnswersDirectory()
    {
        answersListDirectory = ansDirField.text;
    }

    public void RunScorer()
    {
        fileNames = Directory.GetFiles(inputDirectory);

        for (int i = 0; i < fileNames.Length; i++)
        {
            LoadLogs(fileNames[i]);
        }
    }

    public void RunImporter()
    {
        fileNames = Directory.GetFiles(inputDirectory);

        string toWrite = "";

        for (int i = 0; i < fileNames.Length; i++)
        {
            string line;
            StreamReader theReader = new StreamReader(fileNames[i], Encoding.Default);
            line = theReader.ReadLine() + "\n";
            toWrite = toWrite + line;
        }

        File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"//toImport.txt", toWrite);
    }

    public void LoadLogs(string fileToRead)
    {
        stateTracker = 1;
        incorrectCounter = 0;
        stringToSave = "";
        correctLog = incorrectLog = unrecLog = "";

        LoadAnswers(answersListDirectory);
        LoadValidate(validateListDirectory);

        List<string> refinedLogWordsList1 = new List<string>();
        List<string> refinedLogWordsList2 = new List<string>();
        List<string> refinedLogWordsList3 = new List<string>();
        List<string> wordsToScore = new List<string>();
        List<string> wordTimes = new List<string>();
        List<string> hintsUsedList = new List<string>();
        List<string> outOfTimeList = new List<string>();

        hintsUsedList.Clear();
        outOfTimeList.Clear();


        string[] refinedLogWordsArray1;
        string[] refinedLogWordsArray2;
        string[] refinedLogWordsArray3;
        string[] refinedLogWordsArray4;
        string[] refinedLogWordsArray5;

        string line;
        StreamReader theReader = new StreamReader(fileToRead, Encoding.Default);
        line = theReader.ReadLine();

        logWords = line.Split('|');

        for(int i = 0; i < 4; i++)
        {
            stringToSave = stringToSave + logWords[i] + "|";
        }

        for(int i = 4; i < logWords.Length; i++)
        {
            if (logWords[i] != "")
            {
                refinedLogWordsList1.Add(logWords[i]);
            }
            
            if((i % 3) == 0)
            {
                i = i + 3;
            }
        }

        for(int i = 4; i < logWords.Length - 1; i++)
        {
            refinedLogWordsList3.Add(logWords[i]);
        }

        for (int i = 8; i < logWords.Length; i = i + 6)
        {
            hintsUsedList.Add(logWords[i]);
        }

        for (int i = 9; i < logWords.Length; i = i + 6)
        {
            outOfTimeList.Add(logWords[i]);
        }

        refinedLogWordsArray1 = refinedLogWordsList1.ToArray();
        refinedLogWordsArray4 = refinedLogWordsList3.ToArray();
        string tempString = String.Join(":", refinedLogWordsArray1);
        string tempString2 = String.Join(":", refinedLogWordsArray4);
        refinedLogWordsArray2 = tempString.Split(':',';');
        refinedLogWordsArray5 = tempString2.Split(':', ';');

        for(int i = 0; i < refinedLogWordsArray2.Length; i++)
        {
            if (refinedLogWordsArray2[i] != "")
            {
                refinedLogWordsList2.Add(refinedLogWordsArray2[i]);
            }
        }

        refinedLogWordsArray3 = refinedLogWordsList2.ToArray();
        

        for (int j = 0; j < refinedLogWordsArray3.Length-1; j = j + 2)
        {
            wordsToScore.Add(refinedLogWordsArray3[j]);
            wordTimes.Add(refinedLogWordsArray3[j+1]);
        }

        int secondIndex = 0;
        int numberOfQuestions = 0;

        for(int i = 0; i < refinedLogWordsArray5.Length; i++)
        {
            if (refinedLogWordsArray5[i] == "true" || refinedLogWordsArray5[i] == "false")
            {
                numberOfQuestions++;
            }
        }

        int test = 0;

        try
        {
            for (int i = 0; i < refinedLogWordsArray5.Length; i++)
            {
                test = i;

                if (refinedLogWordsArray5[i] == wordsToScore[secondIndex])
                {
                    CheckInput(wordsToScore[secondIndex], wordTimes[secondIndex]);
                    secondIndex++;
                }
                else if ((refinedLogWordsArray5[i] == "true" || refinedLogWordsArray5[i] == "false") && (stateTracker <= numberOfQuestions))
                {
                    stateTracker++;
                    stringToSave = stringToSave + correctLog + "|";
                    stringToSave = stringToSave + incorrectLog + "|";
                    stringToSave = stringToSave + unrecLog + "|";
                    stringToSave = stringToSave + incorrectCounter + "|";
                    stringToSave = stringToSave + hintsUsedList[stateTracker-2] + "|";
                    stringToSave = stringToSave + outOfTimeList[stateTracker-2] + "|";

                    correctLog = incorrectLog = unrecLog = "";
                    incorrectCounter = 0;
                    LoadAnswers(answersListDirectory);
                }

            }
        }catch(Exception e)
        {
            print("error:"+" st: "+stateTracker +" , numberQ: "+numberOfQuestions +", wstLength: " + wordsToScore.Count + " ,second: " + secondIndex +" , iL: "+refinedLogWordsArray5.Length+" , i: "+test);
        }
        //stateTracker++;
        stringToSave = stringToSave + correctLog + "|";
        stringToSave = stringToSave + incorrectLog + "|";
        stringToSave = stringToSave + unrecLog + "|";
        stringToSave = stringToSave + incorrectCounter + "|";
        stringToSave = stringToSave + hintsUsedList[stateTracker-1] + "|";
        stringToSave = stringToSave + outOfTimeList[stateTracker-1] + "|";
        stringToSave = stringToSave + "ScenarioComplete";

        fullPath = outputDirectory + "Scenario-" + logWords[1] + "_User-" + logWords[0]+".txt";

        saveLog(fullPath, stringToSave);

        stringToSave = "";
        incorrectCounter = 0;
        correctLog = incorrectLog = unrecLog = "";

    }

    public void CheckInput(string refinedWord, string time)
    {
        int numRight = 0;
        string normalized;
        string other = refinedWord;

        normalized = other.ToLower();
        normalized = normalized.Trim();
        normalized = normalized.Replace(" ", "");

        for (int i = 0; i < answersList.Length; i++)
        {
            if (normalized == answersList[i])
            {
                numRight++;
            }
        }

        if (numRight > 0)
        {
            correctLog = other + ":" + time;
        }
        else
        {
            if (CheckValid(normalized))
            {
                incorrectCounter++;
                incorrectLog = incorrectLog + other + ":" + time + ";";
            }
            else
            {
                unrecLog = unrecLog + other + ":" + time + ";";
            }
        }

    }

    //loads in a textfile to a list.  Delimiter is a comma.
    public bool LoadAnswers(string fileName)
    {
        string line;

        StreamReader theReader = new StreamReader(fileName, Encoding.Default);

        using (theReader)
        {
            var textInBetween = new List<string>();

            bool startTagFound = false;

            while (!theReader.EndOfStream)
            {
                line = theReader.ReadLine();
                if (System.String.IsNullOrEmpty(line))
                    continue;

                if (!startTagFound)
                {
                    startTagFound = line.StartsWith("QUESTION" + stateTracker + "START");
                    continue;
                }

                bool endTagFound = line.StartsWith("QUESTION" + stateTracker + "END");
                if (endTagFound)
                {

                    foreach (var el in textInBetween)
                    {
                        if (line != null)
                        {

                            answersList = el.Split(';');

                        }
                    }
                    textInBetween.Clear();
                    continue;
                }

                textInBetween.Add(line);
            }
            return true;
        }
    }

    //loads in the text file containing all possible scenario words for validation
    public bool LoadValidate(string fileName)
    {
        string line;

        StreamReader theReader = new StreamReader(fileName, Encoding.Default);

        using (theReader)
        {

            do
            {

                line = theReader.ReadLine();

                if (line != null)
                {

                    validateList = line.Split(';');

                }
            }

            while (line != null);
            theReader.Close();
            return true;
        }
    }



    public bool CheckValid(string normalized)
    {
        numRight = 0;

        for(int i = 0; i < validateList.Length; i++)
        {
            if (normalized == validateList[i])
            {
                numRight++;
            }
        }

        if(numRight > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void saveLog(string fullPath, string stringToSave)
    {

        File.AppendAllText(fullPath, stringToSave);

    }
}
