using AutoMapper;
using IMS.Models;
using IMS.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IMS
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product 
            CreateMap<Product,ProductDTO>();
            CreateMap<ProductDTO,Product>();
            
            // Category
            CreateMap<Category,CategoryDTO>();
            CreateMap<CategoryDTO,Category>();

            CreateMap<InventoryTransaction,InventoryTransactionDTO>();
            CreateMap<InventoryTransactionDTO,InventoryTransaction>();
        }
    }
}
