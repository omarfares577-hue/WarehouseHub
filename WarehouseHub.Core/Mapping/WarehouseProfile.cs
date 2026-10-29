using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WarehouseHub.Core.Dtos;
using WarehouseHub.Domain.Entities;
namespace WarehouseHub.Core.Mapping
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile() {
            CreateMap<WarehouseItem, WarehouseItemDto>()
            .ForMember(item => item.CategoryName, opt => opt.MapFrom(item => item.Category.Name));

            CreateMap<InventoryLog, InventoryLogDto>()
                .ForMember(item => item.WarehouseItemName, opt => opt.MapFrom(item => item.WarehouseItem.Name))
                .ForMember(item => item.SupplierName, opt => opt.MapFrom(item => item.Supplier.Name))
                .ForMember(item => item.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("hh:mm:ss tt dd-MM-yyyy", CultureInfo.InvariantCulture)));
            CreateMap <CreateLogsDto, InventoryLog>()
              .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<CreateCategoryDto,Category>();
            CreateMap<CreateSupplierDto,Supplier>();
            CreateMap<Supplier, SupplierDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<ArchivedInventoryLog, InventoryLogDto>();
        }
    }
}
