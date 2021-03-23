using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Numerics;

namespace IPv6Calculator
{
    public class StartingService : IStartingService
    {
        private readonly ILogger<StartingService> _logger;
        private readonly IConfiguration _configuration;

        public StartingService(ILogger<StartingService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void Run()
        {
            string ipInput = "2a01:1d8:2:280::/127";
            _logger.LogInformation("Hardcoded ipInput: " + ipInput);

            IpAddress ipAddress = new IpAddress(ipInput);
            _logger.LogInformation("IPAddress params, address: " + ipAddress.Address + ", mask: " + ipAddress.Mask);

            ipAddress.FullAddressNoCols = IpConveter.CalcFullAddress(ipAddress.Address); //without colons
            ipAddress.FullAddressCols = IpConveter.AddColons(ipAddress.FullAddressNoCols);
            _logger.LogInformation("Full addr: " + ipAddress.FullAddressCols);

            ipAddress.IPNumber = BigInteger.Parse(ipAddress.FullAddressNoCols, System.Globalization.NumberStyles.AllowHexSpecifier);
            _logger.LogInformation("IpNumber: " + ipAddress.IPNumber);

            ipAddress = IpConveter.CalcRange(ipAddress);

            _logger.LogInformation("StartAddr: " + ipAddress.NetworkRangeStart);
            _logger.LogInformation("EndAddr: " + ipAddress.NetworkRangeEnd);
            _logger.LogInformation("Total addresses: " + ipAddress.TotalIPAddresses);
            _logger.LogInformation("Shorten address: " + IpConveter.ShortenAddress(IpConveter.AddColons(ipAddress.FullAddressNoCols)));

        }
    }
}
