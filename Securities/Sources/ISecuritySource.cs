using System;
using System.Threading.Tasks;

namespace Securities.Sources
{
    public interface ISecuritySource
    {
        Task<ISecurity> DownloadAsync(string symbol, DateTime begin, DateTime end);
    }
}
