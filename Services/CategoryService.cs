using AutoMapper;
using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Models;
using CommerceSystemAPI.Repositories;

namespace CommerceSystemAPI.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(CategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper=mapper;
        }
        public async Task<List<CategoryOutputDTO>>GetAllAsync()
        {
            var categories=await _categoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryOutputDTO>>(categories);
        }
        public async Task<CategoryOutputDTO?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return _mapper.Map<CategoryOutputDTO>(category);
        }
        public async Task AddAsync(CategoryCreateDTO dto)
        {
            var category =_mapper.Map<Category>(dto);

            await _categoryRepository.AddAsync(category);
        }
    }
}
