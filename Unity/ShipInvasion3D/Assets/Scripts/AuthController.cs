using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthController : MonoBehaviour
{
    [SerializeField] GameObject loginSet;
    [SerializeField] GameObject signupSet;
    [SerializeField] GameObject loginForm;
    [SerializeField] GameObject signupForm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchFormToLogIn(){
        loginForm.SetActive(true);
        signupForm.SetActive(false);
    }

    public void SwitchFormToSignUp(){
        loginForm.SetActive(false);
        signupForm.SetActive(true);
    }
}
