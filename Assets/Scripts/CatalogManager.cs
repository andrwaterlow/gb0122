using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogManager : MonoBehaviour
{
    [SerializeField] private CatalogItemsElement _elements;

    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    private void Start()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnFailure);
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        HandleCatalog(result.Catalog);
        Debug.Log($"Catalog was loaded successfully!");
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        string virtualCurrency = "GC";
        foreach (var item in catalog)
        {
            _catalog.Add(item.ItemId, item);
            Debug.Log($"Catalog item {item.ItemId} was added successfully!");

            if (item.VirtualCurrencyPrices.ContainsKey(virtualCurrency))
            {
                var element = Instantiate(_elements, _elements.transform.parent);
                element.gameObject.SetActive(true);
                element.SetItem(item);
            } 
        }
    }

    public void BuyItem()
    {
        string virtualCurrency = "GC";
        var ID = gameObject.transform.GetChild(0).GetComponent<Text>().text;
        _catalog.TryGetValue(ID, out var item);
        item.VirtualCurrencyPrices.TryGetValue(virtualCurrency, out var currency);

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), resultCallback =>
        {
            if (resultCallback.VirtualCurrency.ContainsKey(virtualCurrency))
            {
                if (resultCallback.VirtualCurrency[virtualCurrency] >= currency)
                {
                    Debug.Log("Success");
                }
                else
                {
                    Debug.Log("Fail");
                }
            }
        },
        errorCallback =>
        {
            var errorMessage = errorCallback.GenerateErrorReport();
            Debug.LogError($"Something went wrong: {errorMessage}");
        });
    }
}
