using ExpressiveWeb.Core.Extensions;

namespace TestProject;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("foo", "foo", true)]
    [InlineData("foo", "bar", false)]
    [InlineData("Foo", "foo", true)]
    [InlineData("foo", " foo ", true)]
    [InlineData("", "", false)]
    public void In_WithArray_ShouldMatchCaseInsensitiveAndTrim(string input, string candidate, bool expected)
    {
        bool result = input.In(new[] { candidate });
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("a", "a,b;c", true)]
    [InlineData("b", "a,b;c", true)]
    [InlineData("c", "a,b;c", true)]
    [InlineData("d", "a,b;c", false)]
    [InlineData("x", "", false)]
    [InlineData("x", ";;;, , ", false)]
    public void In_WithDelimitedString_ShouldSupportCommaAndSemicolon(string input, string delimited, bool expected)
    {
        bool result = input.In(delimited);
        Assert.Equal(expected, result);
    }
}
