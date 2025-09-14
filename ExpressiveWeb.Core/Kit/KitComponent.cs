// *********************************************************
// 
// ExpressiveWeb.Core KitComponent.cs
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

using ExpressiveWeb.Core.Kit.ComponentFeatures;

namespace ExpressiveWeb.Core.Kit;

public class KitComponent
{
    public string GetSelector()
    {
        return (!string.IsNullOrEmpty(HtmlClassName)
            ? $"{HtmlTagName}.{HtmlClassName}"
            : HtmlTagName) ?? string.Empty;
    }

    public bool HasFeature<T>()
    {
        return Features.Any(f => f.GetType() == typeof(T));
    }

    public bool Allows(string type)
    {
        return Accepts != null && Accepts.Split(';', StringSplitOptions.RemoveEmptyEntries).Contains(type, StringComparer.OrdinalIgnoreCase);
    }

    public string UID
    {
        get;
        set;
    } = null!;

    public string? Name
    {
        get;
        set;
    }  
    
    public string? Family
    {
        get;
        set;
    }

    public string? Accepts
    {
        get;
        set;
    }

    public string? Denies
    {
        get;
        set;
    }

    public string? Slots
    {
        get;
        set;
    }

    public string? Template
    {
        get;
        set;
    }

    public string? HtmlTagName
    {
        get;
        set;
    }

    public string? HtmlClassName
    {
        get;
        set;
    }

    public List<QuickAction> ActionList
    {
        get;
        set;
    } = new List<QuickAction>();

    public List<ComponentFeatureBase> Features
    {
        get;
    } = new();
    
    
    public List<ComponentVariant> Variants
    {
        get;
    } = new();
}

public class ComponentVariant
{
    public string? Name
    {
        get;
        set;
    }
    
    public string? CssClass
    {
        get;
        set;
    }
}