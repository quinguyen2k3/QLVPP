using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CategoryRes>> GetAll()
        {
            var categories = await _unitOfWork.Category.GetAll();
            return _mapper.Map<List<CategoryRes>>(categories);
        }

        public async Task<List<CategoryRes>> GetAllActived()
        {
            var categories = await _unitOfWork.Category.GetAllIsActived();
            return _mapper.Map<List<CategoryRes>>(categories);
        }

        public async Task<CategoryRes?> GetById(long id)
        {
            var category = await _unitOfWork.Category.GetById(id);
            return category == null ? null : _mapper.Map<CategoryRes>(category);
        }

        public async Task<CategoryRes> Create(CategoryReq request)
        {
            var category = _mapper.Map<Category>(request);

            await _unitOfWork.Category.Add(category);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<CategoryRes>(category);
        }

        public async Task<CategoryRes?> Update(long id, CategoryReq request)
        {
            var category = await _unitOfWork.Category.GetById(id);
            if (category == null)
                return null;

            _mapper.Map(request, category);

            await _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<CategoryRes>(category);
        }   
    }
}
