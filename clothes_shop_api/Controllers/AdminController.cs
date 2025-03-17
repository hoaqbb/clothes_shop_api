using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs;
using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ecommerce_decryptedContext _context;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWork, ecommerce_decryptedContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("get-all-product")]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetAllProduct([FromQuery] AdminProductParams adminProductParams)
        {
            var productList = await _unitOfWork.ProductRepository.GetAllProductsAsync(adminProductParams);

            Response.AddPaginationHeader(productList.CurrentPage, productList.PageSize,
                productList.TotalCount, productList.TotalPages);

            return Ok(productList);
        }

        [HttpGet("get-all-order")]
        public async Task<ActionResult<List<OrderListDto>>> GetAllOrder([FromQuery] AdminOrderParams paginationParams)
        {
            var orderList = await _unitOfWork.OrderRepository.GetAllOrderAsync(paginationParams);

            Response.AddPaginationHeader(orderList.CurrentPage, orderList.PageSize,
                orderList.TotalCount, orderList.TotalPages);

            return Ok(orderList);
        }

        [HttpGet("get-product-detail/{id}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductDetailById(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(id);

            if (product != null)
                return Ok(product);
            return NotFound("Product not found!");
        }

        [HttpGet("overview")]
        public async Task<ActionResult<RevenueDto>> OverviewRevenue()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            //count number of orders in current month
            var monthlyOrders = await _context.Orders
                .Where(x => x.CreateAt.Month == currentMonth
                    && x.CreateAt.Year == currentYear
                    && x.Status <= 3)
                .CountAsync();

            //calculate revenue in current month
            //only orders with status = 3(delivery successful)
            var monthlyRevenue = await _context.Orders
                .Where(x => x.CreateAt.Month == currentMonth
                    && x.CreateAt.Year == currentYear
                    && x.Status == 3)
                .SumAsync(s => s.Amount - s.ShippingFee);

            //best selling products list
            //10 best selling products in first 20 orders
            var bestSellingProducts = await _context.OrderItems
                .Where(oi => _context.Orders
                    .Where(o => o.Status <=3)
                    .OrderByDescending(o => o.CreateAt)
                    .Take(20)
                    .Select(o => o.Id)
                    .Contains(oi.OrderId))
                .GroupBy(p => new
                {
                    ProductId = p.QuantityNavigation.ProductId,
                    ProductName = p.QuantityNavigation.Product.Name,
                    ProductImage = p.QuantityNavigation.Product.ProductImages
                        .FirstOrDefault(x => x.IsMain).ImageUrl
                })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    ProductImage = g.Key.ProductImage,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToListAsync();

            //list of orders waiting to be confirmed
            var unshippedOrders = await _context.Orders
                .Where(o => o.Status < 2)
                .OrderByDescending(o => o.CreateAt)
                .ProjectTo<OrderListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            //total products sold for each category of top 20 orders
            var categorySales = await _context.OrderItems
                .Where(oi => _context.Orders
                    .OrderByDescending(o => o.CreateAt)
                    .Take(20)
                    .Select(o => o.Id)
                    .Contains(oi.OrderId))
                .GroupBy(p => p.QuantityNavigation.Product.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    CategoryName = g.FirstOrDefault().QuantityNavigation.Product.Category.Name,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToListAsync();

            var result = new RevenueDto();
            result.MonthlyOrders = monthlyOrders;
            result.MonthlyRevenue += monthlyRevenue;
            result.BestSellingProducts = bestSellingProducts;
            result.UnshippedOrders = unshippedOrders;
            result.CategorySales = categorySales;

            return Ok(result);
        }

        [HttpGet("revenue")]
        public async Task<ActionResult> CalculateRevenue([FromQuery]int? year)
        {
            var years = await _context.Orders
                .Where(x => x.Status == 3)
                .GroupBy(x => x.CreateAt.Year)
                .Select(p => p.Key)
                .ToListAsync();

            if(!years.Any()) years = new List<int> { 0 };
            
            int selectedYear = 0;
            if (year.HasValue) selectedYear = year.Value;
            else selectedYear = years.Last();
            
            List<int> months = Enumerable.Range(1, 12).ToList();

            var monthlyRevenue = await _context.Orders
                .Where(o => o.Status == 3 && o.CreateAt.Year == selectedYear)
                .GroupBy(o => o.CreateAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalOrder = g.Count(),
                    Revenue = g.Sum(o => o.Amount - o.ShippingFee)
                })
                .ToListAsync();

            var result = months.GroupJoin(
                    monthlyRevenue,
                    month => month,
                    revenue => revenue.Month,
                    (month, revenueGroup) => new
                    {
                        Month = month,
                        TotalOrder = revenueGroup.FirstOrDefault()?.TotalOrder ?? 0,
                        Revenue = revenueGroup.FirstOrDefault()?.Revenue ?? 0
                    }).ToList();

            var orderOverview = await _context.Orders
                    .Where(x => x.Status == 3 && x.CreateAt.Year == selectedYear)
                    .GroupBy(x => x.CreateAt.Year)
                    .Select(o => new
                    {
                        totalOrders = o.Count(),
                        totalSales = o.Sum(x => x.Amount - x.ShippingFee),
                        avgSalesPerOrders = o.Sum(x => x.Amount - x.ShippingFee) / o.Count(),
                        totalUnits =  _context.OrderItems
                            .Where(x => x.Order.CreateAt.Year == selectedYear && x.Order.Status == 3)
                            .Sum(x => x.Quantity)
                    }).FirstOrDefaultAsync();

            return Ok(new
            {
                years = years,
                orderOverview = orderOverview,
                revenues = result
            });
            //return Ok(totalOrders);
        }
    }
}
