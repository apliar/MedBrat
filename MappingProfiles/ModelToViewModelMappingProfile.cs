using AutoMapper;
using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Areas.Appointment.Models;
using MedBrat.Areas.Appointment.ViewModels;

namespace MedBrat.MappingProfiles
{
    public class ModelToViewModelMappingProfile : Profile
    {
        public ModelToViewModelMappingProfile()
        {
            CreateMap<Clinic, ClinicViewModel>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.Name));
            CreateMap<Doctor, UserProfileViewModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email != null ? src.Email : "Не указана"));
            CreateMap<User, UserProfileViewModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email != null ? src.Email : "Не указана"));
        }
    }
}
