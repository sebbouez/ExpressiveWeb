// *********************************************************
// 
// ExpressiveWeb.Core AppEnvironmentService.cs
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

using System.Diagnostics;

namespace ExpressiveWeb.Core.Env;

public class AppEnvironmentService : IEnvironmentService
{
    public AppEnvironmentService()
    {
        try
        {
            ExecutablePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? string.Empty;
            KitsFolderPath = Path.Combine(ExecutablePath, "Kits");
            
            ApplicationDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExpressiveWeb");
            EnsureFolderExists(ApplicationDataFolder);
            

            LibraryFolderPath = Path.Combine(ApplicationDataFolder, "Library");
            EnsureFolderExists(LibraryFolderPath);

            IsValid = true;
        }
        catch (Exception ex)
        {
            IsValid = false;
        }
    }

    public bool IsValid
    {
        get;
    }

    public string ApplicationDataFolder
    {
        get;
        init;
    }= string.Empty;

    public string KitsFolderPath
    {
        get;
        init;
    } = string.Empty;

    public string LibraryFolderPath
    {
        get;
        init;
    } = string.Empty;

    public string ExecutablePath
    {
        get;
        init;
    } = string.Empty;

    private void EnsureFolderExists(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }
}