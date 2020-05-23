using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//============================================
using UnityEngine.Networking;
using UnityEngine.UI; //raw image
using SimpleJSON;
using TMPro; //TextmeshproGUI

public class PokemonAPI : MonoBehaviour
{
    public RawImage pokeRawImage;
    public TextMeshProUGUI pokeNameText, pokeNumText;
    public TextMeshProUGUI[] pokeTypeTextArray;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    // Start is called before the first frame update
    void Start()
    {
        pokeRawImage.texture = Texture2D.blackTexture;

        pokeNameText.text = "";
        pokeNumText.text = "";

        foreach(var pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text = "";
        }
    }
    //function
    public void onBottonRandomPokemon()
    {
        int randomPokemon = Random.Range(1, 808);

        pokeRawImage.texture = Texture2D.blackTexture;

        pokeNameText.text = "Loding...";
        pokeNumText.text = "#" + randomPokemon;

        foreach (var pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text = "";
        }

        StartCoroutine(GetPokemonAtIndex(randomPokemon));
    }

    IEnumerator GetPokemonAtIndex(int pokemonIndex)
    {
        //Get Pokemon Info
        string pokemonURL = basePokeURL + "pokemon/" + pokemonIndex.ToString();
        //Example URL :  https://pokeapi.co/api/v2/pokemon/151

        //讀取jason檔案的資訊
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);
        //等待 URL 回傳 Data，當收到data才繼續執行下面程式碼，這是yield return的特性。
        yield return pokeInfoRequest.SendWebRequest();
        //當回傳值有錯誤，則debug，並且跳出Coroutine，不繼續執行後續。
        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }
        //下載剛剛的jason data，為string
        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);
        //pokeInfo為jason所有的字串，從裡面去找到需要的Title。
        string pokeName = pokeInfo["name"];
        //此為找到sprites資料夾裡的front_default資料夾中的資訊，有層級關係
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];
        //為了再去找層級所以使用 JSONNode 型式
        JSONNode pokeTypes = pokeInfo["types"];
        //types中又有兩個items，就可以得到Count = 2，所以先增一個空的字串list，等等將資料分別裝起來。
        string[] pokeTypeNames = new string[pokeTypes.Count];
        //調換關係，並且把最終結果裝在list裡。因為type1是主要，type0是次要，希望對調(這個是作者的想法，可不用對調)      
        for(int i = 0, j = pokeTypes.Count -1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }

        //Get Pokemon Sprite
        //剛剛收到的sprite data，是另一個URL，是圖片的所在位置。所以要再次從URL下載。
        //讀取圖片檔案的資訊
        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);
        //等待 URL 回傳 Data，當收到data才繼續執行下面程式碼，這是yield return的特性。
        yield return pokeSpriteRequest.SendWebRequest();
        //當回傳值有錯誤，則debug，並且跳出Coroutine，不繼續執行後續。
        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        //Set UI Object
        //Download texture form URL，並套用在我們image上面，達到顯示。
        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        //抗鋸齒功能
        pokeRawImage.texture.filterMode = FilterMode.Point;

        pokeNameText.text = CapitalizeFirstLetter(pokeName);

        for(int i = 0; i < pokeTypeNames.Length; i++)
        {
            pokeTypeTextArray[i].text = CapitalizeFirstLetter(pokeTypeNames[i]);
        }
    }
    //char為字符，可判斷，也可改寫。
    //ToUpper(要變大寫的字)
    private string CapitalizeFirstLetter(string str)
    {
        //回傳大寫字母 + 去除第一個字母之外的字母。因「不能重複」所以刪掉第一個字母。
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
