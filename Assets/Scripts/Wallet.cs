using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using LitJson;
using TMPro;
using System.IO;
using ZXing;
using ZXing.QrCode;

public static class ButtonExtension
{
    public static void AddEventListener<T> (this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener (delegate()
        {
            OnClick (param);
        });
    }
}

public class Wallet : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI status;
    [SerializeField]
    private TextMeshProUGUI address;
    [SerializeField]
    private GameObject ItemTemplate;
    [SerializeField]
    private Transform contentPanel;
    [SerializeField]
    private TMP_Dropdown DropdownCategories;
    [SerializeField]
    private RawImage _rawImageReceiver;
    [Serializable]
    private struct NFT
    {
        public string TokenID;
        public string NftID;
        public string Name;
        public string Description;
        public string ImageUrl;
        public string Category;
        public string SubCategory;
        public string FamilyID;
    }
    private struct NFTCategory
    {
        public string Key;
        public string Title;
    }    
    private class qrClass
    {
        public string code;
    }
    private struct t2d
    {
        public string id;
        public Texture2D texture;
    }
    List<NFT> NFTs = new List<NFT>();
    List<NFTCategory> NFTCategories = new List<NFTCategory>();
    JsonData itemData;
    JsonData resultData;
    private string JsonString;
    private string FilterNFTSubCategory="";
    private string ApiURL="https://api.nextwallet.org/CheckQR";
    private int ProjectID=1;
    private Texture2D _storeEncodedTexture;
    List<t2d> Texture2DArray = new List<t2d>();
    
    private string GenerateRandomAlphaNumericStr()
    {
        int desiredLength=8;
        StringBuilder Code = new StringBuilder("");
        StringBuilder GeneratedCode = new StringBuilder("");
        char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        char singleChar;
        var unixTimeSeconds = new DateTimeOffset(System.DateTime.Now).ToUnixTimeSeconds();
        Code.Append(unixTimeSeconds);
        Code.Append("-");
        while (GeneratedCode.Length < desiredLength)
        {
            singleChar = chars[UnityEngine.Random.Range(0, chars.Length)];
            GeneratedCode.Append(singleChar);
        }
        Code.Append(GeneratedCode);
        Debug.Log("Code -> " + Code.ToString());
        return Code.ToString();
    }

    private IEnumerator GetNFTsWithQRCode(string code)
    {
        qrClass qr1 = new qrClass();
        qr1.code=code; 
        while(true)
        {
            var request = new UnityWebRequest (ApiURL, "POST");
            string json=JsonUtility.ToJson(qr1);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                resultData=JsonMapper.ToObject(request.downloadHandler.text);
                //Debug.Log(request.downloadHandler.text);
                //Debug.Log(resultData["status"]);
                //Debug.Log(resultData["message"]);
                //Debug.Log(resultData["nfts"].Count);
                if (resultData["status"].ToString()=="success")
                {
                    for (int i = 0; i < resultData["nfts"].Count; i++)
                    {
                        /*
                        Debug.Log(resultData["nfts"][i]["name"]);
                        Debug.Log(resultData["nfts"][i]["token_id"]);
                        Debug.Log(resultData["nfts"][i]["nft_id"]);
                        Debug.Log(resultData["nfts"][i]["collection_name"]);
                        Debug.Log(resultData["nfts"][i]["description"]);
                        Debug.Log(resultData["nfts"][i]["family_id"]);
                        Debug.Log(resultData["nfts"][i]["nft_category"]);
                        Debug.Log(resultData["nfts"][i]["nft_sub_category"]);
                        Debug.Log("--------------------");
                        */
                    }
                    itemData=resultData["nfts"];
                    Inventory.itemData=resultData["nfts"];
                    updateListView();
                    request.Dispose();
                }
            }
            yield return new WaitForSeconds(3f);
        }
    }

    private void ItemClicked (int itemIndex)
    {
        Debug.Log ("Item Clicked Index -> " + itemIndex);
        Debug.Log ("NFT Token ID -> " + NFTs [itemIndex].TokenID);
        Debug.Log ("NFT ID -> " + NFTs [itemIndex].NftID);
        Debug.Log ("NFT Family ID -> " + NFTs [itemIndex].FamilyID);
        Debug.Log ("NFT Name -> " + NFTs [itemIndex].Name);
        Debug.Log ("NFT Description -> " + NFTs [itemIndex].Description);
    }

    public void Connect()
    {
        Debug.Log("Trying to connect extension...");
        WebGLPluginJS.Connect();
    }

    public void GetNFTCollections()
    {
        Debug.Log("Getting NFT collections...");
        WebGLPluginJS.GetNFTCollections();
    }

    private async UniTask<Texture2D> GetTexture(string url)
    {
        for (int i=0;i<Texture2DArray.Count;i++)
        {
            if (Texture2DArray[i].id==url)
            {
                return Texture2DArray[i].texture;
                break;
            }
        }

        using (var req = UnityWebRequestTexture.GetTexture(url))
        {
            try
            {
                await req.SendWebRequest();
                Texture2D t=DownloadHandlerTexture.GetContent(req);
                Texture2DArray.Add(new t2d()
                {
                    id=url,
                    texture=t,
                });                
                return t;
            }
            catch (Exception err) {
                Debug.LogError("GetTexture error: " + err);
                return new Texture2D(128, 128);
            }
        }
    }

    public void CheckExtension(string json)
    {
        Debug.Log(json);
        var obj = SimpleJSON.JSON.Parse(json);
        foreach(var kvp in obj)
        {
            Debug.Log("Key:" + kvp.Key + " Value:" + kvp.Value.Value);
            if (kvp.Key=="extension_not_found")
            {
                status.text="Extension not found";
                break;
            }
            if (kvp.Key=="address")
            {
                address.text=kvp.Value.Value;
            }
            if (kvp.Key=="connected"&&kvp.Value.Value.ToString()=="False")
            {
                status.text="Wallet not connected";
            }
            if (kvp.Key=="connected"&&kvp.Value.Value.ToString()=="True")
            {
                status.text="Wallet connected";
            }
        }
    }

    public void Process(string json)
    {
        Debug.Log(json);
        var obj = SimpleJSON.JSON.Parse(json);
        foreach(var kvp in obj)
        {
            Debug.Log("Key:" + kvp.Key + " Value:" + kvp.Value.Value);
            if (kvp.Key=="extension_not_found")
            {
                status.text="Extension not found";
                break;
            }
            if (kvp.Key=="address")
            {
                address.text=kvp.Value.Value;
            }
            if (kvp.Key=="connected"&&kvp.Value.Value.ToString()=="False")
            {
                status.text="Wallet not connected";
            }
            if (kvp.Key=="connected"&&kvp.Value.Value.ToString()=="True")
            {
                status.text="Wallet connected";
            }
        }
    }

    public void ProcessNFTCollections(string json)
    {
        Debug.Log("Processing NFT Collections...");
        Debug.Log(json);
        var obj = SimpleJSON.JSON.Parse(json);
        foreach(var kvp in obj)
        {
            Debug.Log("Key:" + kvp.Key + " Value:" + kvp.Value.Value);
        }
    }

    public void AcceptConnection(string walletAddress)
    {
        Debug.Log("Connection accepted");
        Debug.Log(walletAddress);
        status.text="Connection accepted";
        address.text=walletAddress;
    }

    async void updateListView()
    {
        foreach (Transform child in contentPanel.transform)
        {
             GameObject.Destroy(child.gameObject);
        }
        GameObject NewItem;
        Texture2D _texture=null;
        NFTs.Clear();
        for (int i=0;i<itemData.Count;i++)
        {
            NFTs.Add(new NFT()
            {
                TokenID=(itemData[i]["token_id"]!=null?itemData[i]["token_id"].ToString():""),
                NftID=(itemData[i]["nft_id"]!=null?itemData[i]["nft_id"].ToString():""),
                Name=itemData[i]["name"].ToString(),
                Description=(itemData[i]["description"]!=null?itemData[i]["description"].ToString():""),
                FamilyID=(itemData[i]["family_id"]!=null?itemData[i]["family_id"].ToString():""),
                Category=(itemData[i]["nft_category"]!=null?itemData[i]["nft_category"].ToString():""),
                SubCategory=(itemData[i]["nft_sub_category"]!=null?itemData[i]["nft_sub_category"].ToString():""),
                ImageUrl=itemData[i]["image"].ToString()
            });
        }
        for (int i = 0; i < itemData.Count; i++)
        {
            if (NFTs [i].SubCategory.ToString()==FilterNFTSubCategory||FilterNFTSubCategory=="")
            {
                NewItem = Instantiate (ItemTemplate, contentPanel.transform,false) as GameObject;
                try
                {
                    _texture = (await GetTexture(NFTs [i].ImageUrl));
                    NewItem.transform.GetChild (0).GetComponent <Image> ().sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0, 0));
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                NewItem.transform.GetChild (1).GetComponent <Text> ().text = NFTs [i].Name;
                NewItem.transform.GetChild (2).GetComponent <Text> ().text = NFTs [i].Description;
                NewItem.transform.GetChild (3).GetComponent <Text> ().text = NFTs [i].SubCategory;
                NewItem.transform.GetChild (4).GetComponent <Text> ().text = NFTs [i].FamilyID;
                NewItem.transform.SetParent(contentPanel);
                NewItem.GetComponent <Button> ().AddEventListener (i, ItemClicked);
            }
        }
    }

    void NFTCollections(string json)
    {
        itemData=JsonMapper.ToObject(json);
        Inventory.itemData=itemData;
        Debug.Log("NFT Collections");
        Debug.Log(json);
        Debug.Log(itemData);
        updateListView();
    }
    
    private Color32 [] Encode(string textForEncoding, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height=height,
                Width=width
            }
        };
        return writer.Write(textForEncoding);
    }

    private void EncodeTextQRCode(string textWrite)
    {
        Color32[] _convertPixelToTexture = Encode (textWrite, _storeEncodedTexture.width, _storeEncodedTexture.height);
        _storeEncodedTexture.SetPixels32(_convertPixelToTexture);
        _storeEncodedTexture.Apply();
        _rawImageReceiver.texture=_storeEncodedTexture;
    }

    async void Start()
    {
        NFTCategories.Add(new NFTCategory(){Key="",Title="All"});
        NFTCategories.Add(new NFTCategory(){Key="character_skin",Title="Character Skin"});
        NFTCategories.Add(new NFTCategory(){Key="weapon",Title="Weapon"});
        NFTCategories.Add(new NFTCategory(){Key="weapon_camouflage",Title="Weapon Camouflage"});
        NFTCategories.Add(new NFTCategory(){Key="player_accessory",Title="Player Accessory"});
        NFTCategories.Add(new NFTCategory(){Key="trading_card",Title="Trading Card"});
        NFTCategories.Add(new NFTCategory(){Key="emblem",Title="Emblem"});
        NFTCategories.Add(new NFTCategory(){Key="sticker",Title="Sticker"});
        _storeEncodedTexture= new Texture2D(256,256);
        string code;
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("qr")))
        {
            code=GenerateRandomAlphaNumericStr();
            PlayerPrefs.SetString("qr", code);
            Debug.Log("Set QR code to PlayerPrefs -> " + code);
        }
        else
        {
            code=PlayerPrefs.GetString("qr");
            Debug.Log("Get QR code from PlayerPrefs ->" + code);
        }
        EncodeTextQRCode(ProjectID+":"+code);
        StartCoroutine(GetNFTsWithQRCode(code));
        /*
        JsonString=File.ReadAllText(Application.dataPath+"/Resources/items.json");
        itemData=JsonMapper.ToObject(JsonString);
        Inventory.itemData=itemData;
        Debug.Log(JsonString);
        updateListView();
        */
        DropdownCategories.ClearOptions();
        List<string>Items = new List<string>();
        for (int i = 0; i < NFTCategories.Count; i++)
        {
            Items.Add(NFTCategories[i].Title);
        }
        DropdownCategories.AddOptions(Items);
        DropdownCategories.onValueChanged.AddListener(delegate {DropdownItemSelected(DropdownCategories);});
    }
    
    void DropdownItemSelected(TMP_Dropdown DropdownCategories)
    {
        int index=DropdownCategories.value;
        if (itemData==null) return;
        FilterNFTSubCategory=NFTCategories[index].Key;
        updateListView();
    }
    
    private void EnterGame()
    {
        Debug.Log("Entering game...");
        SceneManager.LoadScene("Main");        
    }

    private void Quit()
    {
        Application.Quit();
    }
}