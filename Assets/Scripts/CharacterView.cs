using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Text _level;
    [SerializeField] private Text _experience;
    [SerializeField] private Text _kills;

    private const string LEVEL = "Level";
    private const string EXPERIENCE = "Experience";
    private const string KILLS = "Kills";

    public void SetCharacter(CharacterResult character)
    {
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest()
        {
            CharacterId = character.CharacterId
        },
            resultCallback =>
            {
                var statistics = resultCallback.CharacterStatistics;

                statistics.TryGetValue(LEVEL, out var level);
                statistics.TryGetValue(EXPERIENCE, out var experience);
                statistics.TryGetValue(KILLS, out var kills);

                _name.text = $"{character.CharacterName}";
                _level.text = $"Level: {level.ToString()}";
                _experience.text = $"Exp: {experience.ToString()}";
                _kills.text = $"Kills: {kills.ToString()}";
            },
            errorCallback =>
            {
                Debug.Log(errorCallback.ErrorMessage);
            });
    }

    public void Clear()
    {
        Destroy(gameObject);
    }
}
