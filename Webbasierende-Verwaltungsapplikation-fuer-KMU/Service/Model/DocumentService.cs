using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class DocumentService : BaseService
    {
        private protected BlobService _blob;
        readonly AuthService _authService;
        public DocumentService(BlobService blob, Database db, AuthService authService) : base(db)
        {
            _blob = blob;
            _authService = authService;
        }

        /// <summary>
        /// Method creates a <see cref="Document"/>, uploads it to the cloud and then returns it.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Returns a <see cref="Document"/> which has not yet been saved in the Database.</returns>
        public async Task<Document> AddAsync<T>(IFormFile file, T obj, DocumentType dt)
        {
            MemoryStream ms = new();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            await AddAsync<T>(ms, file.FileName, obj, dt);
            return await _blob.UploadAsync(file, DocumentType.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="FileName"></param>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<Document> AddAsync<T>(MemoryStream file, String FileName, T obj, DocumentType documentType)
        {
            var document = await _blob.UploadAsync(file, FileName, documentType);
            if (obj is Order) { document.OrderId = (obj as Order)?.Id; }
            else if (obj is Ticket) { document.TicketId = (obj as Ticket)?.Id; }
            else if (obj is Message) { document.MessageId = (obj as Message)?.Id; }
            else { throw new ApplicationException("Document konte nicht hinzugefügt werden."); }
            await base.AddAsync<Document>(document);
            return document;
        }

        public async Task<bool> DeleteAsync(Document document)
        {
            if (document.DocumentType == DocumentType.Invoice) throw new ApplicationException("Eine Rechnung darf nicht gelöscht werden!");
            if (document.DocumentType == DocumentType.CompanyIntern && !_authService.IsEmployee) throw new ApplicationException("Interne Dokumente dürfen nur durch Mitarbeiter gelöscht werden.");
            await _blob.DeleteAsync(document);
            await base.DeleteAsync<Document>(document);
            return true;
        }

        public async Task<Stream> GetDocumentAsync(Document document) => await _blob.GetDocument(document);

    }
}
