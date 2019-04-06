using System.Collections;
//using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;
//using UnityEngine.JSONSerializeModule;


public class AWSRestCall : MonoBehaviour
{
    string id_token = "";

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
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
		print("jsonResponse"); 
    }

    IEnumerator Wait()
    {
        StartCoroutine(SetupIdToken());
        yield return new WaitForSeconds(2);
        StartCoroutine(GetRekognisedLabels());
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
        return "eyJraWQiOiJBQmtGeVRkNzBab1lJTVRlbUx4czlZMWthV010MkxIWTBUR041K1wvRGVUUT0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJhNjgyYTI3Ny1mYjM0LTQ3OWMtOTc3Zi1jOTk1NTVlODMwYjYiLCJhdWQiOiI1MXFma2hxaTYzMGVlNmU1djVlMDc1NGhzdCIsImV2ZW50X2lkIjoiNWU2NDQ1MjgtNTg5ZC0xMWU5LTgxMTEtY2I5OWFiMDAxMGZjIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1NTQ1NzY4NTMsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS13ZXN0LTEuYW1hem9uYXdzLmNvbVwvZXUtd2VzdC0xXzhZVzB0WUZvOSIsImNvZ25pdG86dXNlcm5hbWUiOiJ0ZXN0dXNlciIsImV4cCI6MTU1NDU4MDQ1MywiaWF0IjoxNTU0NTc2ODUzfQ.K5dhFi0TMDptUJhpwTvqCEqNDVSOsKYT3ECPVUy3r7uaSzXqSPso7ywsgUrOcHm-mYaG817Z5lfvJ2D2IGIz34gSY7RS-gNxinDkyPuOf7BUtmvbmylC8SkQwkuk1SHnF2eM_KQOB9XiarvJ_AwfuYZidnEUPFXiogKzbmdEaaGa4ve1DNPFB9LRhATWc5ZTQ6X_TWHwIA1wi3M8n9dq8Vhs4panDHsXE0PFp1fIrNk00q3_k7n2h1j3lMV56sH3S1VlVc6WPH2pMTupmjRMkqwXbkbzrZrk3n_-QQD17WVCFpmS8LXp8rwZzcxz6WU1vZ6hu-HP8PvdgwPUqpoArQ";
    }

    IEnumerator GetRekognisedLabels()
    {
        string authorization = id_token;//authenticate(id_token);
        print ("autorization token: " + authorization);
        string url = "https://72nzjn85fk.execute-api.eu-west-1.amazonaws.com/prod/ping";
        string json_string = "{\"body-json\": \"bucket:trialimagesar\",\"key:/storage/emulated/0/Android/data/com.moh.ar/files/photo.jpg\"}}";
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json_string);

        WWWForm form = new WWWForm();
        
        form.AddField("bucket", "trialimagesar");
        form.AddField("key", "/storage/emulated/0/Android/data/com.moh.ar/files/photo.jpg");
        
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
                Debug.Log("Got labels! -> " + json_obj.Labels[0].Name);
                
            }
        }
    }

    void Update()
    {
        
    }
}
