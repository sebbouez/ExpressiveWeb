// *********************************************************
// 
// ExpressiveWeb.Common WebPackage.cs
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

namespace ExpressiveWeb.Common.Packages.Providers;

public class PackageReference
{
    public string Name
    {
        get;
        set;
    } = string.Empty;

    public string Version
    {
        get;
        set;
    }= string.Empty;
    
    public string Provider
    {
        get;
        set;
    }= string.Empty;
    
    public List<WebResource> Resources
    {
        get;
        set;
    } = new();

    public string? ResourcesListUrl
    {
        get;
        set;
    }
}