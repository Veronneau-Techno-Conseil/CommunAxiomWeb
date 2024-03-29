﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrowser
{
    public class Metadata
    {
        public Uri Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? PublishedTime { get; set; }
        public Uri Image { get; set; }
    }
}
