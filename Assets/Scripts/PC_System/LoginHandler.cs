using TMPro;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    private string username;
    private string password;

    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private string correctUsername = "kevStyle";
    [SerializeField] private string correctPassword = "2005";
    [SerializeField] private PC_UI_Manager uiManager;
    public void Login()
    {
        username = usernameInput.text;
        password = passwordInput.text;
        if (username == correctUsername && password == correctPassword)
        {
            uiManager.hideLogin();
            uiManager.showDesktop();
        }
    }
}
