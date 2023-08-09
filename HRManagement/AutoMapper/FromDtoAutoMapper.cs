using AutoMapper;
using HR_Management.Models;
using HRManagement.Dtos;

namespace HRManagement.AutoMapper
{
    public class FromDtoAutoMapper : Profile
    {

        public FromDtoAutoMapper()
        {
            CreateMap<WorkerToCreateDto, Worker>();
            CreateMap<WorkerToUpdateDto, Worker>();
            CreateMap<RoleToCreateDto, Role>();

            CreateMap<RoleDto, Role>();
            CreateMap<WorkerDto, Worker>();
        }
    }
}
