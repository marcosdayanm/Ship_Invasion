using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AuthController : MonoBehaviour
{
    APIConnection API = null;
    PlayerDetails player = null;
    SceneConnection sceneConnection = null;
    [SerializeField] GameObject loginSet;
    [SerializeField] GameObject signupSet;
    [SerializeField] GameObject loginForm;
    [SerializeField] GameObject signupForm;

    [SerializeField] TMP_InputField loginInputUser;
    [SerializeField] TMP_InputField loginInputPassword;

    [SerializeField] TMP_InputField signupInputUser;
    [SerializeField] TMP_InputField signupInputPassword;
    [SerializeField] TMP_InputField signupInputConfirmPassword;

    // Start is called before the first frame update
    // void Start()
    // {
    //     API = GetComponent<APIConnection>();
    //     sceneConnection = GameObject.FindWithTag("SceneConnection").GetComponent<SceneConnection>();
    // }

    void Start()
    {
        API = GameObject.FindWithTag("APIConnection").GetComponent<APIConnection>();
        if (API == null) {
            Debug.LogError("APIConnection component not found on the object.");
        }

        GameObject sceneConnectionObject = GameObject.FindWithTag("SceneConnection");
        if (sceneConnectionObject != null) {
            sceneConnection = sceneConnectionObject.GetComponent<SceneConnection>();
            if (sceneConnection == null) {
                Debug.LogError("SceneConnection component not found on the object with tag 'SceneConnection'.");
            }
        } else {
            Debug.LogError("No GameObject with tag 'SceneConnection' found in the scene.");
        }
    }


    public void SwitchFormToLogIn(){
        loginForm.SetActive(true);
        signupForm.SetActive(false);
    }

    public void SwitchFormToSignUp(){
        loginForm.SetActive(false);
        signupForm.SetActive(true);
    }

    public void OnLoginButtonClicked() {
        string username = loginInputUser.text;
        string password = loginInputPassword.text;

        StartCoroutine(API.PostPlayerLogInCredentials(username, password));
        player = JsonUtility.FromJson<PlayerDetails>(PlayerPrefs.GetString("user"));
        if (player != null) {
            sceneConnection.toMenu();
        }

    }


    public void OnSignUpButtonClicked() {
        string username = signupInputUser.text;
        string email = signupInputPassword.text;
        string password = signupInputConfirmPassword.text;
    }
}
