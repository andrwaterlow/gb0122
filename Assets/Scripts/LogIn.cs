using UnityEngine;
using UnityEngine.UI;

public class LogIn : MonoBehaviour
{
    [SerializeField] private Button buttonLogIn;
    [SerializeField] private Text textResult;
    [SerializeField] private PlayFabLogin playFabLogin;

    void Start()
    {
        buttonLogIn.onClick.AddListener(() => Connect());
        playFabLogin.OnConnect += SuccessfulConnect;
        playFabLogin.OnError += ErrorConnect;
    }

    private void Connect()
    {
        playFabLogin.Connect();
    }

    private void SuccessfulConnect()
    {
        textResult.text = "Congratulations, you made successful API call!";
        textResult.color = Color.green;
    }

    private void ErrorConnect()
    {
        textResult.text = $"Something went wrong - {playFabLogin.errorMessage}";
        textResult.color = Color.red;
    }
}
