// *********************************************************
// 
// ExpressiveWeb.Core IKitService.cs
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

namespace ExpressiveWeb.Core.Kit;

public interface IKitService
{
    Task<Kit?> LoadKit(string kitName);
    Task<List<Kit>> LoadKits();

    /// <summary>
    /// Reads a kit.xml file and returns a Kit populated with Name, Version and DefaultPackages.
    /// </summary>
    /// <param name="xmlFilePath">Full path to the kit.xml file.</param>
    /// <returns>A Kit instance populated from the XML, or null if parsing fails.</returns>
    Kit? ReadKitXml(string xmlFilePath);

    bool CopyTemplateToFile(string kitName, KitPageTemplate template, string targetFileName);
}