using AutoMapper;
using Contract.DTOModels;
using PaperlessService.Entities;
using PostgreSQL.Entities;

namespace Mapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            // Entity to Business Layer mapping
            CreateMap<Document, BlDocument>()
                .ForMember(dest => dest.File, opt => opt.Ignore()) // Explicitly ignore File property
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id != 0)); // Only map Id if it's not default

            // Upload DTO to Business Layer mapping
            CreateMap<DocumentUploadDto, BlDocument>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.DateUploaded, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.FilePath, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File));

            // Business Layer to DTO mapping
            CreateMap<BlDocument, DocumentDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForSourceMember(src => src.File, opt => opt.DoNotValidate());

            // Add missing mapping for DocumentDto -> BlDocument
            CreateMap<DocumentDto, BlDocument>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dest => dest.DateUploaded, opt => opt.MapFrom(src => src.DateUploaded))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.File, opt => opt.Ignore()); // Assuming you don't want to map the File property here
        }
    }
}