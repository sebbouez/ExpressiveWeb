// *********************************************************
// 
// ExpressiveWeb.Designer AddEditorReferencesFilter.cs
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
using ExpressiveWeb.Designer.Cef;
using HtmlAgilityPack;

namespace ExpressiveWeb.Designer.Filters;

public class AddEditorReferencesFilter : HtmlFilterBase
{
    public override void Execute()
    {
        HtmlNode headNode = Document.DocumentNode.SelectSingleNode("//head");

        AddStyle(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/editorstyle.css");
        
        AddStyle(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://d/kit-editorstyles.css");
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://d/kit-utils.js");
        
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/main.js");
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/EditorControl.Text.js");
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/EditorControl.Dom.js");
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/EditorControl.Adorner.Decorator.js");
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/EditorControl.Adorner.InsertBar.js");
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/EditorControl.Adorner.js");
        AddScript(headNode, $"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://f/EditorControl.js");
    }

    private void AddScript(HtmlNode headNode, string url)
    {
        HtmlNode scriptNode = Document.CreateElement("script");
        scriptNode.SetAttributeValue("editor-usage", "private");
        scriptNode.SetAttributeValue("src", url);
        headNode.AppendChild(scriptNode);
    }

    private void AddStyle(HtmlNode headNode, string url)
    {
        HtmlNode styleNode = Document.CreateElement("link");
        styleNode.SetAttributeValue("editor-usage", "private");
        styleNode.SetAttributeValue("rel", "stylesheet");
        styleNode.SetAttributeValue("href", url);
        headNode.AppendChild(styleNode);
    }
}