using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSystemController : MonoBehaviour {
    public GameObject ProfilesTab;
    public GameObject NewProfileTab;
    public GameObject Keyboard;
    public GameObject UsernameField;
    public GameObject StudentIDField;
    public GameObject CredentialsErrorText;
    public GameObject ProfileButtonTemplate;

    private bool _isNewProfileTabActive = false;
    private string _newUsername = string.Empty;
    private string _newStudentID = string.Empty;
    public static string currentField = string.Empty;

    private string _currentUsername = string.Empty;
    private string _currentStudentID = string.Empty;
    private int currentLengthOfUsername = 0;
    private int currentLengthOfStudentID = 0;

    public GameObject Pointer;
    public LayerMask CatchButtonLayer;

    public static bool _isGameStarted = false;
    //public GameObject ProfileTablet;

    #region Player Variables
    private GameObject _player;
    public static string Username = string.Empty;
    public static string StudentID = string.Empty;
    public static int BeakersFilledWithWater = 0;
    public static int BeakersMixedToOrange = 0;
    public static int BeakersMixedToPurple = 0;
    public static int BeakersMixedToGreen = 0;
    public static int TriesOnLevelMixColors = 0;
    public static int TimesAGuidelineIsMissed = 0;
    public static int TimesAnIncidentWasCaused = 0;

    private int _totalSecondsPlayed = 0;
    private static int TotalMinutesPlayed = 0;
    private static int TotalHoursPlayed = 0;
    public static int CurrentLevelSeconds = 0;
    public static bool PlayingALevel = false;
    public static List<long> TimesForLevelMixColors = new List<long>();
    #endregion

    #region Player preview statistics in the lobby
    public GameObject UsernameObj;
    //public Sprite Portrait;
    public GameObject TotalHoursPlayedObj;
    public GameObject TotalMinutesPlayedObj;
    public GameObject TriesOnLevelMixColorsObj;
    public GameObject BeakersFilledWithWaterObj;
    #endregion

    private GameObject _lastLookedAtObject;
    private Image _imageOfButton;

    void Start ()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.GetComponent<CharacterController>().enabled = false;
        _player.GetComponent<SamplePlayerController>().EnableLinearMovement = false;

        ReadProfiles();

        // This starts the countdown of the timers for total hours and minutes played.
        InvokeRepeating("UpdateTimers", 0f, 1f);
	}
	
	void Update () {
        Ray ray = new Ray(Pointer.transform.position, Pointer.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(Pointer.transform.position, Pointer.transform.forward);

        // If the player happens to hover over UI elements such as
        // buttons, then they will be updated here on different events.
        if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
        {
            if (_imageOfButton != null)
            {
                _imageOfButton.color = Color.white;
            }
            _lastLookedAtObject = hit.collider.gameObject;
            if (_lastLookedAtObject.GetComponent<Image>() != null)
            {
                _imageOfButton = _lastLookedAtObject.GetComponent<Image>();
                _imageOfButton.color = Color.red;
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                Input.GetKeyDown(KeyCode.K))
            {
                if (_lastLookedAtObject.GetComponent<Image>() != null)
                {
                    _imageOfButton.color = Color.green;
                }

                // Button functionality/behaviour on click events
                if (_lastLookedAtObject.name == "Username Field")
                {
                    currentField = "Username";
                    UsernameField.GetComponent<Text>().color = new Color(0, 255, 0, 1);
                    StudentIDField.GetComponent<Text>().color = new Color(255, 255, 255, 1);
                } else if (_lastLookedAtObject.name == "Student ID Field")
                {
                    currentField = "StudentID";
                    StudentIDField.GetComponent<Text>().color = new Color(0, 255, 0, 1);
                    UsernameField.GetComponent<Text>().color = new Color(255, 255, 255, 1);
                } else if (_lastLookedAtObject.name == "Create New Profile Button")
                {
                    ToggleWindows();
                } else if (_lastLookedAtObject.name == "Key")
                {
                    PushKey(_lastLookedAtObject.GetComponentInChildren<Text>().text);
                } else if (_lastLookedAtObject.name == "Profile(Clone)")
                {
                    ReadProfiles();

                    string profileNameAndID = _lastLookedAtObject.GetComponentInChildren<Text>().text;

                    for (int i = 0; i < profileNameAndID.Length; i++)
                    {
                        if (profileNameAndID[i] == '_')
                        {
                            _currentUsername = profileNameAndID.Substring(0, i);
                            _currentStudentID = profileNameAndID.Substring(i + 1, 6);
                            break;
                        }
                    }

                    //.GetComponent<CharacterController>().enabled = true;
                    //_player.GetComponent<SamplePlayerController>().EnableLinearMovement = true;
                    _isGameStarted = true;

                    SelectProflie();

                    //Debug.Log(_currentUsername + "_" + _currentStudentID);

                    UsernameObj.GetComponentInChildren<Text>().text = "Username: " + Username;
                    TotalHoursPlayedObj.GetComponentInChildren<Text>().text = "Total Hours Played: " + TotalHoursPlayed.ToString();
                    TotalMinutesPlayedObj.GetComponentInChildren<Text>().text = "Total Minutes Played: " + TotalMinutesPlayed.ToString();
                    TriesOnLevelMixColorsObj.GetComponentInChildren<Text>().text = "Tries on level Mix Colors: " + TriesOnLevelMixColors.ToString();
                    BeakersFilledWithWaterObj.GetComponentInChildren<Text>().text = "Beakers filled with water: " + BeakersFilledWithWater.ToString();

                    //ProfileTablet.SetActive(false);
                }
                else if (_lastLookedAtObject.name == "Logout Button")
                {
                    Vector3 newPlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
                    newPlayerPosition = GameObject.FindGameObjectWithTag("Start Position").transform.position;
                    GameObject.FindGameObjectWithTag("Player").transform.position = newPlayerPosition;

                    _player.GetComponent<SamplePlayerController>().EnableLinearMovement = false;
                    _player.GetComponent<CharacterController>().enabled = false;

                    //ProfileTablet.SetActive(true);
                }
            }
        }
        else
        {
            if (_imageOfButton != null && (_imageOfButton.color == Color.red || _imageOfButton.color == Color.green))
            {
                _imageOfButton.color = Color.white;
            }
        }
    }

    public void ToggleWindows()
    {
        // If we click the button to create the new profile,
        // then we want every window related to that action to
        // be toggled accordingly.
        _isNewProfileTabActive = !_isNewProfileTabActive;

        Keyboard.SetActive(_isNewProfileTabActive);
        NewProfileTab.SetActive(_isNewProfileTabActive);
        
        if (_isNewProfileTabActive)
        {
            ProfilesTab.SetActive(false);

            // We have to reset the fields every time we 
            // enter the profile creation tab
            _newUsername = string.Empty;
            _newStudentID = string.Empty;

            UsernameField.GetComponent<Text>().text = "Username: " + _newUsername;
            StudentIDField.GetComponent<Text>().text = "Student ID: " + _newStudentID;

            UsernameField.GetComponent<Text>().color = new Color(255, 255, 255, 1);
            StudentIDField.GetComponent<Text>().color = new Color(255, 255, 255, 1);
        } else
        {
            ProfilesTab.SetActive(true);
        }
    }

    private void UpdateTimers()
    {
        if (PlayingALevel)
        {
            CurrentLevelSeconds++;
            //Debug.Log(CurrentLevelSeconds);
        }

        _totalSecondsPlayed++;

        if (_totalSecondsPlayed >= 60)
        {
            TotalMinutesPlayed++;
            _totalSecondsPlayed = 0;
            if (TotalMinutesPlayed >= 60)
            {
                TotalHoursPlayed++;
                TotalMinutesPlayed = 0;
            }
        }
        
        TotalHoursPlayedObj.GetComponentInChildren<Text>().text = "Total Hours Played: " + TotalHoursPlayed.ToString();
        TotalMinutesPlayedObj.GetComponentInChildren<Text>().text = "Total Minutes Played: " + TotalMinutesPlayed.ToString();
        TriesOnLevelMixColorsObj.GetComponentInChildren<Text>().text = "Tries on level Mix Colors: " + TriesOnLevelMixColors.ToString();
        BeakersFilledWithWaterObj.GetComponentInChildren<Text>().text = "Beakers filled with water: " + BeakersFilledWithWater.ToString();

        UpdateProfileData();
    }

    public void CreateNewProfile()
    {
        // The new file for the profile should use the
        // following name structure: Nikolay_376469 (Name_ID).
        string path = Application.persistentDataPath + "/" +
            _newUsername + "_" + _newStudentID + ".txt";
        StreamWriter newProfile = new StreamWriter(path);

        // Using must always be used in case you forget
        // to close the input stream to the file you're writing to.
        using (newProfile)
        {
            newProfile.WriteLine("Username=" + _newUsername);
            newProfile.WriteLine("StudentID=" + _newStudentID);
            newProfile.WriteLine("BeakersFilledWithWater=" + 0);
            newProfile.WriteLine("BeakersMixedToOrange=" + 0);
            newProfile.WriteLine("BeakersMixedToPurple=" + 0);
            newProfile.WriteLine("BeakersMixedToGreen=" + 0);
            newProfile.WriteLine("TriesOnLevelMixColors=" + 0);
            newProfile.WriteLine("TimesAGuidelineIsMissed=" + 0);
            newProfile.WriteLine("TimesAnIncidentWasCaused=" + 0);
            newProfile.WriteLine("TotalHoursPlayed=" + 0);
            newProfile.WriteLine("TotalMinutesPlayed=" + 0);
            //newProfile.WriteLine("TimesForLevelMixColors=", string.Empty);
            // Just in case using doesnt close the stream.
            newProfile.Close();
        }
    }

    public static void UpdateProfileData()
    {
        string path = Application.persistentDataPath + "/" +
            Username + "_" + StudentID + ".txt";

        string finalTimesString = "{";

        for (int i = 0; i <= TimesForLevelMixColors.Count - 1; i++)
        {
            if (i == TimesForLevelMixColors.Count - 1)
            {
                finalTimesString += TimesForLevelMixColors[i] + "}";
            } else
            {
                finalTimesString += TimesForLevelMixColors[i] + ",";
            }
        }

        File.WriteAllLines(path, new string[] {
            "Username=" + Username,
            "StudentID=" + StudentID,
            "BeakersFilledWithWater=" + BeakersFilledWithWater,
            "BeakersMixedToOrange=" + BeakersMixedToOrange,
            "BeakersMixedToPurple=" + BeakersMixedToPurple,
            "BeakersMixedToGreen=" + BeakersMixedToGreen,
            "TriesOnLevelMixColors=" + TriesOnLevelMixColors,
            "TimesAGuidelineIsMissed=" + TimesAGuidelineIsMissed,
            "TimesAnIncidentWasCaused=" + TimesAnIncidentWasCaused,
            "TotalHoursPlayed=" + TotalHoursPlayed,
            "TotalMinutesPlayed=" + TotalMinutesPlayed
            //"TimesForLevelMixColors=" + finalTimesString
        });
    }

    private void ReadProfiles()
    {
        int topYOffset = 100;
        int profilesInTab = ProfilesTab.transform.GetChild(1).childCount;

        for (int j = profilesInTab - 1; j >= 0; j--)
        {
            Destroy(ProfilesTab.transform.GetChild(1).transform.GetChild(j).gameObject);
        }

        string path = Application.persistentDataPath;
        DirectoryInfo profilesDir = new DirectoryInfo(path);
        FileInfo[] profileFiles = profilesDir.GetFiles();

        foreach (FileInfo profile in profileFiles)
        {
            // Unity creates meta files so we need to distinguish
            // between our txt profile files and the meta unity ones.
            int fileTypeString = profile.Name.Length - 3;
            if (profile.Name[profile.Name.Length - 1] == 't' &&
                profile.Name[profile.Name.Length - 2] == 'x' &&
                profile.Name[profile.Name.Length - 3] == 't')
            {
                // OpenRead returns a streamReader object
                StreamReader reader = new StreamReader(profile.OpenRead());

                using (reader)
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        // We want to get the specific substring from the
                        // data of the current file's line being read.
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '=')
                            {
                                string temporaryLine = line;

                                switch (line.Substring(0, i))
                                {
                                    case "Username":
                                        // I'm using Remove because of an error I cannot understand when attemping to use the Substring methodto return the string after the equals sign.
                                        Username = temporaryLine.Remove(0, i + 1);
                                        break;
                                    case "StudentID":
                                        StudentID = temporaryLine.Remove(0, i + 1);
                                        break;
                                    case "BeakersFilledWithWater":
                                        BeakersFilledWithWater = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "BeakersMixedToOrange":
                                        BeakersMixedToOrange = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "BeakersMixedToPurple":
                                        BeakersMixedToPurple = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "BeakersMixedToGreen":
                                        BeakersMixedToGreen = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TriesOnLevelMixColors":
                                        TriesOnLevelMixColors = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TimesAGuidelineIsMissed":
                                        TimesAGuidelineIsMissed = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TimesAnIncidentIsCaused":
                                        TimesAnIncidentWasCaused = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TotalHoursPlayed":
                                        TotalHoursPlayed = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TotalMinutesPlayed":
                                        TotalMinutesPlayed = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                        /*
                                    case "TimesForLevelMixColors":
                                        string times = temporaryLine.Remove(0, i + 1);
                                        string time = string.Empty;
                                        for (int j = 1; j < times.Length - 1; j++)
                                        {
                                            if (times[j] == ',' || times[j] == '}')
                                            {
                                                TimesForLevelMixColors.Add(int.Parse(time));
                                                time = string.Empty;
                                            } else
                                            {
                                                time += times[j];
                                            }
                                        }
                                        break;
                                        */
                                }
                            }
                        }

                        line = reader.ReadLine();
                    }

                    // Now that all lines and data has been read and stored, we can safely instantiate the profile selector for the player to pick from.
                    GameObject newProfileSelectionButton = Instantiate(ProfileButtonTemplate, ProfilesTab.transform.GetChild(1).transform);

                    // Applying the offset in the menu so that the buttons are not
                    // all on top of each other when instantiated
                    Vector3 rectPosition = newProfileSelectionButton.GetComponent<RectTransform>().position;
                    rectPosition.y -= topYOffset; newProfileSelectionButton.GetComponent<RectTransform>().position = rectPosition;

                    topYOffset += 100;

                    newProfileSelectionButton.GetComponentInChildren<Text>().text = Username + "_" + StudentID;

                    reader.Close();
                }
            }
        }
    }

    private void SelectProflie() {

        int lengthOfProfile = 0;
        if (_isGameStarted)
        {
            lengthOfProfile = (_currentUsername.Length) + 1 + (_currentStudentID.Length);
        }
        string path = Application.persistentDataPath;
        DirectoryInfo profilesDir = new DirectoryInfo(path);
        FileInfo[] profileFiles = profilesDir.GetFiles();

        foreach (FileInfo profile in profileFiles)
        {
            if (_isGameStarted &&
            profile.Name.Substring(0, profile.Name.Length - 4) == _currentUsername + "_" + _currentStudentID &&
            profile.Name[profile.Name.Length - 1] == 't' &&
            profile.Name[profile.Name.Length - 2] == 'x' &&
            profile.Name[profile.Name.Length - 3] == 't')
            {
                // OpenRead returns a streamReader object
                StreamReader reader = new StreamReader(profile.OpenRead());

                using (reader)
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        // We want to get the specific substring from the
                        // data of the current file's line being read.
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '=')
                            {
                                string temporaryLine = line;
                                //Debug.Log(line.Substring(0, i) + "  " + line);

                                switch (line.Substring(0, i))
                                {
                                    case "Username":
                                        // I'm using Remove because of an error I cannot understand when attemping to use the Substring methodto return the string after the equals sign.
                                        Username = temporaryLine.Remove(0, i + 1);
                                        break;
                                    case "StudentID":
                                        StudentID = temporaryLine.Remove(0, i + 1);
                                        break;
                                    case "BeakersFilledWithWater":
                                        BeakersFilledWithWater = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "BeakersMixedToOrange":
                                        BeakersMixedToOrange = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "BeakersMixedToPurple":
                                        BeakersMixedToPurple = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "BeakersMixedToGreen":
                                        BeakersMixedToGreen = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TriesOnLevelMixColors":
                                        TriesOnLevelMixColors = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TimesAGuidelineIsMissed":
                                        TimesAGuidelineIsMissed = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TimesAnIncidentIsCaused":
                                        TimesAnIncidentWasCaused = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TotalHoursPlayed":
                                        TotalHoursPlayed = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                    case "TotalMinutesPlayed":
                                        TotalMinutesPlayed = int.Parse(temporaryLine.Remove(0, i + 1));
                                        break;
                                        /*
                                    case "TimesForLevelMixColors":
                                        string times = temporaryLine.Remove(0, i + 1);
                                        string time = string.Empty;
                                        for (int j = 1; j < times.Length - 1; j++)
                                        {
                                            if (times[j] == ',' || times[j] == '}')
                                            {
                                                TimesForLevelMixColors.Add(int.Parse(time));
                                                time = string.Empty;

                                            }
                                            else
                                            {
                                                time += times[j];
                                            }
                                        }
                                        break;
                                        */
                                }
                            }
                        }

                        line = reader.ReadLine();
                    }
                    reader.Close();
                }
            }
        }
    }

    public void PushKey(string keyPushed)
    {
        string newKeyToPush = string.Empty;

        if (keyPushed != "Enter" && keyPushed != "Del")
        {
            newKeyToPush += keyPushed;
        }
        else if (keyPushed == "Enter")
        {
            if (_newUsername.Length > 2 && _newStudentID.Length == 6)
            {
                // After that we generate a txt file that will contain
                // all the new profile data created by the player.
                ToggleWindows();
                CreateNewProfile();
                ReadProfiles();
            } else
            {
                CredentialsErrorText.SetActive(true);

                string errorMessage = string.Empty;
                if (_newUsername.Length < 3)
                {
                    errorMessage += "* Your username must have more than 2 characters.\n";
                }
                if (_newUsername.Length > 12)
                {
                    errorMessage += "* Your username must be between 3 and 12 characters.\n";
                }

                if (_newStudentID.Length != 6)
                {
                    errorMessage += "* Your student ID has an invalid number of digits";
                }

                CredentialsErrorText.GetComponentInChildren<Text>().text = errorMessage;
            }
        }
        else if (keyPushed == "Del")
        {
            if (currentField == "Username")
            {
                int lengthOfUsername = _newUsername.Length;
                _newUsername = _newUsername.Substring(0, lengthOfUsername -= 1);
            }
            else if (currentField == "StudentID")
            {
                int lengthOfStudentID = _newStudentID.Length;
                _newStudentID = _newStudentID.Substring(0, lengthOfStudentID -= 1);
                StudentIDField.GetComponent<Text>().text = "Student ID: " + _newStudentID;
            }
        }

        if (currentField == "Username")
        {
            _newUsername += newKeyToPush;
            UsernameField.GetComponent<Text>().text = "Username: " + _newUsername;
        } else if (currentField == "StudentID")
        {
            if (char.IsDigit(newKeyToPush[0]))
            {
                _newStudentID += newKeyToPush;
                StudentIDField.GetComponent<Text>().text = "Student ID: " + _newStudentID;
            }
        }
    }
}
