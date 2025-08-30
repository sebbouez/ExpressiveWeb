// *********************************************************
// 
// ExpressiveWeb.Core SettingsService.cs
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

using System.Text.Json;
using ExpressiveWeb.Core.Env;
using ExpressiveWeb.Core.FileManagement;

namespace ExpressiveWeb.Core.Settings;

public class SettingsService : ISettingsService
{
    private readonly IEnvironmentService _environment;

    public SettingsService(IEnvironmentService environment)
    {
        _environment = environment;
    }

    public UserSettings UserSettings
    {
        get;
        private set;
    }

    public void SaveSettings()
    {
        string userSettingsFilePath = Path.Combine(_environment.ApplicationDataFolder, "usersettings.json");
        string json = JsonSerializer.Serialize(UserSettings);
        FilesAccessHelper.WriteAllText(userSettingsFilePath, json);
    }

    public void LoadSettings(UserSettings? defaultSettings = null)
    {
        string userSettingsFilePath = Path.Combine(_environment.ApplicationDataFolder, "usersettings.json");

        if (File.Exists(userSettingsFilePath))
        {
            string json = FilesAccessHelper.ReadAllText(userSettingsFilePath);
            UserSettings = JsonSerializer.Deserialize<UserSettings?>(json);
        }

        if (UserSettings == null && defaultSettings != null)
        {
            UserSettings = defaultSettings;
            return;
        }

        UserSettings = new UserSettings();
    }
}