using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using LitJson;

public class Inventory : MonoBehaviour
{
    private struct Item
    {
        public string ItemType;
        public int ItemID;
        public string ItemName;
        public string NftFamilyID;
    }

    public static JsonData itemData;
    static List<Item> GameItems = new List<Item>();

    void Start()
    {
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=0,
            ItemName="Skin 1",
            NftFamilyID="skin_1",
        });
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=1,
            ItemName="Skin 2",
            NftFamilyID="skin_2",
        });
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=2,
            ItemName="Skin 3",
            NftFamilyID="skin_3",
        });
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=3,
            ItemName="Skin 4",
            NftFamilyID="skin_4",
        });
        //
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=4,
            ItemName="Skin 5",
            NftFamilyID="skin_5",
        });
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=5,
            ItemName="Skin 6",
            NftFamilyID="skin_6",
        });
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=6,
            ItemName="Skin 7",
            NftFamilyID="skin_7",
        });
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=7,
            ItemName="Skin 8",
            NftFamilyID="skin_8",
        });        
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=8,
            ItemName="Skin 9",
            NftFamilyID="skin_9",
        });        
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=9,
            ItemName="Skin 10",
            NftFamilyID="skin_10",
        });        
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=10,
            ItemName="Skin 11",
            NftFamilyID="skin_11",
        });        
        GameItems.Add(new Item()
        {
            ItemType="PlayerSkin",
            ItemID=11,
            ItemName="Skin 12",
            NftFamilyID="skin_12",
        });
        GameItems.Add(new Item()
        {
            ItemType="Emblem",
            ItemID=1,
            ItemName="King",
            NftFamilyID="king",
        });          
        GameItems.Add(new Item()
        {
            ItemType="Emblem",
            ItemID=9,
            ItemName="Loco Roco",
            NftFamilyID="loco_roco",
        });                
    }

    public void PrintInventory()
    {
        if (itemData.Count>0)
        {
            Debug.Log("Item Count for NFT Inventory -> " + itemData.Count);
            for (int i = 0; i < itemData.Count; i++)
            {
                Debug.Log("Token ID : " + (itemData[i]["token_id"]!=null?itemData[i]["token_id"].ToString():""));
                Debug.Log("NFT ID : " +  (itemData[i]["nft_id"]!=null?itemData[i]["nft_id"].ToString():""));
                Debug.Log("NFT Family ID : " + (itemData[i]["family_id"]!=null?itemData[i]["family_id"].ToString():""));
                Debug.Log("NFT Name : " + (itemData[i]["name"]!=null?itemData[i]["name"].ToString():""));
                Debug.Log("NFT Description : " + (itemData[i]["description"]!=null?itemData[i]["description"].ToString():""));
            }
        }
        else
        {
            Debug.Log("No NFT found in your NFT inventory.");
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("Login-Fullscreen");
    }

    public static bool IsItemInInventory(string ItemType, int ItemID)
    {
        string NftFamilyID=null;
        bool isItemFound=false;
        bool isNftFound=false;

        for (int i = 0; i < GameItems.Count; i++)
        {
            if (GameItems[i].ItemType==ItemType && GameItems[i].ItemID==ItemID)
            {
                Debug.Log("["+ItemType+"-"+ItemID + "] -> Family ID -> " + GameItems[i].NftFamilyID);
                NftFamilyID=GameItems[i].NftFamilyID;
                isItemFound=true;
                break;
            }
        }
        if (isItemFound&&NftFamilyID!=null)
        {
            for (int i = 0; i < itemData.Count; i++)
            {
                if (itemData[i]["family_id"].ToString()==NftFamilyID)
                {
                    Debug.Log("NFT exist in inventory -> " + itemData[i]["family_id"].ToString());
                    isNftFound=true;
                }
            }
            return isNftFound;
        }
        else
        {
            return false;
        }
    }

    public bool IsNFTAvailable(string nft_family_id)
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            if (itemData[i]["nft_family_id"].ToString()==nft_family_id)
            {
                return true;
            }
        }
        return false;
    } 
}