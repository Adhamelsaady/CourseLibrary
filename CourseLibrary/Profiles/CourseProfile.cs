using AutoMapper;
using CourseLibrary.Entiies;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>();
        CreateMap<CourseForCreationDto, Course>();
    }
}