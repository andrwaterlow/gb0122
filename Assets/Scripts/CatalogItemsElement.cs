using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CatalogItemsElement : MonoBehaviour
{
    [SerializeField] private Text _elementName;
    [SerializeField] private Text _elementCost;
    [SerializeField] private Text _itemID;

    private string _ID;

    public void SetItem(CatalogItem  item)
    {
        _elementName.text = item.DisplayName;
        if (item.VirtualCurrencyPrices.ContainsKey("GC"))
        {
            _elementCost.text = item.VirtualCurrencyPrices["GC"].ToString();
        }

        GetItemID(item);
    }

    private void GetItemID(CatalogItem item)
    {
        _ID = item.ItemId;
    }

    public void SendItemID()
    {
        _itemID.text = _ID;
    }


}
