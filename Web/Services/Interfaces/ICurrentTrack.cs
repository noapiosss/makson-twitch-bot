using System.Threading.Tasks;

namespace Web.Services.Interfaces
{
    public interface ISpotifyAPI
    {
        public Task<string> GetCurrentPlayingTrack();
    }
}