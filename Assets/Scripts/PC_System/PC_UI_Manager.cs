using UnityEngine;

public class PC_UI_Manager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject desktopPanel;
    [SerializeField] private bool loggedIn;

    private void Start()
    {
        loggedIn = false;
    }


    public void showLogin()
    {
        if (loggedIn)
        {
            return;
        }
        loginPanel.SetActive(true);
    }

    public void hideLogin()
    {
        loginPanel.SetActive(false);
    }

    public void showDesktop()
    {
        desktopPanel.SetActive(true);
    }

    public void hideDesktop()
    {
        desktopPanel.SetActive(false);
    }

    public void setLoggedIn(bool isLoggedIn)
    {
        loggedIn = isLoggedIn;
    }





}
