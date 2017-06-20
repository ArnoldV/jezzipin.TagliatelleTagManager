using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace jezzipin.Tagliatelle
{
    [PluginController("Tagliatelle")]
    public class TagEditorApiController : UmbracoAuthorizedApiController
    {
        IContentService cs = ApplicationContext.Current.Services.ContentService;

        public IEnumerable<string> GetTags(int currentNodeId, int containerId, string documentTypeAlias)
        {
            var tagContainer = cs.GetById(containerId);
            
            var tags = Enumerable.Empty<string>();

            if (tagContainer == null)
            {
                var tagContainerByHelper = _helper.TypedContentAtRoot().DescendantsOrSelf("tagFolder");

                if (tagContainerByHelper != null)
                {
                    var firstTagContainer = tagContainerByHelper.FirstOrDefault();

                    if (firstTagContainer != null)
                    {
                        tagContainer = cs.GetById(firstTagContainer.Id);
                    }
                }
            }

            if(tagContainer != null) { 
            /* Compile a list of all tag pages that exist as children of the tags container */
                tags = tagContainer.Children().Where(x => x.ContentType.Alias == documentTypeAlias).Select(x => x.Name);
            }
            
            return tags;
        }

        public IEnumerable<string> GetTagNames(string nodeIds, string documentTypeAlias)
        {
            if (string.IsNullOrWhiteSpace(nodeIds))
            {
                return new List<string>();
            }

            var ids = nodeIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var nodes = cs.GetByIds(ids);

            var values = nodes.Where(x => x.ContentType.Alias == documentTypeAlias).Select(x => x.Name);

            return values;
        }

        public IEnumerable<int> GetAndEnsureNodeIdsForTags(string currentNodeId, string tags, int containerId, string documentTypeAlias)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return new List<int>();
            }

            // put posted tags in an array
            string[] postedTags = tags.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // get the current node
            IContent node = cs.GetById(int.Parse(currentNodeId));

            // get all existing tag nodes in container
            IContent tagContainer = cs.GetById(containerId);
            IEnumerable<IContent> allTagNodes = tagContainer.Children().Where(x => x.ContentType.Alias == documentTypeAlias);

            bool hasNewTags = false;
            foreach (string postedTag in postedTags)
            {
                // get tag names which do not already exist in the tag container
                bool found = allTagNodes.Any(x => x.Name == postedTag);
                if (!found)
                {
                    // tag node doesnt exist so create new node
                    var dic = new Dictionary<string, object>() { { documentTypeAlias, postedTag } };
                    IContent newTag = cs.CreateContent(postedTag, tagContainer.Id, documentTypeAlias);
                    cs.SaveAndPublishWithStatus(newTag);
                    hasNewTags = true;
                }
            }

            // re-get container because new nodes might have been added.
            tagContainer = cs.GetById(containerId);
            if (hasNewTags)
            {
                // new tag so sort!
                cs.Sort(tagContainer.Children().OrderBy(x => x.Name));
            }

            // get all tag ids, and return
            IEnumerable<int> tagIds = tagContainer.Children().Where(x => postedTags.Contains(x.Name)).Select(x => x.Id);

            return tagIds;
        }
    }
}
