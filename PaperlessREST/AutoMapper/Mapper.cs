using AutoMapper;
using PaperlessREST.DomainModel;
using PaperlessREST.DTOModels;
using PaperlessREST.Models;

namespace PaperlessREST.AutoMapper;

public class DocumentProfile : Profile
{
    public DocumentProfile()
    {
        // Mapping von DomainModel zu DTO
        CreateMap<Document, DocumentDto>();

        // Mapping von Upload-DTO zu DomainModel
        CreateMap<DocumentUploadDto, Document>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Title)) // Titel zu Name
            .ForMember(d => d.DateUploaded, o => o.MapFrom(_ => DateTime.UtcNow)) // Hochladedatum auf jetzt setzen
            .ForMember(d => d.FilePath, o => o.Ignore()); // FilePath ignorieren, da es später gesetzt wird

        // Mapping zwischen der Entität aus der Datenbank und der DomainModel-Klasse
        CreateMap<PostgreSQL.Entities.Document, Document>()
            .ForMember(d => d.FilePath, o => o.MapFrom(e => e.FilePath)); // Sicherstellen, dass FilePath korrekt übernommen wird

        CreateMap<Document, PostgreSQL.Entities.Document>()
            .ForMember(e => e.FilePath, o => o.MapFrom(d => d.FilePath)); // Und zurück des Mappings
    }
}
