using System.Net;


namespace TcpListenerProject.Shared
{
    public class CidrParser
    {
        public static IPNetwork[] Parse(string raw)
        {
            if (String.IsNullOrWhiteSpace(raw))
            {
                throw new InvalidOperationException("Invalid or Empty raw string");
            }

            string[] parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            IEnumerable<IPNetwork> networks = parts.Select(ParseOne);
            return networks.ToArray();
        }

        public static IPNetwork ParseOne(string entry)
        {
            if (IPNetwork.TryParse(entry, out var net))
            {
                return net;
            }

            throw new FormatException($"Failed to parse {entry} entry");
        }
    }
}
