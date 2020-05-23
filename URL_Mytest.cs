using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//============================================
using UnityEngine.Networking;
using UnityEngine.UI; //raw image
using SimpleJSON;
using TMPro; //TextmeshproGUI

public class URL_Mytest : MonoBehaviour
{
    float time = 0;
    bool timer = true;

    readonly string UbikeURL = "http://opendata2.epa.gov.tw/AQI.json";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer == true)
        {
            StartCoroutine(test());
            timer = false;
            //Debug.Log(time);
        }
    }

    IEnumerator test()
    {
        //=========Get Data=========
        //讀取jason檔案的資訊
        UnityWebRequest ubikeInfoRequest = UnityWebRequest.Get(UbikeURL);
        //等待 URL 回傳 Data，當收到data才繼續執行下面程式碼，這是yield return的特性。
        yield return ubikeInfoRequest.SendWebRequest();
        //當回傳值有錯誤，則debug，並且跳出Coroutine，不繼續執行後續。
        if (ubikeInfoRequest.isNetworkError || ubikeInfoRequest.isHttpError)
        {
            Debug.LogError(ubikeInfoRequest.error);
            yield break;
        }
        //下載剛剛的jason data，為string
        JSONNode UbikeInfo = JSON.Parse(ubikeInfoRequest.downloadHandler.text);
        for (int i = 0; i < UbikeInfo.Count; i++)
        {
            string ubikeName = UbikeInfo[i]["County"];
            print(ubikeName);
        }
        //array count
        print(UbikeInfo.Count);
        yield return new WaitForSeconds(10);
        time++;
        timer = true;
    }
}
