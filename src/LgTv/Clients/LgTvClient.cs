﻿using LgTv.Clients.Apps;
using LgTv.Clients.Audio;
using LgTv.Clients.Channels;
using LgTv.Clients.Info;
using LgTv.Clients.Inputs;
using LgTv.Clients.Mouse;
using LgTv.Clients.Notifications;
using LgTv.Clients.Playback;
using LgTv.Clients.Power;
using LgTv.Connections;
using LgTv.Extensions;
using LgTv.Networking;
using LgTv.Stores;

namespace LgTv.Clients;

public class LgTvClient : ILgTvClient
{
    public const int DefaultPort = 3001;

    private const string WebSocketScheme = "wss";

    private const string BeforePairHandShake = "{\"type\":\"register\",\"id\":\"register_0\",\"payload\":{\"forcePairing\":false,\"pairingType\":\"PROMPT\",\"manifest\":{\"manifestVersion\":1,\"appVersion\":\"1.1\",\"signed\":{\"created\":\"20140509\",\"appId\":\"com.lge.test\",\"vendorId\":\"com.lge\",\"localizedAppNames\":{\"\":\"LG Remote App\",\"ko-KR\":\"리모컨 앱\",\"zxx-XX\":\"ЛГ Rэмotэ AПП\"},\"localizedVendorNames\":{\"\":\"LG Electronics\"},\"permissions\":[\"TEST_SECURE\",\"CONTROL_INPUT_TEXT\",\"CONTROL_MOUSE_AND_KEYBOARD\",\"READ_INSTALLED_APPS\",\"READ_LGE_SDX\",\"READ_NOTIFICATIONS\",\"SEARCH\",\"WRITE_SETTINGS\",\"WRITE_NOTIFICATION_ALERT\",\"CONTROL_POWER\",\"READ_CURRENT_CHANNEL\",\"READ_RUNNING_APPS\",\"READ_UPDATE_INFO\",\"UPDATE_FROM_REMOTE_APP\",\"READ_LGE_TV_INPUT_EVENTS\",\"READ_TV_CURRENT_TIME\"],\"serial\":\"2f930e2d2cfe083771f68e4fe7bb07\"},\"permissions\":[\"LAUNCH\",\"LAUNCH_WEBAPP\",\"APP_TO_APP\",\"CLOSE\",\"TEST_OPEN\",\"TEST_PROTECTED\",\"CONTROL_AUDIO\",\"CONTROL_DISPLAY\",\"CONTROL_INPUT_JOYSTICK\",\"CONTROL_INPUT_MEDIA_RECORDING\",\"CONTROL_INPUT_MEDIA_PLAYBACK\",\"CONTROL_INPUT_TV\",\"CONTROL_POWER\",\"READ_APP_STATUS\",\"READ_CURRENT_CHANNEL\",\"READ_INPUT_DEVICE_LIST\",\"READ_NETWORK_STATE\",\"READ_RUNNING_APPS\",\"READ_TV_CHANNEL_LIST\",\"WRITE_NOTIFICATION_TOAST\",\"READ_POWER_STATE\",\"READ_COUNTRY_INFO\"],\"signatures\":[{\"signatureVersion\":1,\"signature\":\"eyJhbGdvcml0aG0iOiJSU0EtU0hBMjU2Iiwia2V5SWQiOiJ0ZXN0LXNpZ25pbmctY2VydCIsInNpZ25hdHVyZVZlcnNpb24iOjF9.hrVRgjCwXVvE2OOSpDZ58hR+59aFNwYDyjQgKk3auukd7pcegmE2CzPCa0bJ0ZsRAcKkCTJrWo5iDzNhMBWRyaMOv5zWSrthlf7G128qvIlpMT0YNY+n/FaOHE73uLrS/g7swl3/qH/BGFG2Hu4RlL48eb3lLKqTt2xKHdCs6Cd4RMfJPYnzgvI4BNrFUKsjkcu+WD4OO2A27Pq1n50cMchmcaXadJhGrOqH5YmHdOCj5NSHzJYrsW0HPlpuAx/ECMeIZYDh6RMqaFM2DXzdKX9NmmyqzJ3o/0lkk/N97gfVRLW5hA29yeAwaCViZNCP8iC9aO0q9fQojoa7NQnAtw==\"}]}}}";
    private const string AfterPairHandShake = "{\"type\":\"register\",\"id\":\"register_0\",\"payload\":{\"forcePairing\":false,\"pairingType\":\"PROMPT\",\"client-key\":\"CLIENTKEYGOESHERE\",\"manifest\":{\"manifestVersion\":1,\"appVersion\":\"1.1\",\"signed\":{\"created\":\"20140509\",\"appId\":\"com.lge.test\",\"vendorId\":\"com.lge\",\"localizedAppNames\":{\"\":\"LG Remote App\",\"ko-KR\":\"리모컨 앱\",\"zxx-XX\":\"ЛГ Rэмotэ AПП\"},\"localizedVendorNames\":{\"\":\"LG Electronics\"},\"permissions\":[\"TEST_SECURE\",\"CONTROL_INPUT_TEXT\",\"CONTROL_MOUSE_AND_KEYBOARD\",\"READ_INSTALLED_APPS\",\"READ_LGE_SDX\",\"READ_NOTIFICATIONS\",\"SEARCH\",\"WRITE_SETTINGS\",\"WRITE_NOTIFICATION_ALERT\",\"CONTROL_POWER\",\"READ_CURRENT_CHANNEL\",\"READ_RUNNING_APPS\",\"READ_UPDATE_INFO\",\"UPDATE_FROM_REMOTE_APP\",\"READ_LGE_TV_INPUT_EVENTS\",\"READ_TV_CURRENT_TIME\"],\"serial\":\"2f930e2d2cfe083771f68e4fe7bb07\"},\"permissions\":[\"LAUNCH\",\"LAUNCH_WEBAPP\",\"APP_TO_APP\",\"CLOSE\",\"TEST_OPEN\",\"TEST_PROTECTED\",\"CONTROL_AUDIO\",\"CONTROL_DISPLAY\",\"CONTROL_INPUT_JOYSTICK\",\"CONTROL_INPUT_MEDIA_RECORDING\",\"CONTROL_INPUT_MEDIA_PLAYBACK\",\"CONTROL_INPUT_TV\",\"CONTROL_POWER\",\"READ_APP_STATUS\",\"READ_CURRENT_CHANNEL\",\"READ_INPUT_DEVICE_LIST\",\"READ_NETWORK_STATE\",\"READ_RUNNING_APPS\",\"READ_TV_CHANNEL_LIST\",\"WRITE_NOTIFICATION_TOAST\",\"READ_POWER_STATE\",\"READ_COUNTRY_INFO\"],\"signatures\":[{\"signatureVersion\":1,\"signature\":\"eyJhbGdvcml0aG0iOiJSU0EtU0hBMjU2Iiwia2V5SWQiOiJ0ZXN0LXNpZ25pbmctY2VydCIsInNpZ25hdHVyZVZlcnNpb24iOjF9.hrVRgjCwXVvE2OOSpDZ58hR+59aFNwYDyjQgKk3auukd7pcegmE2CzPCa0bJ0ZsRAcKkCTJrWo5iDzNhMBWRyaMOv5zWSrthlf7G128qvIlpMT0YNY+n/FaOHE73uLrS/g7swl3/qH/BGFG2Hu4RlL48eb3lLKqTt2xKHdCs6Cd4RMfJPYnzgvI4BNrFUKsjkcu+WD4OO2A27Pq1n50cMchmcaXadJhGrOqH5YmHdOCj5NSHzJYrsW0HPlpuAx/ECMeIZYDh6RMqaFM2DXzdKX9NmmyqzJ3o/0lkk/N97gfVRLW5hA29yeAwaCViZNCP8iC9aO0q9fQojoa7NQnAtw==\"}]}}}";

    private readonly Func<ILgTvConnection> _connectionFactory;
    private readonly ILgTvConnection _connection;
    private readonly IClientKeyStore _keyStore;

    private readonly string _ipAddress;

    private readonly Uri _webSocketUri;

    public LgTvClient(
        Func<ILgTvConnection> connectionFactory,
        IClientKeyStore keyStore,
        string host,
        int port)
        : this(connectionFactory,
            keyStore,
            new HostConfiguration
            {
                Host = host,
                Port = port
            })
    {
    }

    public LgTvClient(
        Func<ILgTvConnection> connectionFactory,
        IClientKeyStore keyStore,
        HostConfiguration tvHostConfiguration)
        : this(
            connectionFactory,
            keyStore,
            new LgTvClientConfiguration(tvHostConfiguration))
    {
    }

    public LgTvClient(
        Func<ILgTvConnection> connectionFactory,
        IClientKeyStore keyStore,
        LgTvClientConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _connection = connectionFactory();
        _keyStore = keyStore;

        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        string url;
        if (Configuration.Proxy == null)
        {
            url = ToUrl(Configuration.Tv);
        }
        else
        {
            // TODO Make proxy more configurable
            url = ToUrl(Configuration.Proxy) + "/" + ToUrl(Configuration.Tv);
        }

        _webSocketUri = new Uri(url);

        _ipAddress = configuration.Tv.Host;
        if (!_ipAddress.IsIPAddress())
        {
            _ipAddress = IPAddressResolver.GetIPAddress(_ipAddress);
        }

        Info = new LgTvInfoClient(_connection);
        Power = new LgTvPowerClient(_connection, _ipAddress);
        Audio = new LgTvAudioClient(_connection);
        Playback = new LgTvPlaybackClient(_connection);
        Keyboard = new LgTvKeyboardClient(_connection);
        Channels = new LgTvChannelClient(_connection);
        Apps = new LgTvAppClient(_connection);
        Inputs = new LgTvInputClient(_connection);
        Notifications = new LgTvNotificationClient(_connection);
    }

    public LgTvClientConfiguration Configuration { get; }
    
    public async Task<bool> Connect()
    {
        return await _connection.Connect(_webSocketUri);
    }

    public async Task MakeHandShake()
    {
        var currentPairKey = await _keyStore.GetClientKey(_ipAddress);
        dynamic result;
        if (currentPairKey != null)
        {
            var key = AfterPairHandShake.Replace("CLIENTKEYGOESHERE", currentPairKey);
            result = await _connection.SendCommandAsync(key);
            await _keyStore.SetClientKey(_ipAddress, (string) result.clientKey);
            return;
        }

        result = await _connection.SendCommandAsync(BeforePairHandShake);
        await _keyStore.SetClientKey(_ipAddress, result.clientKey);
    }
    

    public async Task<ILgWebOsMouseClient> GetMouse()
    {
        var requestMessage = new RequestMessage(LgTvCommands.GetMouse.Uri);
        var response = await _connection.SendCommandAsync(requestMessage);
        var socketPath = (string) response.socketPath;

        if (Configuration.Proxy != null)
        {
            socketPath = ToUrl(Configuration.Proxy) + "/" + socketPath;
        }

        var mouseConnection = new LgWebOsMouseClient(_connectionFactory());
        
        await mouseConnection.Connect(socketPath);
        
        return mouseConnection;
    }


    public ILgTvInfoClient Info { get; }

    public ILgTvPowerClient Power { get; }

    public ILgTvAudioClient Audio { get; }

    public ILgTvPlaybackClient Playback { get; }

    public ILgTvKeyboardClient Keyboard { get; }

    public ILgTvChannelClient Channels { get; }
    
    public ILgTvAppClient Apps { get; }
    
    public ILgTvInputClient Inputs { get; }

    public ILgTvNotificationClient Notifications { get; }


    public async Task<dynamic> SendCommand(RequestMessage message)
    {
        return await _connection.SendCommandAsync(message);
    }


    public void Dispose()
    {
       _connection?.Dispose();
    }

    private static string ToUrl(HostConfiguration configuration)
    {
        return new UriBuilder(WebSocketScheme, configuration.Host, configuration.Port).ToString().TrimEnd('/');
    }
}