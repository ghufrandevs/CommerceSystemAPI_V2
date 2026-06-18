using AutoMapper;
using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;
using CommerceSystemAPI.Repositories;

namespace CommerceSystemAPI.Services
{
    public class SupplierService
    {
        private readonly SupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierService(
            SupplierRepository supplierRepository,
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<List<SupplierOutputDTO>> GetAllAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();

            return _mapper.Map<List<SupplierOutputDTO>>(suppliers);
        }

        public async Task<SupplierOutputDTO?> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                return null;
            }

            return _mapper.Map<SupplierOutputDTO>(supplier);
        }

        public async Task AddAsync(SupplierCreateDTO dto)
        {
            var supplier = _mapper.Map<Supplier>(dto);

            await _supplierRepository.AddAsync(supplier);
        }
    }
}

