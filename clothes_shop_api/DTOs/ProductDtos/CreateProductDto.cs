﻿using clothes_shop_api.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace clothes_shop_api.DTOs.ProductDtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
     
        public int Price { get; set; }
        public string? Description { get; set; }
        public int Discount { get; set; }
       
        public string Slug { get; set; } = null!;
        
        public int CategoryId { get; set; }
      
        public int[] ProductColors { get; set; }

        public int[] ProductSizes { get; set; }
        public IFormFile MainImage { get; set; }
        public IFormFile SubImage { get; set; }
        public IFormFile[] ProductImages { get; set; }
    }
}
