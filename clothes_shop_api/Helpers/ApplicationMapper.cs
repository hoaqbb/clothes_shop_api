﻿using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs;
using clothes_shop_api.DTOs.CartItemDtos;
using clothes_shop_api.DTOs.CategoryDtos;
using clothes_shop_api.DTOs.ColorDtos;
using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.DTOs.ProductImageDtos;
using clothes_shop_api.DTOs.QuantityDtos;
using clothes_shop_api.DTOs.SizeDtos;
using clothes_shop_api.DTOs.UserDtos;
using CartDto = clothes_shop_api.DTOs.CartDto;

namespace clothes_shop_api.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper() 
        {
            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.MainPhoto, opt => opt.MapFrom(src =>
                    src.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl))
                .ForMember(dest => dest.SubPhoto, opt => opt.MapFrom(src =>
                    src.ProductImages.FirstOrDefault(x => x.IsSub).ImageUrl))
                .ForMember(dest => dest.ProductColors, opt => opt.MapFrom(src =>
                    src.ProductColors.Select(a => a.Color)))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => 
                    src.Category.Name));
            CreateMap<Product, ProductDetailDto>()
                .ForMember(dest => dest.ProductVariants, opt => opt.MapFrom(src =>
                    src.Quantities));
            CreateMap<CreateProductDto, Product>();

            CreateMap<RegisterDto, User>();
            CreateMap<User, UserDetailDto>();

            CreateMap<Color, ColorDto>();
            CreateMap<Color, ColorListDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => 
                    src.ProductColors.Select(p => p.ProductId).Distinct().Count()));
                

            CreateMap<Size, SizeDto>().ReverseMap();

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Products.Count));

            CreateMap<ProductImage, ProductImageDto>().ReverseMap();

            CreateMap<Quantity, QuantityDto>()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => 
                    src.ProductColor.Color.Name))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(src =>
                    src.ProductColor.Color.ColorCode))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src =>
                    src.Size.Name));

            CreateMap<Quantity, CartItemDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.Product.Name))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src =>
                    src.Product.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    src.Product.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src =>
                    src.Product.Discount))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src =>
                    src.Product.Slug))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    src.Product.Category.Name))
                .ForMember(dest => dest.ProductVariant, opt => opt.MapFrom(src => src));
            

            CreateMap<Cart, CartDto>();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Name))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Discount))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Slug))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Category.Name))
                .ForMember(dest => dest.ProductVariant, opt => opt.MapFrom(src =>
                    src.QuantityNavigation));

            CreateMap<Order, OrderListDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src =>
                    src.Payment.Method));

            CreateMap<Order, OrderDetailDto>()
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src =>
                    src.Payment.Status))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src =>
                    src.Payment.TransactionId))
                .ForMember(dest => dest.Provider, opt => opt.MapFrom(src =>
                    src.Payment.Provider));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Name))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.ProductColor.Color.Name))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Size.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src =>
                    src.QuantityNavigation.Product.Discount));
        }
    }
}
