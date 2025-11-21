using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.Entiies;
using CourseLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.Controllers;

[ApiController]
[Route("api/authorcollections")]
public class AuthorCollectionsController : ControllerBase
{
    protected readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository,  IMapper mapper)
    {
        _courseLibraryRepository = courseLibraryRepository;
        _mapper = mapper;
    }


    [HttpGet("{authorids})", Name = "GetAuthorCollections")]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthorCollection(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))]
        [FromRoute] IEnumerable<Guid> authorIds)
    {
        var authorEntites = await _courseLibraryRepository.GetAuthorsAsync(authorIds);
        if(authorEntites.Count() != authorIds.Count())
            return NotFound();
        return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorEntites));
    }
    
    
    [HttpPost]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> CreateAuthorCollection(IEnumerable<AuthorForCreationDto> authors)
    {
        var authorEntites = _mapper.Map<IEnumerable<Author>>(authors);

        foreach (var author in authorEntites)
        {
            _courseLibraryRepository.AddAuthor(author);
        }
        await _courseLibraryRepository.SaveAsync();
        
        var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntites);
        var authorCollectionIdsAsString = string.Join(" , " ,  authorCollectionToReturn.Select(a => a.AuthorId));
        return CreatedAtRoute("GetAuthorCollections",
            new {authorIds = authorCollectionIdsAsString},
            authorCollectionToReturn
        );
    }
}