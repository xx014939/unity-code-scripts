using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class getJson : MonoBehaviour
{

    public GameObject wallFrame1, wallFrame2, wallFrame3, wallFrame4, orb1, orb2, orb3, block1, block2, block3;
    public List<GameObject> nftFrames;
    public List<GameObject> nftOrbs;
    public List<GameObject> nftBlocks;

    public int nftNumber;

    public string[] seperators;
    public string allUrls;
    public string[] imgUrlArray;

    [System.Serializable]
    public class Wallet 
    {
        public string _id;
        public string walletAddress;
        public string[] img_url;
        public int __v;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Populate object arrays
        nftOrbs.Add(orb1);
        nftOrbs.Add(orb2);
        nftOrbs.Add(orb3);

        nftBlocks.Add(block1);
        nftBlocks.Add(block2);
        nftBlocks.Add(block3);

        nftFrames.Add(wallFrame1);
        nftFrames.Add(wallFrame2);
        nftFrames.Add(wallFrame3);
        nftFrames.Add(wallFrame4);

        // Four wall frames in total (in case user has more than 4 NFT's)
        nftNumber = nftFrames.Count;

        string userURL = "https://tranquil-bayou-15552.herokuapp.com/subscribers/" + saveWalletIDMain.walletID;
        Debug.Log("The correct URL is here --> " + userURL);

        // The testing ID is - 61d397c75c65b25c35285992

        StartCoroutine(ProcessRequest(userURL));
       // StartCoroutine(GetTexture(imgUrlArray[0], wallFrame1.transform.name));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                var responseJson = request.downloadHandler.text;
                Wallet user = JsonUtility.FromJson<Wallet>(responseJson);
                Debug.Log(user);
                Debug.Log(user._id);
                Debug.Log(user.walletAddress);
                Debug.Log(user.img_url[0]);
                Debug.Log(user.__v);

                
                allUrls = user.img_url[0];
                imgUrlArray = allUrls.Split(',');

                if (imgUrlArray.Length < nftNumber) {
                    nftNumber = imgUrlArray.Length;
                }

                for (int i = 0; i < nftNumber; i++) 
                {
                  if (imgUrlArray[i] != "" )
                    {
                        Debug.Log("The URL being passed to Unity is -->" + imgUrlArray[i]);
                        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgUrlArray[i]);
                        yield return www.SendWebRequest();
                        Texture myTexture = DownloadHandlerTexture.GetContent(www);
                        GameObject.Find(nftFrames[i].name).GetComponent<Renderer>().material.SetTexture("_BaseColorMap", myTexture);
                        GameObject.Find(nftOrbs[i].name).GetComponent<Renderer>().material.SetTexture("_BaseColorMap", myTexture);
                        GameObject.Find(nftBlocks[i].name).GetComponent<Renderer>().material.SetTexture("_BaseColorMap", myTexture);
                        Debug.Log("Successful Texture");
                    }
                  else
                    {
                        Debug.Log("The URL being passed to Unity is (empty) -->" + imgUrlArray[i]);
                        UnityWebRequest wwwTwo = UnityWebRequestTexture.GetTexture(imgUrlArray[i + 1]);
                        yield return wwwTwo.SendWebRequest();
                        Texture myTextureTwo = DownloadHandlerTexture.GetContent(wwwTwo);
                        GameObject.Find(nftFrames[i].name).GetComponent<Renderer>().material.SetTexture("_BaseColorMap", myTextureTwo);
                        GameObject.Find(nftOrbs[i].name).GetComponent<Renderer>().material.SetTexture("_BaseColorMap", myTextureTwo);
                        GameObject.Find(nftBlocks[i].name).GetComponent<Renderer>().material.SetTexture("_BaseColorMap", myTextureTwo);
                        Debug.Log("Successful Texture");
                        
                    }

                }

            }
        }
    }

}
