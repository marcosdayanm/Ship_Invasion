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


    public void OnLoginButtonClicked()
    {
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
            // Aquí puedes mostrar un mensaje al usuario sobre el error
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


    public void OnSignUpButtonClicked() {
        string username = signupInputUser.text;
        string email = signupInputPassword.text;
        string password = signupInputConfirmPassword.text;
    }
}
