// *********************************************************
// 
// ExpressiveWeb.Core HtmlHelper.cs
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

using HtmlAgilityPack;

namespace ExpressiveWeb.Core.Html;

public class HtmlFragmentHelper
{
    private readonly string _htmlFragment;

    public HtmlFragmentHelper(string htmlFragment)
    {
        _htmlFragment = htmlFragment;
    }

    public void Process(out string tagName, out string cssClass)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(_htmlFragment);

        HtmlNode? relevantNode = doc.DocumentNode.ChildNodes.FirstOrDefault(x => x is not HtmlTextNode);
        if (relevantNode == null)
        {
            throw new Exception("No relevant node found.");
        }

        tagName = relevantNode.Name;
        cssClass = relevantNode.GetAttributeValue("class", "");
    }
}