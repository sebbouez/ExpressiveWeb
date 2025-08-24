// *********************************************************
// 
// ExpressiveWeb.Designer RemoveEditorReferencesFilter.cs
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

public class RemoveEditorReferencesFilter : HtmlFilterBase
{
    private void CheckRemoveNodes(HtmlNodeCollection nodes)
    {
        nodes.Where(node => node.GetAttributeValue("editor-usage", "") == "private").ToList().ForEach(node => node.Remove());
    }

    public override void Execute()
    {
        CheckRemoveNodes(Document.DocumentNode.SelectNodes("//script"));
        CheckRemoveNodes(Document.DocumentNode.SelectNodes("//link"));
    }
}