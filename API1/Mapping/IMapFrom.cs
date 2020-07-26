using AutoMapper;

namespace WeatherServiceApi.Mapping
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile)
        {
            profile.CreateMap(typeof(T), this.GetType());
            profile.CreateMap(this.GetType(), typeof(T));
        }
    }
}