﻿using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace LgTv.Info
{
    public class LgTvInfoClient : ILgTvInfoClient
    {
        private readonly ILgTvConnection _connection;

        public LgTvInfoClient(
            ILgTvConnection connection)
        {
            _connection = connection;
        }

        public async Task<SystemInformation> GetSystemInfo()
        {
            var requestMessage = new RequestMessage(string.Empty, "ssap://system/getSystemInfo");
            var response = await _connection.SendCommandAsync(requestMessage);

            SystemFeatures features = null;
            if (response.features != null)
            {
                var featureDictionary = (IDictionary<string, object>) response.features;

                features = new SystemFeatures
                {
                    _3D = (bool) featureDictionary["3d"],
                    DVR = (bool) featureDictionary["dvr"]
                };
            }

            var systemInfo = new SystemInformation
            {
                Features = features,
                ReceiverType = response.receiverType,
                ModelName = response.modelName,
                ProgramMode = (bool) response.programMode
            };

            return systemInfo;
        }

        public async Task<SoftwareInformation> GetSoftwareInfo()
        {
            var requestMessage = new RequestMessage(string.Empty, "ssap://com.webos.service.update/getCurrentSWInformation");
            var response = await _connection.SendCommandAsync(requestMessage);

            CultureInfo language = null;
            if (response.language_code != null)
            {
                language = new CultureInfo((string) response.language_code);
            }

            var softwareInfo = new SoftwareInformation
            {
                ProductName = response.product_name,
                ModelName = response.model_name,
                SoftwareType = response.sw_type,
                MajorVersion = response.major_ver,
                MinorVersion = response.minor_ver,
                Country = response.country,
                CountryGroup = response.country_group,
                DeviceId = response.device_id,
                AuthFlag = response.auth_flag,
                IgnoreDisable = response.ignore_disable,
                EcoInfo = response.eco_info,
                ConfigKey = response.config_key,
                Language = language
            };

            return softwareInfo;
        }

        public async Task<IEnumerable<Service>> GetServices()
        {
            var requestMessage = new RequestMessage(string.Empty, "ssap://api/getServiceList");
            var response = await _connection.SendCommandAsync(requestMessage);

            var services = new List<Service>();
            foreach (var service in response.services)
            {
                services.Add(new Service
                {
                    Name = service.name,
                    Version = int.Parse((string) service.version)
                });
            }

            return services;
        }
    }
}
