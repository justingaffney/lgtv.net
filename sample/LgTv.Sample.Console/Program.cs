﻿using System.IO;
using System.Threading.Tasks;
using LgTv.Mouse;

namespace LgTv.Sample.Console
{
    public class Program
    {
        private const string ClientKeyStoreFileName = "client-keys.json";

        private static readonly string ClientKeyStoreFilePath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), ClientKeyStoreFileName);

        private const bool SecureConnection = false;
        private const string TvHostname = "ENTER_HOSTNAME_OR_IP_ADDRESS";
        private const int TvPort = 3000;

        public static async Task Main(string[] args)
        {
            // Initialization
            var client = new LgTvClient(
                () => new LgTvConnection(),
                new JsonFileClientKeyStore(ClientKeyStoreFilePath),
                SecureConnection, TvHostname, TvPort);

            await client.Connect();
            await client.MakeHandShake();


            // Volume control
            await client.Audio.VolumeDown();
            await client.Audio.VolumeUp();


            // Playback control
            await client.Playback.Pause();
            await client.Playback.Play();


            var channels = await client.Channels.GetChannels();

            var inputs = await client.Inputs.GetInputs();

            var apps = await client.Apps.GetApps();


            using (var mouse = await client.GetMouse())
            {
                await mouse.SendButton(ButtonType.Up);
                await mouse.SendButton(ButtonType.Left);
                await mouse.SendButton(ButtonType.Right);
                await mouse.SendButton(ButtonType.Down);
            }


            await client.Power.TurnOff();
        }
    }
}
