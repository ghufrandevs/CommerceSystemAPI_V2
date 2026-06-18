using AutoMapper;
using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;

namespace CommerceSystemAPI.Profiles
{
    public class SupplierProfile :Profile
    {
        public SupplierProfile()
        {
            CreateMap<SupplierCreateDTO, Supplier>();
            CreateMap<SupplierUpdateDTO, Supplier>();
            CreateMap<Supplier, SupplierOutputDTO>();
        }
    }
}
