// *********************************************************
// 
// ExpressiveWeb.Core KitGalleryFamily.cs
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

public enum KitGalleryFamily
{
    None = 0,
    Text = 2,
    Headings = 4,
    Hero = 8,
    Buttons = 16,
    Forms = 32,
    Tables = 64,
    Lists = 128,
    Images = 256,
    Media = 512,
    Navigation = 1024,
    Layout = 2048,
    PriceTable = 4096,
    Cards = 8192,
}