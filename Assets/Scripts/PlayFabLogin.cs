using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabLogin : MonoBehaviour
{
    [NonSerialized] public string errorMessage;

    [SerializeField] private  Image image;

    private string _mail;
    private string _userName;
    private string _password;

    private bool connectionSuccess;

    private string keyAuth = "player-unique-id";
    private const string firstKeyAuth = "player-unique-id";

    public void Start()
    {
        Connect();
        StartCoroutine(Load());
    }

    public void UpdateMail(string mail)
    {
        _mail = mail;
    }

    public void UpdateName(string userName)
    {
        _userName = userName;
    }

    public void UpdatePassword(string password)
    {
        _password = password;
    }

    public void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _userName,
            Email = _mail,
            Password = _password,
        }, result =>
        {
            Debug.Log($"Success: {_userName}");
        }, error =>
        {
            Debug.LogError($"Fail: {error.ErrorMessage}");
        });      
    }

    public void Login()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _userName,
            Password = _password,
        }, result =>
        {
            Debug.Log($"Success: {_userName}");
        }, error =>
        {
            Debug.LogError($"Fail: {error.ErrorMessage}");
        });
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "Diablo Online";
        }

        var needCreation = !PlayerPrefs.HasKey(keyAuth);
        Debug.Log($"needCreation = {needCreation}");

        var id = PlayerPrefs.GetString(keyAuth, Guid.NewGuid().ToString());
        Debug.Log($"id = {id}");

        var request = new LoginWithCustomIDRequest { CustomId = id,
            CreateAccount = needCreation};

        PlayFabClientAPI.LoginWithCustomID(request, 
            OnLoginSuccess =>
            {
                PlayerPrefs.SetString(keyAuth, id);
                Debug.Log("Congratulations, you made successful API call!");
                connectionSuccess = true;
                SceneManager.LoadScene("MainProfile");
            },
            OnLoginFailure);
        
    }

    public void CreateNewAccount()
    {
        System.Random random = new System.Random();
        var newKeyAuth = keyAuth + $"_{random.Next(0,100)}";
        
        var id = PlayerPrefs.GetString(newKeyAuth, Guid.NewGuid().ToString());
        Debug.Log($"id = {id}");

        var request = new LoginWithCustomIDRequest
        {
            CustomId = id,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            OnLoginSuccess =>
            {
                PlayerPrefs.SetString(newKeyAuth, id);
                Debug.Log("Congratulations, you made successful API call!");
                SceneManager.LoadScene("MainProfile");
            },
            OnLoginFailure);

        keyAuth = newKeyAuth;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    private IEnumerator Load()
    {
        var rotateSpeed = -150f;
        while (!connectionSuccess)
        {
            image.GetComponent<Cirle>().Active();
            image.transform.GetChild(0).transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
            yield return null;
        };
        image.GetComponent<Cirle>().Deactive();
    }
}
