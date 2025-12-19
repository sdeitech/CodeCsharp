using App.Application.Dto.MasterSetting;
using App.Application.Dto.MasterSystemSettings;
using App.Domain.Entities.MasterSettings;
using App.Domain.Entities.MasterSystemSettings;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Profiles.SystemSettings
{
    public class SystemSettingMappingProfile : Profile
    {
        public SystemSettingMappingProfile()
        {
            CreateMap<MasterSystemSetting, MasterSystemSettingsDto>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
      .ReverseMap();

            CreateMap<MasterSetting, MasterSettingsDto>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
      .ReverseMap();
        }
    }
}
