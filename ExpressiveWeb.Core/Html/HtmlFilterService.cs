// *********************************************************
// 
// ExpressiveWeb.Core HtmlFilterService.cs
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

public class HtmlFilterService
{
    private readonly List<Type> _knownFilters = new();

    public string Filter(string? htmlContent)
    {
        if (string.IsNullOrEmpty(htmlContent))
        {
            return string.Empty;
        }
        
        HtmlDocument doc = new();
        doc.LoadHtml(htmlContent);

        foreach (Type filter in _knownFilters)
        {
            HtmlFilterBase? filterInstance = (HtmlFilterBase?) Activator.CreateInstance(filter);
            if (filterInstance == null)
            {
                throw new Exception($"Unable to create instance of {filter.Name}");
            }

            filterInstance.Init(doc);
            filterInstance.Execute();
        }

        return doc.DocumentNode.OuterHtml;
    }

    public Task<string> FilterAsync(string? htmlContent)
    {
        return Task.Run(() => Filter(htmlContent));
    }

    public string FilterWith<T>(string? htmlContent) where T : HtmlFilterBase, new()
    {
        if (string.IsNullOrEmpty(htmlContent))
        {
            return string.Empty;
        }

        HtmlDocument doc = new();
        doc.LoadHtml(htmlContent);

        HtmlFilterBase? filterInstance = (HtmlFilterBase?) Activator.CreateInstance(typeof(T));
        if (filterInstance == null)
        {
            throw new Exception($"Unable to create instance of {typeof(T).Name}");
        }

        filterInstance.Init(doc);
        filterInstance.Execute();

        return doc.DocumentNode.OuterHtml;
    }

    public void UseFilter<T>() where T : HtmlFilterBase, new()
    {
        _knownFilters.Add(typeof(T));
    }
}