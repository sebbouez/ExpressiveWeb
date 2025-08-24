// *********************************************************
// 
// ExpressiveWeb.Core NetworkService.cs
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

using System.Net.NetworkInformation;
using ExpressiveWeb.Core.Log;

namespace ExpressiveWeb.Core.Network;

public class NetworkService : INetworkService
{
    private readonly ILogService _logService;
    private bool _acceptOnlyHttps = true;
    private DateTime _lastConnectionCheckTime;
    private bool _lastConnectionStatus;
    private NetworkServiceMode _mode = NetworkServiceMode.Auto;

    public NetworkService(ILogService logService)
    {
        _logService = logService;
    }

    private double LastConnectionCheckMs
    {
        get
        {
            return (DateTime.Now - _lastConnectionCheckTime).TotalMilliseconds;
        }
    }

    public bool AcceptOnlyHttps
    {
        get
        {
            return _acceptOnlyHttps;
        }
        set
        {
            _acceptOnlyHttps = value;
            _logService.Info($"AcceptOnlyHttps set to {value}");
        }
    }

    public NetworkServiceMode Mode
    {
        get
        {
            return _mode;
        }
        set
        {
            _mode = value;
            _logService.Info($"Network service mode set to {value}");
            ModeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? ModeChanged;

    public bool IsConnected()
    {
        if (Mode == NetworkServiceMode.RequiredOffline)
        {
            return false;
        }

        if (LastConnectionCheckMs < 30000)
        {
            return _lastConnectionStatus;
        }

        try
        {
            _lastConnectionCheckTime = DateTime.Now;
            Ping myPing = new();
            string host = "google.com";
            byte[] buffer = new byte[32];
            int timeout = 1000;
            PingOptions pingOptions = new();
            PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);

            _lastConnectionStatus = reply.Status == IPStatus.Success;
            return _lastConnectionStatus;
        }
        catch (Exception)
        {
            return false;
        }
    }
}