using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace jezzipin.Tagliatelle
{
    [PluginController("Tagliatelle")]
    public class DocumentTypesApiController : UmbracoAuthorizedApiController
    {
        IContentTypeService ct = ApplicationContext.Current.Services.ContentTypeService;

        public List<DocumentType> GetDocumentTypes()
        {
            List<DocumentType> documentTypes = new List<DocumentType>();

            IEnumerable<IContentType> contentTypes = ct.GetAllContentTypes();
            foreach (IContentType contentType in contentTypes)
            {
                DocumentType documentType = new DocumentType();
                documentType.Name = contentType.Name;
                documentType.Alias = contentType.Alias;
                documentTypes.Add(documentType);
            }

            return documentTypes;

        }
    }

    public class DocumentType
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
