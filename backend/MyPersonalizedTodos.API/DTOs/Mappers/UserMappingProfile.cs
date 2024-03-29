﻿using AutoMapper;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.Enums;
using MyPersonalizedTodos.API.Extensions;

namespace MyPersonalizedTodos.API.DTOs.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<RegisterUserDTO, User>()
                .ForMember(user => user.Name, configExpression => configExpression.MapFrom(dto => dto.Login))
                .ForMember(user => user.Gender, configExpression => configExpression.MapFrom(dto => dto.Gender.ConvertToEnum<UserGender>()))
                .ForMember(user => user.Purposes, configExpression => configExpression.MapFrom(dto => dto.Purposes.Select(x => x.ConvertToEnum<Purpose>()).ToList()))
                .ForMember(user => user.Nationality, configExpression => configExpression.MapFrom(dto => dto.Nationality.ConvertToEnum<UserNationality>()))
                .ForMember(user => user.Settings, configExpression => configExpression.MapFrom(_ => new UserSettings()));
        }
    }
}
