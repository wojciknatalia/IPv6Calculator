using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace IPv6Calculator
{
    public static class IpConveter
    {
        /// <summary>
        /// Validates if provided address is valid IPv6 address.
        /// </summary>
        /// <param name="input">Input argument provided by user.</param>
        /// <returns>True if address is valid, false otherwise.</returns>
        public static bool ValidateAddress(string input)
        {
            try
            {
                bool isValidIp = IPAddress.TryParse(input.Split('/')[0], out IPAddress ipAddress);
                if (isValidIp && ipAddress.AddressFamily == AddressFamily.InterNetworkV6
                && input.Contains(":") && !string.IsNullOrEmpty(input.Split('/')[1]))
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Calculate full address as a string without colons.
        /// </summary>
        /// <param name="address">IPv6 address without colons.</param>
        /// <returns>The full IPv6 address without colons.</returns>
        public static string CalcFullAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                return "00000000000000000000000000000000";

            if (address.Contains('/'))
            {
                address = address.Split('/')[0];
            }

            string[] fullAddress = new string[8] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000" };

            address = address.Trim();
            address = address.ToLower();
            string normalizedAddress = address;

            string[] addrPart = normalizedAddress.Split(':');

            normalizedAddress = normalizedAddress.Replace("::", "//");
            int numberOfColons = Regex.Matches(normalizedAddress, ":").Count;

            for (int i = 0; i < addrPart.Length; i++)
            {
                if (addrPart[i].Length == 0)
                    continue;
                else
                    addrPart[i] = addrPart[i].PadLeft(4, '0'); // Add ommited zeros
            }

            if ((addrPart[addrPart.Length - 1].Length == 0) && (addrPart[addrPart.Length - 2].Length == 0))
            {
                int num = numberOfColons + 1;
                for (int i = 0; i < num; i++)
                    fullAddress[i] = addrPart[i];
            }
            else if (addrPart[0].Length == 0 && addrPart[1].Length == 0)
            {
                int num = numberOfColons + 1;
                for (int i = 0; i < num; i++)
                    fullAddress[7 - i] = addrPart[addrPart.Length - 1 - i];
            }
            else
            {
                int id = Array.IndexOf(addrPart, "");

                for (int i = 0; i < id; i++)
                    fullAddress[i] = addrPart[i];

                for (int i = 0; i < addrPart.Length - id - 1; i++)
                    fullAddress[7 - i] = addrPart[addrPart.Length - 1 - i];
            }

            string fullAddressResult = string.Empty;
            for (int i = 0; i < 8; i++)
                fullAddressResult += fullAddress[i];

            return fullAddressResult;
        }

        /// <summary>
        /// Adds colons to full address string.
        /// </summary>
        /// <param name="addressString">The full IPv6 address string without colons.</param>
        /// <returns>The full IPv6 address string.</returns>
        public static string AddColons(string addressString)
        {
            addressString = addressString.Trim();
            string resultAddress = null;

            if (addressString.Length > 32)
            {
                addressString = addressString.PadRight(32, '0');
                for (int i = 0; i < 32; i++)
                {
                    if (i % 4 == 0)
                        resultAddress += ":";
                    resultAddress += addressString.Substring(i, 1);
                }
                resultAddress = resultAddress.Trim(':');
            }
            else
            {
                addressString = addressString.PadLeft(32, '0');
                for (int i = 0; i < 32; i++)
                {
                    if (i % 4 == 0)
                        resultAddress += ":";
                    resultAddress += addressString.Substring(i, 1);
                }
                resultAddress = resultAddress.TrimStart(':');
            }
            return resultAddress;
        }

        /// <summary>
        /// Calculates network range - stard and end addresses, amount of total addresses, previous and next IPv6 addresses.
        /// </summary>
        /// <param name="ipAddress">The IPv6 address.</param>
        /// <returns>IpAddress object with proper data assigned.</returns>
        public static IpAddress CalcRange(IpAddress ipAddress)
        {
            if (ipAddress.Mask == 128)
            {
                    ipAddress.NetworkRangeStartNumber = ipAddress.NetworkRangeEndNumber = ipAddress.IPNumber;
                    return ipAddress;
            }

            BigInteger mask = BigInteger.Pow(2, 128) - BigInteger.One;
            mask >>= (ipAddress.Mask); // Host mask
            ipAddress.NetworkRangeEndNumber = (ipAddress.IPNumber | mask);
            mask = ~mask; // Net mask

            ipAddress.NetworkRangeStartNumber = (ipAddress.IPNumber & mask);

            ipAddress.Network = HexShortAddress((ipAddress.IPNumber & mask), ipAddress.Mask).Split('/')[0]; // Split for removing net mask from string
            ipAddress.NetworkRangeStart = HexAddress(ipAddress.NetworkRangeStartNumber, ipAddress.Mask);
            ipAddress.NetworkRangeEnd = HexAddress(ipAddress.NetworkRangeEndNumber, ipAddress.Mask);

            ipAddress.TotalIPAddresses = ipAddress.NetworkRangeEndNumber - ipAddress.NetworkRangeStartNumber + BigInteger.One;

            BigInteger prevAddressNumber = ipAddress.NetworkRangeStartNumber - ipAddress.TotalIPAddresses;
            ipAddress.PreviousAddress = HexShortAddress(prevAddressNumber, ipAddress.Mask);

            BigInteger nextAddressNumber = ipAddress.NetworkRangeStartNumber + ipAddress.TotalIPAddresses;
            ipAddress.NextAddress = HexShortAddress(nextAddressNumber, ipAddress.Mask);

            return ipAddress;
        }

        /// <summary>
        /// Converts BigInteger number to hex IPv6 address.
        /// </summary>
        /// <param name="addressNumber">The full IPv6 address as BigInteger number.</param>
        /// <param name="mask">The network mask.</param>
        /// <returns>The IPv6 address as a string of hexadecilam digits.</returns>
        public static string HexAddress(BigInteger addressNumber, int mask)
        {
            string address = String.Format("{0:x}", addressNumber);
            if (address.Length > 32)
                address = address.Substring(1, 32);
            return (AddColons(address)) + "/" + mask;
        }

        /// <summary>
        /// Converts BigInteger number to hex and calculates shorten IPv6 address.
        /// </summary>
        /// <param name="addressNumber">The IPv6 address as BigInteger number.</param>
        /// <param name="mask">The network mask.</param>
        /// <returns>The shorten IPv6 address as a string of hexadecilam digits.</returns>
        public static string HexShortAddress(BigInteger addressNumber, int mask)
        {
            string address = String.Format("{0:x}", addressNumber);
            if (address.Length > 32)
                address = address.Substring(1, 32);
            return ShortenAddress(AddColons(address)) + "/" + mask;
        }

        /// <summary>
        /// Assigns list of adjacent subnets addresses with different char marked.
        /// </summary>
        /// <param name="ipAddress">The IpAddress object.</param>
        public static void CompareAddresses(IpAddress ipAddress)
        {
            string addr = (ShortenAddress(ipAddress.FullAddressCols) + "/" + ipAddress.Mask);
            string prevAddr = ipAddress.PreviousAddress;
            string nextAddr = ipAddress.NextAddress;

            string thirdAddress_v1 = MarkAddressesDifference(addr, nextAddr);
            string thirdAddress_v2 = MarkAddressesDifference(prevAddr, nextAddr);
            int minIndex = Math.Min(thirdAddress_v1.IndexOf(' '), thirdAddress_v2.IndexOf(' '));
            int maxIndex = Math.Max(thirdAddress_v1.LastIndexOf(' '), thirdAddress_v2.LastIndexOf(' '));
            string thirdAddr = nextAddr.Insert(minIndex, " ");
            thirdAddr = thirdAddr.Insert(maxIndex, " ");

            ipAddress.AdjacentAddresses = new List<string>()
            {
                MarkAddressesDifference(addr, prevAddr),
                MarkAddressesDifference(prevAddr, addr),
                thirdAddr
            };
        }

        /// <summary>
        /// Add space before and after the different char.
        /// </summary>
        /// <param name="mainAddr">First address to compare.</param>
        /// <param name="comparedAddr">Second address to compare.</param>
        /// <returns>Second argument value with space between different char.</returns>
        public static string MarkAddressesDifference(string mainAddr, string comparedAddr)
        {
            int first = -1;
            int last = -1;
            string result;
            int shorterLength = (comparedAddr.Length < mainAddr.Length ? comparedAddr.Length : mainAddr.Length);

            // Start of difference
            for (int i = 0; i < shorterLength; i++)
            {
                if (mainAddr[i] != comparedAddr[i])
                {
                    first = i;
                    break;
                }
            }

            // End of difference
            char[] a = mainAddr.Reverse().ToArray();
            char[] b = comparedAddr.Reverse().ToArray();
            for (int i = 0; i < shorterLength; i++)
            {
                if (a[i] != b[i])
                {
                    last = i;
                    break;
                }
            }

            if (first == -1 && last == -1)
                result = comparedAddr;
            else
            {
                StringBuilder sb = new StringBuilder();
                if (first == -1)
                    first = shorterLength;
                if (last == -1)
                    last = shorterLength;
                // If same char repeats multiple times and error is on that char, trail has to be trimmed.
                if (first + last > shorterLength)
                    last = shorterLength - first;

                if (first > 0)
                    sb.Append(comparedAddr.Substring(0, first));

                sb.Append(" ");

                if (last > -1 && last + first < comparedAddr.Length)
                    sb.Append(comparedAddr.Substring(first, comparedAddr.Length - last - first));

                sb.Append(" ");

                if (last > 0)
                    sb.Append(comparedAddr.Substring(comparedAddr.Length - last, last));

                result = sb.ToString();
            }
            return result;
        }

        /// <summary>
        /// Calculates shorten IPv6 address with collapsed insignificant zeros.
        /// </summary>
        /// <param name="address">The IPv6 address.</param>
        /// <returns>The shorten IPv6 address.</returns>
        public static string ShortenAddress(string address)
        {
            string resultAddr = "";
            ushort[] partsHex = new ushort[8];
            int[] filledParts = new int[8]; // 1 if part is not empty, 0 if part is empty
            
            string[] parts = address.Split(':');
            for (int i = 0; i < 8; i++)
            {
                partsHex[i] = ushort.Parse(parts[i], System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            // Greater than zero parts
            for (int i = 0; i < 8; i++)
            {
                if (partsHex[i] > 0)
                    filledParts[i] = 1;
            }

            // Length of zero parts
            int nonZero = 0;
            int max = 0;
            int zero, diff;

            int[] dist = new int[2]; // Stard and end of ommited zeros
            for (int i = 0; i < 8; i++)
            {
                if (filledParts[i] != 0)
                {
                    nonZero = i;
                    nonZero++;
                }
                else
                {
                    zero = i;
                    diff = zero - nonZero;
                    if (diff > max)
                    {
                        max = diff;
                        dist[0] = nonZero;
                        dist[1] = zero;
                    }
                }
            }

            if (dist[0] == dist[1])
            {
                string result = "";
                foreach (ushort part in partsHex)
                {
                    if (part == 0)
                        result += "#" + ":";
                    else
                        result += part.ToString("x") + ":";
                }
                result = result.TrimEnd(':');

                int zeroCount = result.Split('#').Length - 1;

                if (zeroCount == 1)
                {
                    result = result.Replace("#", ":");
                }
                else if (zeroCount > 1)
                {
                    var regexp = new Regex(Regex.Escape("#"));
                    result = regexp.Replace(result, ":", 1);
                }
                result = result.Replace(":::", "::");
                result = result.Replace('#', '0');

                return result;
            }

            for (int i = 0; i < 8; i++)
            {
                if (i == dist[0])
                    resultAddr += "::";
                if (i < dist[0] || i > dist[1])
                {
                    if (i != 7)
                        resultAddr += partsHex[i].ToString("x") + ":";
                    else
                        resultAddr += partsHex[i].ToString("x");
                }
            }
            resultAddr = resultAddr.Replace(":::", "::");

            return resultAddr;
        }
    }
}
