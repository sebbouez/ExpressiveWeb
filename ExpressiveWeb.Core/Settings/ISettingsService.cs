// *********************************************************
// 
// ExpressiveWeb.Core ISettingsService.cs
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

namespace ExpressiveWeb.Core.Settings;

public interface ISettingsService
{
    UserSettings UserSettings
    {
        get;
    }

    /// <summary>
    /// Loads user settings from a predefined location. If no saved settings exist,
    /// initializes the settings with provided default values or creates a new instance.
    /// </summary>
    /// <param name="defaultSettings">Optional parameter to provide a set of default settings
    /// if no saved settings are found.</param>
    void LoadSettings(UserSettings? defaultSettings = null);

    void SaveSettings();
}