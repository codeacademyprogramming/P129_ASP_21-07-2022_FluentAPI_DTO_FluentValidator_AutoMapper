using AutoMapper;
using P129FirstApi.Data.Entities;
using P129FirstApi.DTOs.CatagoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P129FirstApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CategoryPostDto, Category>()
                .ForMember(des => des.Name, src => src.MapFrom(x => x.Ad.Trim()))
                .ForMember(des=>des.Image,src=>src.MapFrom(x=>x.AidOlduguUstCategoryId == null ? x.Sekli : "1.jpg"))
                .ForMember(des=>des.IsMain,src=>src.MapFrom(x=>x.Esasdirmi))
                .ForMember(des=>des.ParentId,src=>src.MapFrom(x=>x.AidOlduguUstCategoryId))
                .ForMember(des=>des.CreatedAt,src=>src.MapFrom(x=>DateTime.UtcNow.AddHours(4)));

            CreateMap<Category, CategoryGetDto>()
                .ForMember(des => des.Ad, src => src.MapFrom(x => x.Name))
                .ForMember(des => des.Sekil, src => src.MapFrom(x => x.Image))
                .ForMember(des => des.Esasdirmi, src => src.MapFrom(x => x.IsMain))
                .ForMember(des => des.AidOlduguUstCategoryId, src => src.MapFrom(x => x.ParentId));

            CreateMap<Category, CategoryListDto>()
                .ForMember(des => des.Ad, src => src.MapFrom(x => x.Name));
        }
    }
}
