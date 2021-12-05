using System;

namespace CommunAxiomWeb.Models
{
    public class TLGroup
    {
        public string group{get;set;}
        public TLLabel[] data{get;set;}

    }

    public class TLLabel
    {
        public string label{get;set;}
        public TLSegments[] data{get;set;}
    }

    public class TLSegments{
        public DateTime[] timeRange{get;set;}
        public string val {get;set;}
    }
}