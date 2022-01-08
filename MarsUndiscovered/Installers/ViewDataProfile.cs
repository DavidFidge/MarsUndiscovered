using AutoMapper;

using MarsUndiscovered.Components;

namespace MarsUndiscovered.Installers
{
    public class ViewDataProfile : Profile
    {
        public ViewDataProfile()
        {
            CreateMap<Monster, MonsterStatus>();
            CreateMap<Player, PlayerStatus>();
        }
    }
}
