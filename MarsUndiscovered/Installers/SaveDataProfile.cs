using System.Linq;

using AutoMapper;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Installers
{
    public class SaveDataProfile : Profile
    {
        public SaveDataProfile()
        {
            CreateMap<Terrain, TerrainSaveData>()
                .Include<Wall, WallSaveData>()
                .Include<Floor, FloorSaveData>();

            CreateMap<Player, PlayerSaveData>();
        }
	}
}
