using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service
{
    public class BlobService
    {
        private protected Database _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public BlobService(Database db)
        {
            _db = db;
        }

        /// <summary>
        /// Uploads an <see cref="IFormFile"/> <see cref="object"/> to an Azure Blob Storage. This Function does not upload the <see cref="Document"/> to the <see cref="Database"/>.
        /// </summary>
        /// <param name="file">Represents a file sent with the <see cref="HttpRequest"/>.</param>
        /// <param name="dt"></param>
        /// <param name="blobContainerName"></param>
        /// <returns>Returns an uploaded <see cref="Document"/>.</returns>
        public async Task<Document> UploadAsync(IFormFile file, DocumentType dt)
        {
            MemoryStream ms = new();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            return await UploadAsync(ms, file.FileName, dt);
        }

        /// <summary>
        /// Uploads a <see cref="MemoryStream"/> to an Azure Blob Storage. This Function does not upload the <see cref="Document"/> to the <see cref="Database"/>.
        /// </summary>
        /// <param name="document">Represents a MemoryStream</param>
        /// <param name="blobContainerName"></param>
        /// <param name="dt"></param>
        /// <param name="originalFileName">FileName + FileEnding (Rechnung.pdf)</param>
        /// <returns>Returns an uploaded <see cref="Document"/>.</returns>
        /// <exception cref="ApplicationException">Throws <see cref="ApplicationException"/> if <paramref name="document"/> is empty.</exception>
        public async Task<Document> UploadAsync(Stream file, string originalFileName, DocumentType dt)
        {
            MemoryStream document = new();
            await file.CopyToAsync(document);
            document.Position = 0;
            string connectionString = GetConnectionString();
            var created = DateTime.UtcNow;
            var doc = new Document()
            {
                BlobContainer = GetBlobName(),
                Created = created,
                BlobFileName = Guid.NewGuid().ToString() + created.ToString(),
                OriginalName = Path.GetFileNameWithoutExtension(originalFileName),
                Size = document.Length,
                DocumentType = dt,
                MimeType = Path.GetExtension(originalFileName),
                LastUpdated = created
            };

            try
            {
                CheckBlobContainer(GetBlobName());
                BlobContainerClient container = new(connectionString, GetBlobName());
                BlobClient blob = container.GetBlobClient(doc.BlobFileName);
                await blob.UploadAsync(document);
            }
            catch (Azure.RequestFailedException)
            {
                throw new ApplicationException("Das Dokument konnte nicht hochgeladen werden. Probieren Sie es später erneut!");
            }
            return doc;
        }

        /// <summary>
        /// Gets a <see cref="Document"/> object and delets it from the Azure Blob.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="blobContainerName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Document doc)
        {
            if (!(doc is object)) throw new NullReferenceException($"Paramter{nameof(doc)} is empty or null!");
            string connectionString = GetConnectionString();
            CheckBlobContainer(GetBlobName());
            BlobContainerClient container = new(connectionString, GetBlobName());
            BlobClient blob = container.GetBlobClient(doc.BlobFileName);
            _db.Document.Remove(doc);
            return await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        /// <summary>
        /// Gets a <paramref name="document"/> and returns a <see cref="MemoryStream"/> representing that <paramref name="document"/> in the Blob.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> which the function should get from the Blob.</param>
        /// <returns>Retruns a <see cref="MemoryStream"/> representing the <paramref name="document"/> in the Blob.</returns>
        public async Task<MemoryStream> GetDocument(Document document)
        {
            string connectionString = GetConnectionString();
            try
            {
                MemoryStream stream = new();
                CheckBlobContainer(document.BlobContainer);
                BlobServiceClient client = new(connectionString);
                BlobContainerClient container = new(connectionString, document.BlobContainer);
                BlobClient blob = container.GetBlobClient(document.BlobFileName);
                await blob.DownloadToAsync(stream);
                stream.Position = 0;
                if (stream.Length == 0) throw new ApplicationException($"Das Dokument mit dem Namen '{document.GetFullFileName}' konnte nicht heruntergeladen werden.!");
                return stream;
            }
            catch (Azure.RequestFailedException)
            {
                throw new ApplicationException("Das Dokument konnte aufgrund eines Fehlers nicht heruntergeladen werden!");
            }
        }

        /// <summary>
        /// Delets the Blob Container in the Azure Cloud.
        /// </summary>
        /// <param name="blobContainerName">The name of the azure blob.</param>
        /// <returns>True if the blob was deleted. False if it wasent.</returns>
        public async Task<bool> DeleteBlobContainer()
        {
            string connectionString = GetConnectionString();
            BlobServiceClient client = new(connectionString);
            await client.DeleteBlobContainerAsync(GetBlobName());
            return true;
        }

        /// <summary>
        /// Method checks if a Blob Container exists. If not it creates it.
        /// </summary>
        /// <param name="blobContainerName"></param>
        /// <returns></returns>
        private static void CheckBlobContainer(string blobContainerName = "documents")
        {
            BlobServiceClient client = new(GetConnectionString());
            client.GetBlobContainerClient(blobContainerName).CreateIfNotExists();
        }

        /// <summary>
        /// Function (should) get(s) the connectino string from the config file and returns it.
        /// </summary>
        /// <returns>String representing a Azure Connection string.</returns>
        private protected static string GetConnectionString() => "CONNECTION STRING";
        private protected static string GetBlobName() => "documents";
    }


}
