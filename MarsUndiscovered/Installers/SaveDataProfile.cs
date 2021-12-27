using System.Linq;

using AutoMapper;
using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Installers
{
    public class SaveDataProfile : Profile
    {
        public SaveDataProfile()
        {
            CreateMap<IGameObject, GameObjectSaveData>()
                .Include<Wall, WallSaveData>()
                .Include<Floor, FloorSaveData>()
                .Include<Player, PlayerSaveData>();
        }
	}
}
