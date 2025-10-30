using AutoMapper;
using BankingApi.Application.DTOs;
using BankingApi.Core.Entities;

namespace BankingApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Account, AccountResponse>();
    }
}
