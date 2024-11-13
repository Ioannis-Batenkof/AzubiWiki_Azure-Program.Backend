using AutoMapper;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.DTOs;
using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CarQ, Car>()
                .ForMember(target => target.ID, source => source.MapFrom(source => source.ID))
                .ForMember(target => target.Manufacturer, source => source.MapFrom(source => source.Manufacturer))
                .ForMember(target => target.Model, source => source.MapFrom(source => source.Model))
                .ForMember(target => target.Year, source => source.MapFrom(source => source.Year))
                .ForMember(target => target.Horsepower, source => source.MapFrom(source => source.Horsepower))
                .ForMember(target => target.Available, source => source.MapFrom(source => source.Available));

            CreateMap<Car, CarQ>()
                .ForMember(target => target.ID, source => source.MapFrom(source => source.ID))
                .ForMember(target => target.Manufacturer, source => source.MapFrom(source => source.Manufacturer))
                .ForMember(target => target.Model, source => source.MapFrom(source => source.Model))
                .ForMember(target => target.Year, source => source.MapFrom(source => source.Year))
                .ForMember(target => target.Horsepower, source => source.MapFrom(source => source.Horsepower))
                .ForMember(target => target.Available, source => source.MapFrom(source => source.Available));

            CreateMap<Car, CarDTO>()
                .ForMember(target => target.ID, source => source.MapFrom(source => source.ID))
                .ForMember(target => target.Manufacturer, source => source.MapFrom(source => source.Manufacturer))
                .ForMember(target => target.Model, source => source.MapFrom(source => source.Model))
                .ForMember(target => target.Year, source => source.MapFrom(source => source.Year))
                .ForMember(target => target.Horsepower, source => source.MapFrom(source => source.Horsepower))
                .ForMember(target => target.Available, source => source.MapFrom(source => source.Available));



            CreateMap<GarageQ, Garage>()
                .ForMember(target => target.ID, source => source.MapFrom(source => source.ID))
                .ForMember(target => target.BelongsTo, source => source.MapFrom(source => source.BelongsTo));

            CreateMap<Garage, GarageDTO>()
                .ForMember(target => target.ID, source => source.MapFrom(source => source.ID))
                .ForMember(target => target.BelongsTo, source => source.MapFrom(source => source.BelongsTo))
                .ForMember(target => target.Cars, source => source.MapFrom(source => source.Cars));


        }
    }
}
