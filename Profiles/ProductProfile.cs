using AutoMapper;
using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;

namespace CommerceSystemAPI.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDTO, Product>();

            CreateMap<ProductUpdateDTO, Product>();

            // Custom Mapping because CategoryName and SupplierName
            // are inside navigation properties, not directly in Product.
            CreateMap<Product, ProductOutputDTO>()
                .ForMember(
                    dest => dest.CategoryName,
                    opt => opt.MapFrom(src =>
                        src.Category != null
                            ? src.Category.CategoryName
                            : null))

                .ForMember(
                    dest => dest.SupplierName,
                    opt => opt.MapFrom(src =>
                        src.Supplier != null
                            ? src.Supplier.SupplierName
                            : null));
        }
    }
}
