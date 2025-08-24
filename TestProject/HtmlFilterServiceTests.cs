using System.Threading.Tasks;
using ExpressiveWeb.Core.Html;
using HtmlAgilityPack;

namespace TestProject;

public class HtmlFilterServiceTests
{
    // Simple test filters
    private sealed class AddP1Filter : HtmlFilterBase
    {
        public override void Execute()
        {
            HtmlNode? body = Document.DocumentNode.SelectSingleNode("//body");
            if (body == null)
            {
                body = Document.CreateElement("body");
                Document.DocumentNode.AppendChild(body);
            }
            HtmlNode p = Document.CreateElement("p");
            p.SetAttributeValue("id", "p1");
            p.InnerHtml = "one";
            body.AppendChild(p);
        }
    }

    private sealed class AddP2Filter : HtmlFilterBase
    {
        public override void Execute()
        {
            HtmlNode? body = Document.DocumentNode.SelectSingleNode("//body");
            if (body == null)
            {
                body = Document.CreateElement("body");
                Document.DocumentNode.AppendChild(body);
            }
            HtmlNode p = Document.CreateElement("p");
            p.SetAttributeValue("id", "p2");
            p.InnerHtml = "two";
            body.AppendChild(p);
        }
    }

    private const string BaseHtml = "<html><head><title>T</title></head><body><div id='c'>x</div></body></html>";

    [Fact]
    public void FilterWith_ShouldApplySingleFilter()
    {
        HtmlFilterService sut = new();
        string result = sut.FilterWith<AddP1Filter>(BaseHtml);

        Assert.Contains("id=\"p1\"", result);
        Assert.Contains(">one<", result);
        // Base content should remain
        Assert.Contains("id='c'", result);
    }

    [Fact]
    public void Filter_ShouldApplyRegisteredFiltersInOrder()
    {
        HtmlFilterService sut = new();
        sut.UseFilter<AddP1Filter>();
        sut.UseFilter<AddP2Filter>();

        string result = sut.Filter(BaseHtml);

        int i1 = result.IndexOf("id=\"p1\"", StringComparison.Ordinal);
        int i2 = result.IndexOf("id=\"p2\"", StringComparison.Ordinal);
        Assert.True(i1 >= 0 && i2 >= 0, "Both p1 and p2 should be present");
        Assert.True(i1 < i2, "p1 should appear before p2, preserving filter registration order");
    }

    [Fact]
    public async Task FilterAsync_ShouldReturnSameAsSync()
    {
        HtmlFilterService sut = new();
        sut.UseFilter<AddP1Filter>();
        sut.UseFilter<AddP2Filter>();

        string sync = sut.Filter(BaseHtml);
        string asyncRes = await sut.FilterAsync(BaseHtml);

        Assert.Equal(sync, asyncRes);
    }
}
