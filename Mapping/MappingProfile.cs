using AutoMapper;
using WebApiDia2.Entities;

namespace WebApiDia2.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Configuración de mapeo para Product
            CreateMap<Product, Product>();
            CreateMap<Product, Product>();

            // Puedes agregar más configuraciones de mapeo aquí
            // Ejemplo:
            // CreateMap<OtherEntity, OtherDTO>();
            // CreateMap<OtherDTO, OtherEntity>();
        }
    }
}
