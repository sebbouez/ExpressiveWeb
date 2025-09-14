// *********************************************************
// 
// ExpressiveWeb.Core IEnvironmentService.cs
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

namespace ExpressiveWeb.Core.Env;

public interface IEnvironmentService
{
    /// <summary>
    ///     Gets the path to the ApplicationData folder.
    /// </summary>
    string ApplicationDataFolder
    {
        get;
        init;
    }

    /// <summary>
    ///     Indicates whether the environment is valid.
    /// </summary>
    bool IsValid
    {
        get;
    }

    /// <summary>
    ///     Gets the path to the Kits folder.
    /// </summary>
    string KitsFolderPath
    {
        get;
        init;
    }

    /// <summary>
    ///     Gets the path to the Library folder.
    /// </summary>
    string LibraryFolderPath
    {
        get;
        init;
    }
}