using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace AsyncAwaitBestPractices
{
    public static class DocumentDbService
    {
        #region Constant Fields
        static readonly DocumentClient _readonlyClient = new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadOnlyPrimaryKey);
        static readonly Uri _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(PersonModel.DatabaseId, PersonModel.CollectionId);
        #endregion

        #region Fields
        static int _networkIndicatorCount = 0;
        #endregion

        #region Methods
        public static Task<List<T>> GetAll<T>() where T : CosmosDbModel<T> =>
            Task.Run(() => _readonlyClient.CreateDocumentQuery<T>(_documentCollectionUri).Where(x => x.TypeName.Equals(typeof(T).Name))?.ToList());

        public static async Task<T> Get<T>(string id)
        {
            var result = await _readonlyClient.ReadDocumentAsync<T>(CreateDocumentUri(id)).ConfigureAwait(false);

            if (result.StatusCode != HttpStatusCode.Created)
                return default;

            return result;
        }

        public static Task<ResourceResponse<Document>> Update<T>(T document) where T : CosmosDbModel<T>
        {
            var documentClient = GetReadWriteDocumentClient();

            return documentClient?.ReplaceDocumentAsync(CreateDocumentUri(document.Id), document);
        }

        public static Task<ResourceResponse<Document>> Create<T>(T document) where T : CosmosDbModel<T>
        {
            var documentClient = GetReadWriteDocumentClient();

            return documentClient?.CreateDocumentAsync(_documentCollectionUri, document);
        }

        public static async Task<HttpStatusCode> Delete(string id)
        {
            var readWriteClient = GetReadWriteDocumentClient();
            if (readWriteClient == null)
                return default;

            var result = await readWriteClient?.DeleteDocumentAsync(CreateDocumentUri(id));

            return result?.StatusCode ?? throw new HttpRequestException("Delete Failed");
        }

        static Uri CreateDocumentUri(string id) =>
            UriFactory.CreateDocumentUri(PersonModel.DatabaseId, PersonModel.CollectionId, id);

        static DocumentClient GetReadWriteDocumentClient()
        {
            if (DocumentDbConstants.ReadWritePrimaryKey.Equals("Add Read Write Primary Key"))
                return default;

            return new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadWritePrimaryKey);
        }

        #endregion
    }
}
