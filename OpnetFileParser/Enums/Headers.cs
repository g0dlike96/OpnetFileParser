using System.Collections.Generic;

namespace OpnetFileParser.Enums
{
    public static class Headers
    {
        public static List<string> HeadersForTr1FileFormat =>
            new List<string>
            {
                "#Each line represents a uniform level of traffic over the specified interval",
                "#Fields are:source_node, dest_node, time_window, packets_per_sec, bits_per_sec",
                "traffic:"
            };

        public static List<string> HeadersForTr2FileFormat =>
            new List<string>
            {
                "#NOTE: the following two lines are required",
                "traffic:",
                "source,destination,protocol,source_port,destination_port,start,end,bits_sec,packets_sec"
            };
    }
}
