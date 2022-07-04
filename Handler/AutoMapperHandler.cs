using AutoMapper;
using ProductAPIVS.Entity;
using ProductAPIVS.Models;

namespace ProductAPIVS.Handler;
public class AutoMapperHandler:Profile
{
   public AutoMapperHandler(){
    CreateMap<Product,ProductEntity>().ForMember(item=>item.ProductName,opt=>opt.MapFrom(item=>item.Name))
    .ForMember(item=>item.Price,opt=>opt.MapFrom(item=>Convert.ToDecimal(item.Price))).ReverseMap();

   }
}