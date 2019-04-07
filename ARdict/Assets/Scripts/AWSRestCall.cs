using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;


public class AWSRestCall : MonoBehaviour
{
    string id_token = "";
    string label = "Nothing";

    string[] lang = { "en", "de", "da", "fr", "es" };
    int targetLang = 1;
    string sourceLang = "auto";
    string translatedLabel = "Nothing";
    public GameObject AnimatorFlags;
    private bool isHidden;
    public TextMesh labelOrigin;
    [Serializable]
    public class TokenResponseJson
    {
        public string id_token;
        public string status;
    }

    [Serializable]
    public class RekognitionLabelsJson
    {
        public Label[] Labels;
    }

    [Serializable]
    public class Label
    {
        public string Name;
        public float Confidence;
        public Instance[] Instances;
    }

    [Serializable]
    public class Instance
    {
        public Dimension BoundingBox;
        public float Confidence;
    }

    [Serializable]
    public class Dimension
    {
        public float Height;
        public float Width;
        public float Top;
        public float Left;
    }
    Hashtable translationCache = new Hashtable();
	
    public void init()
    {
        StartCoroutine(Wait());
        isHidden=false;
        print("jsonResponse");
    }

    IEnumerator Wait()
    {
        StartCoroutine(SetupIdToken());
        yield return new WaitForSeconds(1);


    }
    public IEnumerator UpdateFrame()
    {
        StartCoroutine(GetRekognisedLabels());
        yield return null;
        StartCoroutine(GetTranslation(sourceLang, lang[targetLang], label));
    }

    IEnumerator SetupIdToken()
    {
        byte[] body = System.Text.Encoding.UTF8.GetBytes("{\"username\":\"testuser\", \"password\":\"testpassword\"}");
        using (UnityWebRequest request = UnityWebRequest.Put("https://rmw6h8h7y6.execute-api.eu-west-1.amazonaws.com/prod/authenticate", body))
        {
            yield return request.Send();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                TokenResponseJson json_obj = JsonUtility.FromJson<TokenResponseJson>(request.downloadHandler.text);
                Debug.Log("Response Status: " + request.error);
                Debug.Log("Token set!");
                id_token = json_obj.id_token;
                print(request.downloadHandler.text);
            }
        }
    }

    string authenticate(string token)
    {
        string auth = token;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("eyJraWQiOiJBQmtGeVRkNzBab1lJTVRlbUx4czlZMWthV010MkxIWTBUR041K1wvRGVUUT0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJhNjgyYTI3Ny1mYjM0LTQ3OWMtOTc3Zi1jOTk1NTVlODMwYjYiLCJhdWQiOiI1MXFma2hxaTYzMGVlNmU1djVlMDc1NGhzdCIsImV2ZW50X2lkIjoiNWU2NDQ1MjgtNTg5ZC0xMWU5LTgxMTEtY2I5OWFiMDAxMGZjIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1NTQ1NzY4NTMsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS13ZXN0LTEuYW1hem9uYXdzLmNvbVwvZXUtd2VzdC0xXzhZVzB0WUZvOSIsImNvZ25pdG86dXNlcm5hbWUiOiJ0ZXN0dXNlciIsImV4cCI6MTU1NDU4MDQ1MywiaWF0IjoxNTU0NTc2ODUzfQ.K5dhFi0TMDptUJhpwTvqCEqNDVSOsKYT3ECPVUy3r7uaSzXqSPso7ywsgUrOcHm-mYaG817Z5lfvJ2D2IGIz34gSY7RS-gNxinDkyPuOf7BUtmvbmylC8SkQwkuk1SHnF2eM_KQOB9XiarvJ_AwfuYZidnEUPFXiogKzbmdEaaGa4ve1DNPFB9LRhATWc5ZTQ6X_TWHwIA1wi3M8n9dq8Vhs4panDHsXE0PFp1fIrNk00q3_k7n2h1j3lMV56sH3S1VlVc6WPH2pMTupmjRMkqwXbkbzrZrk3n_-QQD17WVCFpmS8LXp8rwZzcxz6WU1vZ6hu-HP8PvdgwPUqpoArQ"));
        return auth;
    }

    IEnumerator GetRekognisedLabels()
    {
        string authorization = id_token;//authenticate(id_token);
        print("autorization token: " + authorization);
        string url = "https://72nzjn85fk.execute-api.eu-west-1.amazonaws.com/prod/ping";
        string json_string = "{\"body-json\": \"bucket:trialimagesar\",\"key:/storage/emulated/0/Android/data/com.moh.ar/files/photo.jpg\"}}";
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json_string);

        WWWForm form = new WWWForm();

        // this is not used anymore, bucket and key are now hardcoded in the AWS Lambda function 
        // form.AddField("bucket", "trialimagesar");
        // //form.AddField("key", "/storage/emulated/0/Android/data/com.moh.ar/files/photo.jpg");
        // form.AddField("key", "/Users/mohamedaboughazala/Library/Application Support/DefaultCompany/HelloAR U3D/photo.jpg");

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.SetRequestHeader("Authorization", authorization);
            request.SetRequestHeader("content-type", "application/json");
            yield return request.Send();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error + ": " + request.downloadHandler.text);
            }
            else
            {
                RekognitionLabelsJson json_obj = JsonUtility.FromJson<RekognitionLabelsJson>(request.downloadHandler.text);
                Debug.Log("Response: " + request.downloadHandler.text);
                Debug.Log("Got labels!");
                label = json_obj.Labels[0].Name;

            }
        }
    }

    [Serializable]
    public class TranslateResult { public object[] data; }
    [Serializable]
    public class TranslateData { public Translation[] translations; }
    [Serializable]
    public class Translation { public string translatedText; }

       IEnumerator GetTranslation(string srcLang, string tarLang, string sourceText)
    {
        if (translationCache.ContainsKey(sourceText + tarLang))
        {
            translatedLabel = (string)translationCache[sourceText + tarLang];
            print(label + ": " + translatedLabel);
            yield return null;
        }
        else
        {
            string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                srcLang, tarLang, sourceText);
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.Send();
                
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error + ": " + request.downloadHandler.text);
                }
                else
                {
                    string translation = (string)request.downloadHandler.text;
                    translation = translation.Substring(translation.IndexOf("\"")+1);
                    translation = translation.Substring(0, translation.IndexOf("\""));
                    //translation = translation.Substring(0, translation.IndexOf("\"")+1);
                    translatedLabel = translation;
                    translationCache.Add(sourceText + tarLang, translation);
                    print(label + ": " + translatedLabel);
                }
            }
        }
    }
    public void ChangeToEN()
    {
        targetLang = 0;
    }
    public void ChangeToDE()
    {
        targetLang = 1;

    }
    public void ChangeToDA()
    {
        targetLang = 2;

    }
    public void ChangeToFr()
    {
        targetLang = 3;

    }
    public void ChangeToEs()
    {
        targetLang = 4;

    }
    public void ControlFlags(){
        if(isHidden){
            showsFlags();
        }
        else{
            hideFlags();
        }
    }
    public void showsFlags(){
        AnimatorFlags.GetComponent<Animator>().Play("HideFlag");
                isHidden=false;

    }
    public void hideFlags(){
        AnimatorFlags.GetComponent<Animator>().Play("ShowFlag");
        isHidden=true;

    }
}
