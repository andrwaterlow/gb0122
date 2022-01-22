using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    public event Action OnConnect;
    public event Action OnError;

    public string errorMessage;

    public void Connect()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "Diablo Online";
        }

        var request = new LoginWithCustomIDRequest { CustomId = "GameSky Studio",
            CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made successful API call!");
        OnConnect.Invoke();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        OnError.Invoke();
    }
}
