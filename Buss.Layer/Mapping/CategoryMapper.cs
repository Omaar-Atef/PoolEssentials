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
    public class CategoryMapper : Profile
    {
        public CategoryMapper() 
        {
            CreateMap<Category,CategoryDTO>();
            CreateMap<CategoryDTO,Category>();
        }
    }
}
