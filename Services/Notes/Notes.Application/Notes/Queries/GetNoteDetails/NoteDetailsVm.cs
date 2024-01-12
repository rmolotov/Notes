using AutoMapper;
using Notes.Application.Common.Mappings;
using Notes.Core.Entities;

namespace Notes.Application.Notes.Queries.GetNoteDetails;

public class NoteDetailsVm : IMapWith<Note>
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    public string Details { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? EditedDate { get; set; }

    public void Mapping(Profile profile) =>
        profile
            .CreateMap<Note, NoteDetailsVm>()
            .ForMember(
                vm => vm.Id,
                options => options.MapFrom(note => note.Id))
            .ForMember(
                vm => vm.Title,
                options => options.MapFrom(note => note.Title))
            .ForMember(
                vm => vm.Details,
                options => options.MapFrom(note => note.Details))
            .ForMember(
                vm => vm.CreatedDate,
                options => options.MapFrom(note => note.CreatedDate))
            .ForMember(
                vm => vm.EditedDate,
                options => options.MapFrom(note => note.EditedDate));
}