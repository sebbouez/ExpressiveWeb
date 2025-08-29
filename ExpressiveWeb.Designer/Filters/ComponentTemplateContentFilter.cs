// *********************************************************
// 
// ExpressiveWeb.Designer ComponentTemplateContentFilter.cs
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

using System.Text.RegularExpressions;
using ExpressiveWeb.Core.Html;
using HtmlAgilityPack;

namespace ExpressiveWeb.Designer.Filters;

public class ComponentTemplateContentFilter : HtmlFilterBase
{
    public override void Execute()
    {
        HtmlNode? slotTag = Document.DocumentNode.SelectSingleNode("//slot");
        if (slotTag != null)
        {
            slotTag.Remove();
        }

        HtmlNode? firstRelevantNode = Document.DocumentNode.ChildNodes.FirstOrDefault(x => !x.Name.Equals("#text", StringComparison.InvariantCultureIgnoreCase));

        if (firstRelevantNode == null)
        {
            return;
        }

        string content = firstRelevantNode.InnerHtml;
        content = Regex.Replace(content, @"[\r\n]", string.Empty);
        Document.LoadHtml(content.Trim());
    }
}