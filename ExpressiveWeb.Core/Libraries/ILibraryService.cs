// *********************************************************
// 
// ExpressiveWeb.Core ILibraryService.cs
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressiveWeb.Core.Libraries;

public interface ILibraryService
{
    List<LibraryItemBase> LibraryItems
    {
        get;
        set;
    }

    Task LoadItemsAsync(CancellationToken cancellationToken = default);

    // Returns the Type mapped to the given category, or null if unknown
    Type? GetItemTypeForCategory(CategoryNodeType category);

    // Returns items filtered by the given category.
    IEnumerable<LibraryItemBase> GetItemsByCategory(CategoryNodeType category);
}