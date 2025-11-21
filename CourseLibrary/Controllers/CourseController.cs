using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CourseLibrary.Entiies;
using CourseLibrary.Models;
using Microsoft.AspNetCore.JsonPatch;

[ApiController]
[Route("api/authors/{authorId}/Courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public CourseController(ICourseLibraryRepository  courseLibraryRepository
        , IMapper mapper)
    {
        _courseLibraryRepository = courseLibraryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses(Guid authorId)
    {
        if (! await _courseLibraryRepository.AuthorExistsAsync(authorId))
            return NotFound();
        
        var courses = await _courseLibraryRepository.GetCoursesAsync(authorId);
        return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
    }

    [HttpGet("{courseId}" , Name = "GetCourse")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid authorId , Guid courseId)
    {
        if (! await _courseLibraryRepository.AuthorExistsAsync(authorId))
            return NotFound();

        var courseForTheAuthor = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);
        if (courseForTheAuthor == null)
            return NotFound();
        return Ok(_mapper.Map<CourseDto>(courseForTheAuthor));
    }
    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(Guid authorId, CourseForCreationDto courseDto)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
            return NotFound();
        var courseEntity = _mapper.Map<Course>(courseDto);
        _courseLibraryRepository.AddCourse(authorId , courseEntity);
        await _courseLibraryRepository.SaveAsync();
        var courseToReturn = _mapper.Map<CourseDto>(courseEntity);
        return CreatedAtRoute("GetCourse" 
            ,new {authorId, courseId = courseEntity.CourseId}
            ,courseToReturn);
    }

    [HttpPut("{courseId}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(
          Guid authorId
        , Guid courseId
        , CourseForUpdateDto courseDto)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
            return NotFound();
       
        var courseEntity = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

        if (courseEntity == null)
        {
            var courseToAdd = _mapper.Map<Course>(courseDto);
            courseToAdd.CourseId =  courseId;
            _courseLibraryRepository.AddCourse(authorId ,  courseToAdd);
            await _courseLibraryRepository.SaveAsync();
            
            var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
            return CreatedAtRoute("GetCourse",
                new { authorId, courseId },
                courseToReturn);
        }

        _mapper.Map(courseDto, courseEntity);
        _courseLibraryRepository.UpdateCourse(courseEntity);
        await _courseLibraryRepository.SaveAsync();

        return NoContent();
    }

    [HttpPatch("{courseId}")]
    public async Task<IActionResult> PartiallyUpdateCourse(
        Guid authorId,
        Guid courseId,
        JsonPatchDocument<CourseForUpdateDto> patchDocument)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
            return NotFound();
       
        var courseEntity = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

        if (courseEntity == null)
        {
            var courseDto = new CourseForUpdateDto();
            patchDocument.ApplyTo(courseDto);
           
            var courseToAdd = _mapper.Map<Course>(courseDto);
            courseToAdd.CourseId = courseId;
            _courseLibraryRepository.AddCourse(authorId , courseToAdd);
            _courseLibraryRepository.SaveAsync();
            
            var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
            return CreatedAtRoute("GetCourse",
                new { authorId, courseId },
                courseToReturn);
            
        }
        
        var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseEntity);
        patchDocument.ApplyTo(courseToPatch);
        _mapper.Map(courseToPatch, courseEntity);
        _courseLibraryRepository.UpdateCourse(courseEntity);
        await _courseLibraryRepository.SaveAsync();

        return NoContent();
    }
    

    [HttpDelete("{courseId}")]
    public async Task<ActionResult> DeleteCourse(Guid authorId, Guid courseId)
    {
        if (!await _courseLibraryRepository.AuthorExistsAsync(authorId))
            return NotFound();
       
        var courseEntity = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);
        if (courseEntity == null)
            return NotFound();
        _courseLibraryRepository.DeleteCourse(courseEntity);
        await _courseLibraryRepository.SaveAsync();
        return NoContent();
    }

    [HttpOptions]
    public IActionResult GetOptions()
    {
        Response.Headers.Add("Allow", "OPTIONS,POST,GET,HEAD");
        return Ok();    
    }
}
