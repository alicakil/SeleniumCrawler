using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Dashboard.Models;
using System.Drawing;

namespace Dashboard.Services
{
    public class S3
    {
        AmazonS3Client client;

        public static string Folder_AccountImageProfile = "account/Image/profile/";
        public static string Folder_Dir_EventImage = "event/image/";


        string Root = "";

        public S3(string root)
        {
            Console.WriteLine("S3 Initialize");
            Root = root;
            client = new AmazonS3Client(new BasicAWSCredentials(AppConstants.AWS.accessKey, AppConstants.AWS.secretKey), Amazon.RegionEndpoint.EUNorth1);
            Console.WriteLine("S3.Initialized");
        }

        public string GetUniqueName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }



        public bool GetBase64FromUrl(string url, out string base64, out double aspectRatio)
        {
            try
            {
                Console.WriteLine("S3.GetBase64FromUrl");
                using (var httpClient = new HttpClient())
                {
                    var imageBytes = httpClient.GetByteArrayAsync(url).GetAwaiter().GetResult();
                    base64 = Convert.ToBase64String(imageBytes);

                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        Image image = Image.FromStream(ms);
                        aspectRatio = (float)image.Width / image.Height;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                base64 = ex.Message;
                aspectRatio = 0;
                return false;
            }

        }

        public void SaveBase64File(string LocalPath, string Base64)
        {
            try
            {
                Console.WriteLine("SaveBase64File : " + LocalPath);
                var dir = Path.GetDirectoryName(LocalPath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                byte[] bytes = Convert.FromBase64String(Base64);
                System.IO.File.WriteAllBytes(LocalPath, bytes);
                Console.WriteLine("Image Saved to Local path");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ! : SaveBase64File! LocalPath:" + LocalPath + " Error : " + ex.Message);
            }
        }

        public bool UploadFile(string LocalPath, string S3Path)
        {
            TransferUtility utility = new TransferUtility(client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
            request.StorageClass = S3StorageClass.Standard;
            request.Key = S3Path;
            request.FilePath = LocalPath;
            request.BucketName = AppConstants.AWS.PublicBucket;
            utility.Upload(request);
            return true;
        }

        public void UploadBase64(string FileName, string base64String)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = AppConstants.AWS.PublicBucket;
            request.Key = FileName;
            request.ContentType = "text/plain";
            request.ContentBody = base64String;
            client.PutObjectAsync(request);
        }

        public async Task<bool> Download(string objectName)
        {
            // Create a GetObject request
            var request = new GetObjectRequest
            {
                BucketName = AppConstants.AWS.PublicBucket,
                Key = objectName,
            };

            try
            {
                // Issue request and remember to dispose of the response
                using GetObjectResponse response = await client.GetObjectAsync(request);

                // Save object to local file
                await response.WriteResponseStreamToFileAsync($"{Root}\\{objectName.Replace(@"\", "/")}", true, CancellationToken.None);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error saving {objectName}: {ex.Message}");
                return false;
            }
        }


        public static async Task<bool> PutS3Object(string bucket, string key, string content)
        {
            try
            {
                using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucket,
                        Key = key,
                        ContentBody = content
                    };
                    var response = await client.PutObjectAsync(request);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in PutS3Object:" + ex.Message);
                return false;
            }
        }


        public async void ListObjects()
        {
            ListObjectsRequest request = new ListObjectsRequest();
            request.BucketName = AppConstants.AWS.PublicBucket;
            ListObjectsResponse response = await client.ListObjectsAsync(request);
            foreach (S3Object o in response.S3Objects)
            {
                Console.WriteLine("{0}\t{1}\t{2}", o.Key, o.Size, o.LastModified);
            }
        }


    }
}
