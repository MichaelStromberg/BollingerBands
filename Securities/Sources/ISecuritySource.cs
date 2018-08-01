using System.Threading.Tasks;

namespace Securities.Sources
{
    public interface ISecuritySource
    {
        Task<ISecurity> DownloadFiveYearsAsync(string symbol);
    }
}
