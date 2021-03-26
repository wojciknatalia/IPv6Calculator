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
            //Test IP addresses:
            //2a01:1d8:2:280::/58
            //2a01:1d8:2:280::/127
            //1:0:0:1:0:0:0:1/1

            Console.WriteLine("Enter an IP Address with mask, e.g. 2a01:1d8:2:280::/58");
            string ipInput = Console.ReadLine();
            while (!IpConveter.ValidateAddress(ipInput))
            {
                Console.WriteLine("This address is not valid. Please, type the correct one.");
                ipInput = Console.ReadLine();
            }

            IpAddress ipAddress = new IpAddress(ipInput);
            ipAddress.FullAddressNoCols = IpConveter.CalcFullAddress(ipAddress.Address); // Without colons
            ipAddress.FullAddressCols = IpConveter.AddColons(ipAddress.FullAddressNoCols);
            ipAddress.ShortAddress = IpConveter.ShortenAddress(ipAddress.FullAddressCols) + "/" + ipAddress.Mask;
            ipAddress.IPNumber = BigInteger.Parse(ipAddress.FullAddressNoCols, System.Globalization.NumberStyles.AllowHexSpecifier);
            ipAddress = IpConveter.CalcRange(ipAddress);
            IpConveter.CompareAddresses(ipAddress);

            Console.WriteLine("IP address: " + ipAddress.Address + "/" + ipAddress.Mask);
            Console.WriteLine("Mask: " + ipAddress.Mask);
            Console.WriteLine("Full IP address: " + IpConveter.AddColons(IpConveter.CalcFullAddress(ipAddress.Address)));
            Console.WriteLine("Short IP addres: " + ipAddress.ShortAddress);
            Console.WriteLine("Network: " + ipAddress.Network);
            Console.WriteLine("Network range: " + IpConveter.AddColons(IpConveter.CalcFullAddress(ipAddress.NetworkRangeStart))
                + " - " + IpConveter.AddColons(IpConveter.CalcFullAddress(ipAddress.NetworkRangeEnd)));
            Console.WriteLine("Total IP addresses: " + ipAddress.TotalIPAddresses);
            Console.WriteLine("Previous full address: " + IpConveter.AddColons(IpConveter.CalcFullAddress(ipAddress.PreviousAddress)));
            Console.WriteLine("Next full address: " + IpConveter.AddColons(IpConveter.CalcFullAddress(ipAddress.NextAddress)));
            Console.WriteLine("Adjacent subnets: ");
            foreach(string addr in ipAddress.AdjacentAddresses)
            {
                Console.WriteLine(addr);
            }

        }
    }
}
