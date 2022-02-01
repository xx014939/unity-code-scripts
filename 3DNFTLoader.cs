using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;

namespace BNG
{
    public class ModelLoader : MonoBehaviour
    {
        GameObject wrapper;
        string filePath;
        public string[] imgUrlArray;
        public int nftNumber;
        public string allUrls;
        public string latestThreedURL;
        private float update;

        public class Wallet
        {
            public string _id;
            public string walletAddress;
            public string[] img_url;
            public string[] threed_img_url;
            public int __v;
        }

        public class CoroutineWithData
        {
            public Coroutine coroutine { get; private set; }
            public object result;
            private IEnumerator target;
            public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
            {
                this.target = target;
                this.coroutine = owner.StartCoroutine(Run());
            }

            private IEnumerator Run()
            {
                while (target.MoveNext())
                {
                    result = target.Current;
                    yield return result;
                }
            }
        }

        private void Start()
        {
            filePath = $"{Application.persistentDataPath}/Files/";
            wrapper = new GameObject
            {
                name = "Test Model from IPFS"
            };


            // DownloadFile("https://ipfs.io/ipfs/QmbqbZ32qXUuLdVoMY7EeKqQPDHWaSCnQet25ndog3TJ4K?filename=QmbqbZ32qXUuLdVoMY7EeKqQPDHWaSCnQet25ndog3TJ4K.gltf");

            //Debug.Log("testing return value" + StartCoroutine(ProcessRequest("http://localhost:3000/subscribers/61eda429f894af88ad7adef2")));
            //StartCoroutine(ProcessRequest("http://localhost:3000/subscribers/61eda429f894af88ad7adef2"));

            Debug.Log("This is the static variable -->" + saveWalletID.walletID);
            string expressURL = "https://tranquil-bayou-15552.herokuapp.com/subscribers/" + saveWalletID.walletID;
            StartCoroutine(ProcessRequest(expressURL));

        }

        public void DownloadFile(string url)
        {
            string path = GetFilePath(url);
            if (File.Exists(path))
            {
                Debug.Log("Found file locally, loading...");
                LoadModel(path);
                return;
            }

            StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
            {
                if (req.isNetworkError || req.isHttpError)
                {
                    // Log any errors that may happen
                    Debug.Log($"{req.error} : {req.downloadHandler.text}");
                }
                else
                {
                    // Save the model into a new wrapper
                    LoadModel(path);
                }
            }));
        }

        string GetFilePath(string url)
        {
            string[] pieces = url.Split('=');
            string filename = pieces[pieces.Length - 1];

            return $"{filePath}{filename}";
        }

        void LoadModel(string path)
        {
            ResetWrapper();
            GameObject model = Importer.LoadFromFile(path);
            model.transform.position = new Vector3(5, 3, 5);
            model.AddComponent<Rigidbody>();
            model.AddComponent<BoxCollider>();
            model.AddComponent<Grabbable>();
            Debug.Log("found successfully");
        }

        IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        void ResetWrapper()
        {
            if (wrapper != null)
            {
                foreach (Transform trans in wrapper.transform)
                {
                    Destroy(trans.gameObject);
                }
            }
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
                    Debug.Log(user.threed_img_url[0]);
                    Debug.Log(user.__v);


                    //nftNumber = user.threed_img_url.Length - 1;
                    //latestThreedURL = user.threed_img_url[];


                    allUrls = user.threed_img_url[0];
                    imgUrlArray = allUrls.Split(',');

                    if (imgUrlArray.Length > 0)
                    {
                        nftNumber = imgUrlArray.Length - 1;
                    }

                    latestThreedURL = imgUrlArray[nftNumber];
                    Debug.Log("The URL is -->" + latestThreedURL);

                }

                DownloadFile(latestThreedURL.ToString());
            }
        }

    }
}
