// *********************************************************
// 
// ExpressiveWeb.Core EditorInternalIdFilter.cs
// Copyright (c) Sébastien Bouez. All rights reserved.
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// *********************************************************

using ExpressiveWeb.Core.Html;
using HtmlAgilityPack;

namespace ExpressiveWeb.Designer.Filters;

public class RemoveEditorInternalIdFilter : HtmlFilterBase
{
    public override void Execute()
    {
        IEnumerable<HtmlNode> allTags = Document.DocumentNode.Descendants();

        foreach (HtmlNode item in allTags)
        {
            List<string> attributesToRemove = (from t in item.Attributes where t.Name.StartsWith("data-internal", StringComparison.InvariantCultureIgnoreCase) select t.Name).Distinct().ToList();

            foreach (string attrName in attributesToRemove)
            {
                item.Attributes.Remove(attrName);
            }
        }
    }
}