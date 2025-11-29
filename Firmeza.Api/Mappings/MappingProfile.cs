using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Web.Models.Entities;

namespace Firmeza.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();

        // User/Client mappings
        CreateMap<User, ClientDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        // Sale mappings
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null));

        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
    }
}
