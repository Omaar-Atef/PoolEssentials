using AutoMapper;
using Buss.Layer.DTOs;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buss.Layer.Mapping
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())  // Ignore if creating new
                .ForMember(dest => dest.Category, opt => opt.Ignore()); // Ignore if creating new
        }
    }
}
