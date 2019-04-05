using System.Collections;
//using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


public class AWSRestCall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    	/*var api = "https://jsonplaceholder.typicode.com";
		RestClient.GetArray<Post>(api + "/posts", (err, res) => {
      		RestClient.GetArray<Todo>(api + "/todos", (errTodos, resTodos) => {
    			RestClient.GetArray<User>(api + "/users", (errUsers, resUsers) => {
      			//Missing validations to catch errors!
    				});
  				});
			});*/
		/*var api = "https://jsonplaceholder.typicode.com";
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();*/
        var api = "https://jsonplaceholder.typicode.com";
        StartCoroutine(GetRequest(api));
        //StartCoroutine(PostRequest(GenerateRequestURL(lastRequestURL, lastRequestParameters, "POST"), JSON_body));
		print("jsonResponse"); 
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                print(webRequest.downloadHandler.text);
            }
        }
    }

	IEnumerator PostRequest(string url, string bodyJsonString)
	{
		bool requestFinished = false;
	    bool requestErrorOccurred = false;


	    var request = new UnityWebRequest(url, "POST");
	    byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);

	    request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
	    request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
	    request.SetRequestHeader("Content-Type", "application/json");

	    yield return request.Send();
	    requestFinished = true;

	    if (request.isError)
	    {
	        Debug.Log("Something went wrong, and returned error: " + request.error);
	        requestErrorOccurred = true;
	    }
	    else
	    {
	        Debug.Log("Response: " + request.downloadHandler.text);

			if (request.responseCode == 201)
	        {
	            Debug.Log("Request finished successfully! New User created successfully.");
	        }
	        else if (request.responseCode == 401)
	        {
	            Debug.Log("Error 401: Unauthorized. Resubmitted request!");
	            requestErrorOccurred = true;
	        }
	    }
	}

	string authenticate(string username, string password)
	{
	    string auth = username + ":" + password;
	    auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
	    auth = "Basic " + auth;
	    return auth;
	}

	IEnumerator makeRequest()
	{
	    string authorization = authenticate("YourUserName", "YourPassWord");
	    string url = "yourUrlWithoutUsernameAndPassword";


	    UnityWebRequest www = UnityWebRequest.Get(url);
	    www.SetRequestHeader("AUTHORIZATION", authorization);

	    yield return www.Send();
	    
	}

/*
	IEnumerator PostRequest(string url, string bodyJsonString)
	{
	    requestFinished = false;
	    requestErrorOccurred = false;

	    var request = new UnityWebRequest(url, "POST");
	    byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
	    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
	    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
	    request.SetRequestHeader("Content-Type", "application/json");

	    yield return request.Send();
	    requestFinished = true;

	    if (request.isError)
	    {
	        Debug.Log("Something went wrong, and returned error: " + request.error);
	        requestErrorOccurred = true;
	    }
	    else
	    {
	        Debug.Log("Response: " + request.downloadHandler.text);

	        if (request.responseCode == 201)
	        {
	            Debug.Log("Request finished successfully! New User created successfully.");
	        }
	        else if (request.responseCode == 401)
	        {
	            Debug.Log("Error 401: Unauthorized. Resubmitted request!");
	            StartCoroutine(PostRequest(GenerateRequestURL(lastRequestURL, lastRequestParameters, "POST"), bodyJsonString));
	            requestErrorOccurred = true;
	        }
	        else
	        {
	            Debug.Log("Request failed (status:" + request.responseCode + ").");
	            requestErrorOccurred = true;
	        }

	        if (!requestErrorOccurred)
	        {
	            yield return null;
	            // process results
	        }
	    }
	}
*/
    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");

        using (UnityWebRequest www = UnityWebRequest.Post("http://www.my-server.com/myform", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    // IEnumerator PostRequest(string url, string bodyJsonString)
    // {
    //     bool requestFinished = false;
    //     bool requestErrorOccurred = false;

    //     var request = new UnityWebRequest(url, "POST");
    //     byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
    //     request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    //     request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //     request.SetRequestHeader("Content-Type", "application/json");

    //     yield return request.Send();
    //     requestFinished = true;

    //     if (request.isNetworkError)
    //     {
    //         Debug.Log("Something went wrong, and returned error: " + request.error);
    //         requestErrorOccurred = true;
    //     }
    //     else
    //     {
    //         Debug.Log("Response: " + request.downloadHandler.text);

    //         if (request.responseCode == 201)
    //         {
    //             Debug.Log("Request finished successfully! New User created successfully.");
    //         }
    //         else if (request.responseCode == 401)
    //         {
    //             Debug.Log("Error 401: Unauthorized. Resubmitted request!");
    //             StartCoroutine(PostRequest(GenerateRequestURL(lastRequestURL, lastRequestParameters, "POST"), bodyJsonString));
    //             requestErrorOccurred = true;
    //         }
    //         else
    //         {
    //             Debug.Log("Request failed (status:" + request.responseCode + ").");
    //             requestErrorOccurred = true;
    //         }

    //         if (!requestErrorOccurred)
    //         {
    //             yield return null;
    //             // process results
    //         }
    //     }
    // }
    // Update is called once per frame
    void Update()
    {
        
    }
}
