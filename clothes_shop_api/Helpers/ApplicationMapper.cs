using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs;
using clothes_shop_api.DTOs.ColorDtos;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.DTOs.ProductImageDtos;
using clothes_shop_api.DTOs.QuantityDtos;
using clothes_shop_api.DTOs.SizeDtos;

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

            CreateMap<Color, ColorDto>().ReverseMap();

            CreateMap<Size, SizeDto>().ReverseMap();

            CreateMap<ProductImage, ProductImageDto>().ReverseMap();

            CreateMap<Quantity, QuantityDto>()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => 
                    src.ProductColor.Color.Name))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(src =>
                    src.ProductColor.Color.ColorCode))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src =>
                    src.Size.Name));

            CreateMap<Quantity, QuantitiesTestDto>()
                .ForMember(src => src.Size, opt => opt.MapFrom(src => 
                    src.Size.Name));

            CreateMap<Quantity, TestDto>()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src =>
                    src.ProductColor.Color.Name))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(src =>
                    src.ProductColor.Color.ColorCode));
                //.ForMember(dest => dest.ProductVariants, opt => opt.MapFrom(
                //    src => src)); 
        }
    }
}
