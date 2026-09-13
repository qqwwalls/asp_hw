using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.DTOs;
using ProductsApi.Models;

namespace ProductsApi.Services;

public class DeliveryAddressService : IDeliveryAddressService
{
    private readonly AppDbContext _context;

    public DeliveryAddressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryAddressReadDto> AddAddressAsync(int userId, DeliveryAddressCreateDto dto, CancellationToken cancellationToken)
    {
        var address = new DeliveryAddress
        {
            UserId = userId,
            Country = dto.Country,
            City = dto.City,
            Street = dto.Street,
            ZipCode = dto.ZipCode
        };

        _context.DeliveryAddresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return new DeliveryAddressReadDto
        {
            Id = address.Id,
            Country = address.Country,
            City = address.City,
            Street = address.Street,
            ZipCode = address.ZipCode
        };
    }

    public async Task<IEnumerable<DeliveryAddressReadDto>> GetUserAddressesAsync(int userId, CancellationToken cancellationToken)
    {
        var addresses = await _context.DeliveryAddresses
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        return addresses.Select(a => new DeliveryAddressReadDto
        {
            Id = a.Id,
            Country = a.Country,
            City = a.City,
            Street = a.Street,
            ZipCode = a.ZipCode
        });
    }
}
