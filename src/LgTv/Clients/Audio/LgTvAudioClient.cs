﻿using System.Threading.Tasks;
using LgTv.Connections;

namespace LgTv.Clients.Audio
{
    internal class LgTvAudioClient : ILgTvAudioClient
    {
        private readonly ILgTvConnection _connection;

        public LgTvAudioClient(
            ILgTvConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> GetOutput()
        {
            var requestMessage = new RequestMessage("status", "ssap://com.webos.service.apiadapter/audio/getSoundOutput");
            var response = await _connection.SendCommandAsync(requestMessage);
            return response.soundOutput;
        }

        public async Task<int> GetVolume()
        {
            // {
            //     "type": "response",
            //     "id": "status_1",
            //     "payload": {
            //         "muted": false,
            //         "scenario": "mastervolume_tv_speaker",
            //         "active": false,
            //         "action": "requested",
            //         "volume": 7,
            //         "returnValue": true,
            //         "subscribed": true
            //     }
            // }
            var requestMessage = new RequestMessage("status", "ssap://audio/getVolume");
            var response = await _connection.SendCommandAsync(requestMessage);
            return (bool) response.muted ? -1 : int.Parse((string) response.volume);
        }

        public async Task<bool> IsMuted()
        {
            var requestMessage = new RequestMessage("status", "ssap://audio/getStatus");
            var response = await _connection.SendCommandAsync(requestMessage);
            return (bool) response.mute;
        }

        public async Task VolumeUp()
        {
            var requestMessage = new RequestMessage("volumeup", "ssap://audio/volumeUp");
            await _connection.SendCommandAsync(requestMessage);
        }

        public async Task VolumeDown()
        {
            var requestMessage = new RequestMessage("volumedown", "ssap://audio/volumeDown");
            await _connection.SendCommandAsync(requestMessage);
        }

        public async Task SetVolume(int value)
        {
            if (value < 0 || value > 100)
            {
                return;
            }

            var requestMessage = new RequestMessage("ssap://audio/setVolume", new { volume = value });
            await _connection.SendCommandAsync(requestMessage);
        }

        public async Task SetMute(bool value)
        {
            var requestMessage = new RequestMessage("ssap://audio/setMute", new { mute = value });
            await _connection.SendCommandAsync(requestMessage);
        }

        public async Task ToggleMute()
        {
            await SetMute(!await IsMuted());
        }
    }
}
