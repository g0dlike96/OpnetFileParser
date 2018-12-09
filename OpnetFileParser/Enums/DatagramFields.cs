namespace OpnetFileParser.Enums
{
    public static class DatagramFields
    {
        public static int StartTimeIndex => 0;
        public static int EndTimeIndex => 2;
        public static int SourceAddressIndex => 3;
        public static int DestinationAddressIndex => 4;
        public static int SourcePortIndex => 5;
        public static int DestinationPortIndex => 6;
        public static int ProtocolIndex => 7;
        public static int PacketsCountIndex => 11;
        public static int BytesCountIndex => 12;
    }
}