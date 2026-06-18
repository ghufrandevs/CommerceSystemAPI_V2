using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/Supplier")]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierService _supplierService;

        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [AllowAnonymous]
        [HttpGet("GetAllSuppliers")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllAsync();

            return Ok(suppliers);
        }

        [AllowAnonymous]
        [HttpGet("GetSupplierById")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);

            if (supplier == null)
            {
                return NotFound("Supplier Not Found");
            }

            return Ok(supplier);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddSupplier")]
        public async Task<IActionResult> AddSupplier(SupplierCreateDTO dto)
        {
            await _supplierService.AddAsync(dto);

            return Ok("Supplier Added Successfully");
        }
    }
}
