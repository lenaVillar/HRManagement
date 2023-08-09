using AutoMapper;
using HR_Management.Models;
using HRManagement.Dtos;
using NuGet.Packaging;

namespace Servitally.Tara.Infrastructure.Repositories.Impl.AutoMapper
{
    public class ToDtoAutoMapper : Profile
    {
        public ToDtoAutoMapper()
        {

            CreateMap<Worker, WorkerToCreateDto>();
            CreateMap<Worker, WorkerDto>()
                    .ForMember(dest => dest.WorkingStartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.WorkingStartDate)));



            CreateMap<Role, RoleDto>();

            CreateMap<Role, RoleToCreateDto>();

        }
    }
}
