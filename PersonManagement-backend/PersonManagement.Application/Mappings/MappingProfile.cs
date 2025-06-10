using AutoMapper;
using PersonManagement.Application.DTOs;
using PersonManagement.Domain.Entities;

namespace PersonManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Person to PersonDto
            CreateMap<Person, PersonDto>();

            // CreatePersonDto to Person
            CreateMap<CreatePersonDto, Person>();

            // UpdatePersonDto to Person
            CreateMap<UpdatePersonDto, Person>();
        }
    }
}