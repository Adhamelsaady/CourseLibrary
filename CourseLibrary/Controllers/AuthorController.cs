using AutoMapper;
using CourseLibrary.Entiies;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/authors")]
public class AuthorController : ControllerBase
{
    private readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public AuthorController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _courseLibraryRepository = courseLibraryRepository;
    }

    [HttpGet]
    [HttpHead]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var result = await _courseLibraryRepository.GetAuthorsAsync();
        return Ok(_mapper.Map<IEnumerable<AuthorDto>>(result));
    }

    [HttpGet("{authorId}", Name = "GetAuthor")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid authorId)
    {
        var author = await _courseLibraryRepository.GetAuthorAsync(authorId);
        if(author == null)
            return NotFound();
        return Ok(_mapper.Map<AuthorDto>(author));
    }

    [HttpPost]  
    public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorDto author)
    {
        var authorEntity = _mapper.Map<Author>(author);
        _courseLibraryRepository.AddAuthor(authorEntity);
        var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
        return CreatedAtRoute("GetAuthour"
            , new {authorId = authorToReturn.AuthorId} 
            , authorToReturn
            );
    }
        
}