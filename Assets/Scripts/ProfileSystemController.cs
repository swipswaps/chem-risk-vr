using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSystemController : MonoBehaviour {
    public GameObject ProfilesTab;
    public GameObject NewProfileTab;
    public GameObject Keyboard;
    public GameObject UsernameField;
    public GameObject StudentIDField;
    public GameObject CredentialsErrorText;

    private bool _isNewProfileTabActive = false;
    private string _newUsername = string.Empty;
    private string _newStudentID = string.Empty;
    public static string currentField = string.Empty;

    private int currentLengthOfUsername = 0;
    private int currentLengthOfStudentID = 0;

    public GameObject Pointer;
    public LayerMask CatchButtonLayer;
    private Image _image;

    void Start () {
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
                    CreateNewProfile();
                } else if (lookedAtButton.name == "Key")
                {
                    PushKey(lookedAtButton.GetComponentInChildren<Text>().text);
                }
            }
        }
            if (_image != null)
            {
                _image.color = Color.white;
            }
    }

    public void CreateNewProfile()
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
        } else
        {
            ProfilesTab.SetActive(true);
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
                CreateNewProfile();
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
