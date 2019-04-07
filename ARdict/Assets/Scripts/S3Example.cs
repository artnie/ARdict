using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;

namespace AWSSDK.Examples
{
    public class S3Example : MonoBehaviour
    {
        public string IdentityPoolId = "";
        public string CognitoIdentityRegion = RegionEndpoint.EUWest1.SystemName;
        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        public string S3Region = RegionEndpoint.EUWest1.SystemName;
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(S3Region); }
        }
        public string S3BucketName = null;



        private Texture2D screenShot;
        public GameObject ArCamera;
        public GameObject plane;
        public Text debugger;
        public AWSRestCall awsHandler;
        void Start()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);

            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
            awsHandler.init();

            StartCoroutine(TakePhoto());
        }


        IEnumerator TakePhoto()
        {

            //   Texture mainTex = ArCamera.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture;
            yield return new WaitForSeconds(2);

            while (true)
            {
                StartCoroutine(awsHandler.UpdateFrame());
                // Texture2D texture2D = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGB24, true);
                // texture2D.SetPixels(((Texture2D)mainTex).GetPixels());
                // texture2D.Apply();              

                // plane.GetComponent<Renderer>().material.mainTexture = texture2D;
                // yield return new WaitForSeconds(2);
                // byte[] bytes = texture2D.EncodeToJPG();
                //Write out the PNG. Of course you have to substitute your_path for something sensible

                string ScreenshotName = "/photo.jpg";
                string screenShotPath = Application.persistentDataPath + ScreenshotName;
    


                ScreenCapture.CaptureScreenshot(screenShotPath);
                debugger.text="taking ss";

                yield return new WaitForSeconds(2);
                debugger.text=screenShotPath;
                PostObject( screenShotPath);


                // PostObject( "/storage/emulated/0/Android/data/com.moh.ar/files/photo.jpg");

            }

        }
        #region private members

        private IAmazonS3 _s3Client;
        private AWSCredentials _credentials;

        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials("eu-west-1:80b3455b-ccf5-4e10-b5c4-68781493dedb", _CognitoIdentityRegion);
                return _credentials;
            }
        }

        private IAmazonS3 Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(Credentials, _S3Region);
                }
                //test comment
                return _s3Client;
            }
        }

        #endregion



        /// <summary>
        /// Post Object to S3 Bucket. 
        /// </summary>
        public void PostObject(String fileName)
        {
            debugger.text="Retrieving the file";
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            debugger.text=" Creating request object";
            var request = new PostObjectRequest()
            {
                Bucket = S3BucketName,
                Key = fileName,
                InputStream = stream,
                CannedACL = S3CannedACL.Private,
                Region = _S3Region

            };

            debugger.text=" Making HTTP post call";

            Client.PostObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                {
                    print(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
                }
                else
                {
                    print("\nException while posting the result object");
                    print(string.Format("\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString()));
                }
            });
        }





        #region helper methods



        private string GetPostPolicy(string bucketName, string key, string contentType)
        {
            bucketName = bucketName.Trim();

            key = key.Trim();
            // uploadFileName cannot start with /
            if (!string.IsNullOrEmpty(key) && key[0] == '/')
            {
                throw new ArgumentException("uploadFileName cannot start with / ");
            }

            contentType = contentType.Trim();

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentException("bucketName cannot be null or empty. It's required to build post policy");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("uploadFileName cannot be null or empty. It's required to build post policy");
            }
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType cannot be null or empty. It's required to build post policy");
            }

            string policyString = null;
            int position = key.LastIndexOf('/');
            if (position == -1)
            {
                policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
                    bucketName + "\"},[\"starts-with\", \"$key\", \"" + "\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
            }
            else
            {
                policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
                    bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring(0, position) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
            }

            return policyString;
        }

    }

    #endregion
}
