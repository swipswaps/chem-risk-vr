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

    private int currentLengthOfUsername = 0;
    private int currentLengthOfStudentID = 0;

    public GameObject Pointer;
    public LayerMask CatchButtonLayer;
    private Image _image;

    public static bool _isGameStarted = false;

    void Start ()
    {
        ReadProfiles();
	}
	
	void Update () {
        Ray ray = new Ray(Pointer.transform.position, Pointer.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(Pointer.transform.position, Pointer.transform.forward);

        // If the player happens to hover over UI elements such as
        // buttons, then they will be updated here on different events.
        if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
        {
            var lookedAtButton = hit.collider.gameObject;
            if (lookedAtButton.GetComponent<Image>() != null)
            {
                _image = lookedAtButton.GetComponent<Image>();
                _image.color = Color.red;
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                Input.GetKeyDown(KeyCode.K))
            {
                if (lookedAtButton.GetComponent<Image>() != null)
                {
                    _image.color = Color.green;
                }

                // Button functionality/behaviour on click events
                if (lookedAtButton.name == "Username Field")
                {
                    currentField = "Username";
                    UsernameField.GetComponent<Text>().color = new Color(0, 255, 0, 1);
                    StudentIDField.GetComponent<Text>().color = new Color(255, 255, 255, 1);
                } else if (lookedAtButton.name == "Student ID Field")
                {
                    currentField = "StudentID";
                    StudentIDField.GetComponent<Text>().color = new Color(0, 255, 0, 1);
                    UsernameField.GetComponent<Text>().color = new Color(255, 255, 255, 1);
                } else if (lookedAtButton.name == "Create New Profile Button")
                {
                    ToggleWindows();
                } else if (lookedAtButton.name == "Key")
                {
                    PushKey(lookedAtButton.GetComponentInChildren<Text>().text);
                } else if (lookedAtButton.name == "Profile(Clone)")
                {
                    _isGameStarted = true;
                }
            }
        }
        else
        {
            if (_image != null)
            {
                _image.color = Color.white;
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

    public void CreateNewProfile()
    {
        // The new file for the profile should use the
        // following name structure: Nikolay_376469 (Name_ID).
        string path = Application.dataPath +
            "/StreamingAssets/User Profiles/" +
            _newUsername + "_" + _newStudentID + ".txt";
        StreamWriter newProfile = new StreamWriter(path);

        // Using must always be used in case you forget
        // to close the input stream to the file you're writing to.
        using (newProfile)
        {
            newProfile.WriteLine("Username=" + _newUsername);
            newProfile.WriteLine("StudentID=" + _newStudentID);
            // Just in case XD
            newProfile.Close();
        }
    }

    private void ReadProfiles()
    {
        int topYOffset = 100;
        int profilesInTab = ProfilesTab.transform.GetChild(1).childCount;

        for (int j = profilesInTab - 1; j >= 0; j--)
        {
            Destroy(ProfilesTab.transform.GetChild(1).transform.GetChild(j).gameObject);
        }

        string path = Application.dataPath +
            "/StreamingAssets/User Profiles/";
        DirectoryInfo profilesDir = new DirectoryInfo(path);
        FileInfo[] profileFiles = profilesDir.GetFiles();

        foreach (FileInfo profile in profileFiles)
        {
            string username = string.Empty;
            string studentID = string.Empty;

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

                                if (line.Substring(0, i) == "Username")
                                {
                                    // I'm using Remove because of an error I cannot understand when attemping to use the Substring methodto return the string after the equals sign.
                                    username = temporaryLine.Remove(0, i + 1);
                                }
                                else if (line.Substring(0, i) == "StudentID")
                                {
                                    studentID = temporaryLine.Remove(0, i + 1);
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

                    newProfileSelectionButton.GetComponentInChildren<Text>().text = username + " (" + studentID + ")";
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
