using TMPro;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private GameObject passwordError;
    [SerializeField] private GameObject usernameError;


    private string username;
    private string password;
    [SerializeField] private string correctUsername = "kevStyle";
    [SerializeField] private string correctPassword = "2005";
    [SerializeField] private PC_UI_Manager uiManager;
    private void Start()
    {
        passwordError.SetActive(false);
        usernameError.SetActive(false);
    }
    public void Login()
    {
        username = usernameInput.text;
        password = passwordInput.text;
        if (username == correctUsername && password == correctPassword)
        {
            uiManager.hideLogin();
            uiManager.showDesktop();
            uiManager.setLoggedIn(true);
        }
        if (username != correctUsername)
        {
            usernameError.SetActive(true);
        }
        if (password != correctPassword)
        {
            passwordError.SetActive(true);
        }
        if(username == correctUsername)
        {
            usernameError.SetActive(false);
        }
        if(password == correctPassword)
        {
            passwordError.SetActive(false);
        }
    }

    public void Logout()
    {   
        uiManager.setLoggedIn(false);
        uiManager.showLogin();
        uiManager.hideDesktop();
    }
}
