using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Notes.Application.Common.Mappings;
using Notes.Application.Notes.Commands.CreateNote;

namespace Notes.WebApi.Models;

public class CreateNoteDto : IMapWith<CreateNoteCommand>
{
    [Required]
    public string Title { get; set; }
    public string Details { get; set; }
    
    public void Mapping(Profile profile) =>
        profile
            .CreateMap<CreateNoteDto, CreateNoteCommand>()
            .ForMember(
                command => command.Title,
                options => options.MapFrom(dto => dto.Title))
            .ForMember(
                command => command.Details,
                options => options.MapFrom(dto => dto.Details));
}