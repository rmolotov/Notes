using AutoMapper;
using Notes.Application.Common.Mappings;
using Notes.Application.Interfaces;
using Notes.Persistence.Database;

namespace Notes.Tests.Common;

public class QueryTestFixture : IDisposable
{
    public NotesDbContext Context;
    public IMapper Mapper;

    public QueryTestFixture()
    {
        Context = NotesContextFactory.Create();
        var configProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AssemblyMappingProfile(typeof(INotesDbContext).Assembly));
        });
        Mapper = configProvider.CreateMapper();
    }

    public void Dispose() =>
        NotesContextFactory.Destroy(Context);
}