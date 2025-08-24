// *********************************************************
// 
// ExpressiveWeb.Core IPackagesService.cs
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

using ExpressiveWeb.Common.Packages.Providers;

namespace ExpressiveWeb.Core.Packages;

public interface IPackagesService
{
    Task Init();
    PackageProviderBase? GetProvider(string providerName);
    Task LoadPackages(Project.Project project);
    ExpressiveWeb.Common.Packages.Providers.PackageReference? ReadPackageFromXmlNode(System.Xml.XmlNode node);
}