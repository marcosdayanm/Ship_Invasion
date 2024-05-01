using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] GameObject alert;
    Button submitButton;

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
        loginSet.GetComponent<Image>().color = new Color(0.003921569f, 0.05490196f, 0.9568628f, 0.4392157f);
        signupSet.GetComponent<Image>().color = new Color(0.007843138f, 0.05490196f, 0.9607844f, 0.09803922f);
    }

    public void SwitchFormToSignUp(){
        loginForm.SetActive(false);
        signupForm.SetActive(true);
        signupSet.GetComponent<Image>().color = new Color(0.003921569f, 0.05490196f, 0.9568628f, 0.4392157f);
        loginSet.GetComponent<Image>().color = new Color(0.007843138f, 0.05490196f, 0.9607844f, 0.09803922f);
    }

    public void OnLoginButtonClicked(Button clickedButton)
    {
        submitButton = clickedButton;
        clickedButton.interactable = false;
        string username = loginInputUser.text;
        string password = loginInputPassword.text;

        PlayerPrefs.DeleteKey("user");
        PlayerPrefs.DeleteKey("error");

        StartCoroutine(PostPlayerLogInCredentials(username, password));
    }

    private IEnumerator PostPlayerLogInCredentials(string username, string password)
    {
        // Llama a la API para enviar las credenciales
        yield return StartCoroutine(API.PostPlayerLogInCredentials(username, password));

        // Espera un segundo antes de acceder a los PlayerPrefs
        yield return new WaitForSeconds(1.0f);

        string error = PlayerPrefs.GetString("error");
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Login failed: " + error);
            string errorMsg = PlayerPrefs.GetString("errorMsg");
            Response response = JsonUtility.FromJson<Response>(errorMsg);
            alert.SetActive(true);
            alert.transform.Find("Message").GetComponent<TMP_Text>().text = response.error;
            submitButton.interactable = true;
            yield return new WaitForSeconds(2.0f);
            alert.SetActive(false);
            yield break; // Salir de la corrutina si hay un error
        }

        string userData = PlayerPrefs.GetString("user");
        Debug.Log($"This is the userData: {userData}");
        player = JsonUtility.FromJson<PlayerDetails>(userData);
        Debug.Log($"This is the player: {player}");
        if (player != null)
        {
            sceneConnection.toMenu();
        }
        else
        {
            Debug.LogError("Failed to parse player data");
            // Manejar el caso de datos de usuario incorrectos o nulos
        }
    }

    public void OnSignUpButtonClicked(Button clickedButton) {
        submitButton = clickedButton;
        clickedButton.interactable = false;
        string username = signupInputUser.text;
        string password = signupInputPassword.text;
        string confirmPassword = signupInputConfirmPassword.text;

        PlayerPrefs.DeleteKey("user");
        PlayerPrefs.DeleteKey("error");

        if (password == confirmPassword){
            StartCoroutine(PostPlayerSignUpCredentials(username, password));
        }
    }

    private IEnumerator PostPlayerSignUpCredentials(string username, string password)
    {
        // Llama a la API para enviar las credenciales
        yield return StartCoroutine(API.PostPlayerSignUpCredentials(username, password));

        // Espera un segundo antes de acceder a los PlayerPrefs
        yield return new WaitForSeconds(1.0f);

        string error = PlayerPrefs.GetString("error");
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Sigup failed: " + error);
            string errorMsg = PlayerPrefs.GetString("errorMsg");
            Response response = JsonUtility.FromJson<Response>(errorMsg);
            alert.SetActive(true);
            alert.transform.Find("Message").GetComponent<TMP_Text>().text = response.error;
            submitButton.interactable = true;
            yield return new WaitForSeconds(2.0f);
            alert.SetActive(false);
            yield break; // Salir de la corrutina si hay un error
        }

        string userData = PlayerPrefs.GetString("user");
        Debug.Log($"This is the userData: {userData}");
        player = JsonUtility.FromJson<PlayerDetails>(userData);
        Debug.Log($"This is the player: {player}");
        if (player != null)
        {
            sceneConnection.toMenu();
        }
        else
        {
            Debug.LogError("Failed to parse player data");
            // Manejar el caso de datos de usuario incorrectos o nulos
        }
    }
}


[System.Serializable] public class Response{
    public string error;
    public string status;
}
