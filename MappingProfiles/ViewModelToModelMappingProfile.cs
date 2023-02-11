using AutoMapper;
using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using MedBrat.Areas.Appointment.Models;
using MedBrat.Areas.Appointment.ViewModels;

namespace MedBrat.MappingProfiles
{
    public class ViewModelToModelMappingProfile : Profile
    {
        public ViewModelToModelMappingProfile()
        {
            CreateMap<DoctorAddViewModel, Doctor>();
            CreateMap<UserRegistrationViewModel, Patient>();
        }
    }
}
