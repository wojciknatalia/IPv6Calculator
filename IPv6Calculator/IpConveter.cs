using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace IPv6Calculator
{
    public static class IpConveter
    {
        /// <summary>
        /// Calculate full address as a string without colons.
        /// </summary>
        /// <param name="address">IPv6 address without colons.</param>
        /// <returns>The full IPv6 address without colons.</returns>
        public static string CalcFullAddress(string address)
        {
            if (string.IsNullOrEmpty(address)) //(address == "" || address == null)
                return "00000000000000000000000000000000";

            string[] fullAddress = new string[8] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000" };

            address = address.Trim();
            address = address.ToLower();
            string normalizedAddress = address;

            string[] addrPart = normalizedAddress.Split(':');

            normalizedAddress = normalizedAddress.Replace("::", "//");//"**");
            int numberOfColons = Regex.Matches(normalizedAddress, ":").Count;

            for (int i = 0; i < addrPart.Length; i++)
            {
                if (addrPart[i].Length == 0)
                    continue;
                else
                    addrPart[i] = addrPart[i].PadLeft(4, '0');
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
        /// Calculates network range - stard and end addresses, amount of total addresses.
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
            mask >>= (ipAddress.Mask); // Net mask
            ipAddress.NetworkRangeEndNumber = (ipAddress.IPNumber | mask);
            mask = ~mask; // Host mask
            ipAddress.NetworkRangeStartNumber = (ipAddress.IPNumber & mask);

            string startAddress = String.Format("{0:x}", ipAddress.NetworkRangeStartNumber);
            if (startAddress.Length > 32)
                startAddress = startAddress.Substring(1, 32);
            ipAddress.NetworkRangeStart = AddColons(startAddress) + "/" + ipAddress.Mask;

            string endAddress = String.Format("{0:x}", ipAddress.NetworkRangeEndNumber);
            if (endAddress.Length > 32)
                endAddress = endAddress.Substring(1, 32);
            ipAddress.NetworkRangeEnd = AddColons(endAddress) + "/" + ipAddress.Mask;

            ipAddress.TotalIPAddresses = ipAddress.NetworkRangeEndNumber - ipAddress.NetworkRangeStartNumber + BigInteger.One;

            return ipAddress;
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

            int[] dist = new int[2];
            for (int i = 0; i < 8; i++)
            {
                if (filledParts[i] != 0)
                {
                    nonZero = i; //zero = i;
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
