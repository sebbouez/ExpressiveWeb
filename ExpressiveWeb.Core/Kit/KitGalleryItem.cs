// *********************************************************
// 
// ExpressiveWeb.Core KitGalleryItem.cs
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

public class KitGalleryItem
{
    public string? Name
    {
        get;
        set;
    }  
    
    public KitGalleryFamily Family
    {
        get;
        set;
    }
    
    public string? Template
    {
        get;
        set;
    }

    public string? ImageFilePath
    {
        get;
        set;
    }
}