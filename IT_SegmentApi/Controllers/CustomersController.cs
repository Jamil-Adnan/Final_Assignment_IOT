using IT_SegmentApi.Data;
using IT_SegmentApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static IT_SegmentApi.DTOs.CustomerDto;

namespace IT_SegmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IOTFinalDbContext _context;

        public CustomersController(IOTFinalDbContext context)
        {
            _context = context;
        }

        // POST: api/customers/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCustomerDto dto)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == dto.Email))
                return BadRequest("Email already registered.");

            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Postcode = dto.Postcode,
                City = dto.City,
                Country = dto.Country,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(dto.HashPassword)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(new { customer.CustomerId, customer.FirstName, customer.LastName, customer.Email });
        }

        // POST: api/customers/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.Email);
            if (customer == null || !BCrypt.Net.BCrypt.Verify(dto.HashPassword, customer.HashPassword))
                return Unauthorized("Invalid email or password.");

            return Ok(new { customer.CustomerId, customer.FirstName, customer.LastName, customer.Email });
        }
    }
}
