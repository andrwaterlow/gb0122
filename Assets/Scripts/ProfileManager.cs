using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private Text _textResult;
    [SerializeField] private Image image;

    private bool connectionSuccess = false;

    void Start()
    {
        StartCoroutine(Load());
        Connect();
    }

    private void Connect()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), 
            success =>
            {
                _textResult.text = $"Welcome back, Player ID {success.AccountInfo.PlayFabId}\n" +
                $"Entertime {DateTime.Now}\n" +
                $"Was Created {success.AccountInfo.TitleInfo.Created}";
                connectionSuccess = true;
            },
            errorCallback =>
            {
                var errorMessage = errorCallback.GenerateErrorReport();
                Debug.LogError($"Something went wrong: {errorMessage}");
            });
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
