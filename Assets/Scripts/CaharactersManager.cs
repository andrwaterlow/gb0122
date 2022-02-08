using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaharactersManager : MonoBehaviour
{
    [SerializeField] private GameObject _charactersPanel;
    [SerializeField] private GameObject _nameCharacterPanel;
    [SerializeField] private CharacterView _characterView;

    private readonly List<CharacterResult> _characters = new List<CharacterResult>();
    private readonly List<CharacterView> characterViews = new List<CharacterView>();

    private string _characterName;

    private void Start()
    {
        StartCoroutine(FirstUpdateCharactersView());
    }

    #region PLAYFAB

    private void CreateNewCharacter()
    {
        CheckTokenCharacter();
    }

    private void CheckTokenCharacter()
    {
        string tokenCharacterID = "Ch_t";
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            resultCallback =>
            {
                var inentory = resultCallback.Inventory;
                foreach (var item in inentory)
                {
                    if (item.ItemId == tokenCharacterID)
                    {
                        TransformTokenToCharacter(tokenCharacterID);
                        return;
                    }
                }
            },
            errorCallback =>
            {
                Debug.Log(errorCallback.ErrorMessage);
            });
    }

    private void TransformTokenToCharacter(string itemID)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = _characterName,
            ItemId = itemID
        },
            resultCallback =>
            {
                UpdateCharacterStatistics(resultCallback.CharacterId);
            },
            errorCallback =>
            {
                Debug.Log(errorCallback.ErrorMessage);
            });
    }

    private void UpdateCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest()
        {
            CharacterId = characterId,
            CharacterStatistics = new Dictionary<string, int>
                 {
                     {"Level", 1},
                     {"Experience", 0},
                     {"Kills", 0}
                 }
        },
            resultCallback =>
            {
                Debug.Log("Character was created");
            },
            errorCallback =>
            {
                Debug.Log(errorCallback.ErrorMessage);
            });
    }

    private void GetAllCharacters()
    {
        _characters.Clear();

        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            resultCallback =>
            {
                foreach (var item in resultCallback.Characters)
                {
                    _characters.Add(item);
                }
            },
            errorCallback =>
            {
                Debug.Log(errorCallback.ErrorMessage);
            });
    }

    #endregion

    #region UI

    public void SetName(string name)
    {
        _characterName = name;
    }

    public void OnPanelButtonClick()
    {
        if (!_charactersPanel.activeInHierarchy)
        {
            _charactersPanel.SetActive(true);
        }
        else
        {
            _charactersPanel.SetActive(false);
        }
    }

    public void OnAddButtonClick()
    {
        _nameCharacterPanel.SetActive(true);
        ClearAllCharactersView();
        Debug.Log(_characters.Count);
    }

    public void OnCreateButtonClick()
    {
        CreateNewCharacter();
        _nameCharacterPanel.SetActive(false);
        StartCoroutine(UpdateCharactersViewAfterCreate());
    }

    #endregion

    #region UNITY

    private void ShowAllCharacters()
    {
        if (_characters.Count > 0)
        {
            foreach (var item in _characters)
            {
                AddOneCharacterView(item);
            }
        }
    }

    private void AddOneCharacterView(CharacterResult character)
    {
        var characterView = Instantiate(_characterView, _characterView.transform.parent);
        characterView.gameObject.SetActive(true);
        characterView.SetCharacter(character);
        characterViews.Add(characterView);
    }

    private void ClearAllCharactersView()
    {
        foreach (var character in characterViews)
        {
            character.Clear();
        }
    }

    private IEnumerator UpdateCharactersViewAfterCreate()
    {
        float waitingTime = 1.5f;
        yield return new WaitForSeconds(waitingTime);
        GetAllCharacters();
        yield return new WaitForSeconds(waitingTime);
        ShowAllCharacters();
    }

    private IEnumerator FirstUpdateCharactersView()
    {
        float waitingTime = 1.5f;
        GetAllCharacters();
        yield return new WaitForSeconds(waitingTime);
        ShowAllCharacters();
    }

    #endregion
}
