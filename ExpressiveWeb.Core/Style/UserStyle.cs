// *********************************************************
// 
// ExpressiveWeb.Core UserStyle.cs
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

namespace ExpressiveWeb.Core.Style;

public class UserStyle
{
    public string Name
    {
        get;
        set;
    }

    public List<StyleProperty> RuleSet
    {
        get;
        set;
    } = new();
}

public class StyleProperty
{
    public StyleProperty(string name)
    {
        Name = name;
    }

    public string Name
    {
        get;
        set;
    }

    public string Value
    {
        get;
        set;
    } =string.Empty;
}