using System;
using System.Linq;
using ExpressiveWeb.Core.Html;
using ExpressiveWeb.Designer.Cef;
using ExpressiveWeb.Designer.Filters;
using HtmlAgilityPack;
using Xunit;

namespace TestProject;

public class DesignerFiltersTests
{
    private static HtmlDocument LoadHtml(string html)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(html);
        return doc;
    }

    [Fact]
    public void AddEditorReferencesFilter_ShouldInjectPrivateResourcesIntoHead()
    {
        // Arrange
        string input = "<html><head><title>T</title></head><body></body></html>";
        HtmlFilterService svc = new();

        // Act
        string output = svc.FilterWith<AddEditorReferencesFilter>(input);

        // Assert
        HtmlDocument doc = LoadHtml(output);
        HtmlNode head = doc.DocumentNode.SelectSingleNode("//head");
        Assert.NotNull(head);

        // One stylesheet link with editor-usage="private" and pfinternal scheme
        var links = head.SelectNodes("./link")?.ToList() ?? new();
        Assert.True(links.Count >= 1, "At least one link should be present");
        var privateLinks = links.Where(l => l.GetAttributeValue("editor-usage", "") == "private").ToList();
        Assert.Single(privateLinks);
        string href = privateLinks[0].GetAttributeValue("href", "");
        Assert.StartsWith($"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://", href, StringComparison.Ordinal);
        Assert.Contains("/editorstyle.css", href, StringComparison.OrdinalIgnoreCase);

        // Seven private scripts added with scheme and expected filenames
        var scripts = head.SelectNodes("./script")?.ToList() ?? new();
        var privateScripts = scripts.Where(s => s.GetAttributeValue("editor-usage", "") == "private").ToList();
        Assert.Equal(7, privateScripts.Count);
        foreach (var s in privateScripts)
        {
            string src = s.GetAttributeValue("src", "");
            Assert.StartsWith($"{CustomSchemeHandler.LOCAL_FILE_SCHEME}://", src, StringComparison.Ordinal);
        }
        // Ensure each expected script is present by filename
        string[] expected =
        {
            "kit-utils.js",
            "main.js",
            "EditorControl.Text.js",
            "EditorControl.Dom.js",
            "EditorControl.Adorner.Decorator.js",
            "EditorControl.Adorner.js",
            "EditorControl.js"
        };
        foreach (string name in expected)
        {
            Assert.Contains(privateScripts, s => s.GetAttributeValue("src", "").EndsWith("/" + name, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void RemoveEditorReferencesFilter_ShouldRemoveOnlyPrivateRefs()
    {
        // Arrange: add both private and public resources
        string input = $"<html><head>" +
                       $"<link rel='stylesheet' href='pfinternal://f/editor.css' editor-usage='private'/>" +
                       $"<link rel='stylesheet' href='https://cdn/some.css'/>" +
                       $"<script src='pfinternal://f/private.js' editor-usage='private'></script>" +
                       $"<script src='https://cdn/public.js'></script>" +
                       "</head><body></body></html>";
        HtmlFilterService svc = new();

        // Act
        string output = svc.FilterWith<RemoveEditorReferencesFilter>(input);

        // Assert
        HtmlDocument doc = LoadHtml(output);
        HtmlNode head = doc.DocumentNode.SelectSingleNode("//head");
        Assert.NotNull(head);

        var allLinks = head.SelectNodes("./link")?.ToList() ?? new();
        var allScripts = head.SelectNodes("./script")?.ToList() ?? new();
        // No private resources remain
        Assert.DoesNotContain(allLinks, l => l.GetAttributeValue("editor-usage", "") == "private");
        Assert.DoesNotContain(allScripts, s => s.GetAttributeValue("editor-usage", "") == "private");
        // Public ones should remain
        Assert.Contains(allLinks, l => l.GetAttributeValue("href", "").StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(allScripts, s => s.GetAttributeValue("src", "").StartsWith("https://", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RemoveEditorInternalIdFilter_ShouldStripDataInternalAttributes()
    {
        // Arrange
        string input = "<html><body>" +
                       "<div id='x' class='c' data-internal-id='1' data-Internal-State='focus' data-internal='y' data-info='keep'>content</div>" +
                       "</body></html>";
        HtmlFilterService svc = new();

        // Act
        string output = svc.FilterWith<RemoveEditorInternalIdFilter>(input);

        // Assert
        HtmlDocument doc = LoadHtml(output);
        HtmlNode div = doc.DocumentNode.SelectSingleNode("//div[@id='x']");
        Assert.NotNull(div);
        // removed
        Assert.Null(div.Attributes["data-internal-id"]);
        Assert.Null(div.Attributes["data-Internal-State"]);
        Assert.Null(div.Attributes["data-internal"]);
        // kept
        Assert.Equal("c", div.GetAttributeValue("class", ""));
        Assert.Equal("keep", div.GetAttributeValue("data-info", ""));
    }

    [Fact]
    public void RemoveEditorAdornerFilter_ShouldRemoveAdornerLayer()
    {
        // Arrange
        string input = "<html><head></head><body><div>ok</div><adorner-layer id='a1'></adorner-layer></body></html>";
        HtmlFilterService svc = new();

        // Act
        string output = svc.FilterWith<RemoveEditorAdornerFilter>(input);

        // Assert
        HtmlDocument doc = LoadHtml(output);
        var adorner = doc.DocumentNode.SelectSingleNode("//adorner-layer");
        Assert.Null(adorner);
        // Ensure other content remains
        Assert.NotNull(doc.DocumentNode.SelectSingleNode("//div[text()='ok']"));
    }
}
