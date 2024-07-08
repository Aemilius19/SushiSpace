using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Mapper
{
    public class MapProfile:Profile
    {
        public MapProfile()
        {
            CreateMap<Product, Product>();
            CreateMap<ProductDTO, Product>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<CommentDTO,Comment>();
            CreateMap<CommentDTO, Comment>().ReverseMap();
            CreateMap<RegisterDTO, User>();
            CreateMap<UpdateDTO, User>();
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>().ReverseMap();


        }
    }
}
