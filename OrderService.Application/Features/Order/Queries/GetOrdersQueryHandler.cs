using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Order.Queries;

public record GetOrdersQuery(
    int? PageNumber = null,
    int? PageSize = null,
    OrderStatus? Status = null,
    DateTime? CreatedAtFrom = null,
    DateTime? CreatedAtTo = null) : IRequest<Pagination<OrderResponse>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Pagination<OrderResponse>>
{
    private readonly IOrderServiceDbContext _dbContext;
    private readonly ILogger<GetOrdersQueryHandler> _logger;
    private readonly IHttpContextAccessor _accessor;

    public GetOrdersQueryHandler(
        IOrderServiceDbContext dbContext,
        ILogger<GetOrdersQueryHandler> logger, 
        IHttpContextAccessor accessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _accessor = accessor;
    }
    public async Task<Pagination<OrderResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber.HasValue && request.PageNumber.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }
        
        if (request.PageSize.HasValue && request.PageSize.Value <= 0)
        {
            throw new IncorrectPaginationException("Page number must be grater than 0");
        }

        var role = _accessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        if (role == null)
        {
            throw new NotAuthorizedException();
        }

        var query = _dbContext.Orders.OrderBy(o=>o.Id).AsQueryable();

        if (role == "MarketAdmin" || role == "ShopperAssistant")
        {
            var marketIdStr = _accessor.HttpContext?.User.FindFirst("MarketId")?.Value;
            if (marketIdStr == null)
            {
                throw new NotAuthorizedException();
            }
            
            long.TryParse(marketIdStr, out var marketId);
            
            query = query.Where(o => o.MarketId == marketId);
        }

        if (role == "Customer")
        {
            var customerIdStr = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerIdStr == null)
            {
                throw new NotAuthorizedException();
            }
                    
            long.TryParse(customerIdStr, out var customerId);
            
            query = query.Where(o => o.CustomerId == customerId);
        }
        
        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        if (request.CreatedAtFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= request.CreatedAtFrom);
        }
        
        if (request.CreatedAtTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= request.CreatedAtFrom);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
        }

        var orders = await query
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                MarketId = o.MarketId,
                ShopperAssistantId = o.ShopperAssistantId,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                CompletedAt = o.CompletedAt
            }).ToListAsync(cancellationToken);

        return new Pagination<OrderResponse>
        {
            Items = orders,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}