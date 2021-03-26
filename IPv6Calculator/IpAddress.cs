using System.Collections.Generic;
using System.Numerics;

namespace IPv6Calculator
{
    public class IpAddress
    {
        /// <summary>
        /// IPv6 address, e.g. 2a01:1d8:2:280::
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// IPv6 shorten address
        /// </summary>
        public string ShortAddress { get; set; }

        /// <summary>
        /// Full IPv6 address without colums, e.g. 2a0101d8000202800000000000000000
        /// </summary>
        public string FullAddressNoCols { get; set; }

        /// <summary>
        /// Full IPv6 address without colums, e.g. 2a01:01d8:0002:0280:0000:0000:0000:0000
        /// </summary>
        public string FullAddressCols { get; set; }

        /// <summary>
        /// IPv6 Address as IP number
        /// </summary>
        public BigInteger IPNumber { get; set; }

        /// <summary>
        /// Mask, e.g. 128
        /// </summary>
        public int Mask { get; set; }

        /// <summary>
        /// Network, e.g. 2800:: for 2a01:1d8:2:280::/6
        /// </summary>
        public string Network { get; set; }

        /// <summary>
        /// Network range start addres as IPNumber
        /// </summary>
        public BigInteger NetworkRangeStartNumber { get; set; }

        /// <summary>
        /// Network range end address as IPNumber
        /// </summary>
        public BigInteger NetworkRangeEndNumber { get; set; }

        /// <summary>
        /// Network range start addres, e.g. 2800:0000:0000:0000:0000:0000:0000:0000 for 2a01:1d8:2:280::/6
        /// </summary>
        public string NetworkRangeStart { get; set; }

        /// <summary>
        /// Network range end address, e.g. 2bff:ffff:ffff:ffff:ffff:ffff:ffff:ffff for 2a01:1d8:2:280::/6
        /// </summary>
        public string NetworkRangeEnd { get; set; }

        /// <summary>
        /// The amount of total IP addresses, e.g. 2 for 2a01:1d8:2:280::/127
        /// </summary>
        public BigInteger TotalIPAddresses { get; set; }

        /// <summary>
        /// Adjacent previous IPv6 address.
        /// </summary>
        public string PreviousAddress { get; set; }

        /// <summary>
        /// Adjacent next IPv6 address.
        /// </summary>
        public string NextAddress { get; set; }

        /// <summary>
        /// List of the adjacent IPv6 addresses with difference marked.
        /// </summary>
        public List<string> AdjacentAddresses { get; set; }

        public IpAddress(string address)
        {
            Address = address.Split('/')[0];
            Mask = short.Parse(address.Split('/')[1]);
        }

    }
}
