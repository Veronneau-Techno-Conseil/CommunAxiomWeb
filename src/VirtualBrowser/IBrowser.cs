using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrowser
{
    public interface IBrowser
    {
        Task<Metadata> GetMetadata(string url);
    }
}
