// *********************************************************
// 
// ExpressiveWeb.Core HtmlElementPropertyDeclaration.cs
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

namespace ExpressiveWeb.Core.Html;

public class HtmlElementPropertyDeclaration
{
    public string? Name
    {
        get;
        init;
    }

    public string? Description
    {
        get;
        init;
    }

    public string? Type
    {
        get;
        init;
    }

    /// <summary>
    /// Indicates if the property is read only.
    /// </summary>
    public bool IsReadOnly
    {
        get;
        set;
    }
}