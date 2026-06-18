using AutoMapper;
using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;

namespace CommerceSystemAPI.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryOutputDTO>();
        }
    }
}
