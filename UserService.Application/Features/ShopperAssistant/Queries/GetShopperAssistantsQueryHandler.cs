using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Common;
using UserService.Application.DTOs.Response;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Data;

namespace UserService.Application.Features.ShopperAssistant.Queries;

public record GetShopperAssistantsQuery(
    int? PageNumber = null,
    int? PageSize = null) : IRequest<Pagination<ShopperAssistantResponse>>;

public class GetShopperAssistantsQueryHandler : IRequestHandler<GetShopperAssistantsQuery, Pagination<ShopperAssistantResponse>>
{
    private readonly IUserServiceDbContext _dbContext;
    private readonly ILogger<GetShopperAssistantsQueryHandler> _logger;

    public GetShopperAssistantsQueryHandler(
        IUserServiceDbContext dbContext,
        ILogger<GetShopperAssistantsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<Pagination<ShopperAssistantResponse>> Handle(GetShopperAssistantsQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber.HasValue && request.PageNumber.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }

        if (request.PageSize.HasValue && request.PageSize.Value <= 0)
        {
            throw new IncorrectPaginationException("Page size must be greater than 0");
        }
        
        var query = _dbContext.ShopperAssistants.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            query = query.OrderBy(sh => sh.Id)
                .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value);
        }

        var shAssistant = await query.OrderBy(sh => sh.Id)
            .Select(sh => new ShopperAssistantResponse
            {
                Id = sh.Id,
                Name = sh.Name,
                Surname = sh.Surname,
                Username = sh.Username,
                Email = sh.Email,
                PhoneNumber = sh.PhoneNumber,
                MarketId = sh.MarketId,
                Role = sh.Role,
                Position = sh.Position,
                CreatedAt = sh.CreatedAt,
                UpdatedAt = sh.UpdatedAt,
                DeletedAt = sh.DeletedAt
            }).ToListAsync(cancellationToken);

        return new Pagination<ShopperAssistantResponse>
        {
            Items = shAssistant,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalCount
        };
    }
}