// *********************************************************
// 
// ExpressiveWeb.Core FilterBase.cs
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

public abstract class HtmlFilterBase
{
    public void Init(HtmlDocument document)
    {
        Document = document;
    }

    protected HtmlDocument Document
    {
        get;
        private set;
    }

    /// <summary>
    /// Cleans and transforms HTML from editor to export
    /// </summary>
    public abstract void Execute();


}