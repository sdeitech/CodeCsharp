using App.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Profiles.UserMapper
{

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Entity → Domain
            CreateMap<UserEntity, Users>()
                .ConstructUsing(src => new Users(
                    src.Email,
                    src.UserName,        // DB stores UserName only
                    string.Empty,        // DB has no LastName
                    src.Password_hash,
                    0,                   // (adjust if needed)
                    src.IsActive ? 1 : 0,
                    src.OrganizationId
                ))
                .AfterMap((src, dest) =>
                {
                    dest.SetId(src.UserID); // preserve DB identity
                });

            // Domain → Entity
            CreateMap<Users, UserEntity>()
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FirstName)) // DB only has UserName
                .ForMember(dest => dest.Password_hash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == 1))
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId.HasValue ? src.OrganizationId.Value : (int?)null));
        }
    }

}
