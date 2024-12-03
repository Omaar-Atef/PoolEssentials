using AutoMapper;
using Buss.Layer.DTOs;
using DataLayer.Entities;

namespace Buss.Layer.Mapping
{
    public class BrandMapper : Profile
    {
        public BrandMapper()
        {
            CreateMap<Brand, BrandDTO>();
            CreateMap<BrandDTO, Brand>();

        }
    }
}
