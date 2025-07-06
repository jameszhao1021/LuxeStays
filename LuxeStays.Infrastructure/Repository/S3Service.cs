using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxeStays.Infrastructure.Repository
{
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using LuxeStays.Application.Common.Interfaces;
    using Microsoft.Extensions.Configuration;

    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration)
        {
            _bucketName = Environment.GetEnvironmentVariable("S3_BUCKET");

            _s3Client = new AmazonS3Client(
                Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
                Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"),
                Amazon.RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("S3_BASE_URL")));
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = uniqueFileName,
                BucketName = _bucketName,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            string fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{uniqueFileName}";
            return fileUrl;
        }
    }

}
