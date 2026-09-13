using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProductsApi.DTOs;

namespace ProductsApi.Services;

public interface IDeliveryAddressService
{
    Task<DeliveryAddressReadDto> AddAddressAsync(int userId, DeliveryAddressCreateDto dto, CancellationToken cancellationToken);
    Task<IEnumerable<DeliveryAddressReadDto>> GetUserAddressesAsync(int userId, CancellationToken cancellationToken);
}
