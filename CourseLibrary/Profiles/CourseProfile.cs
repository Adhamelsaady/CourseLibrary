using AutoMapper;
using CourseLibrary.Entiies;
using CourseLibrary.Models;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>();
        CreateMap<CourseForCreationDto, Course>();
        CreateMap<CourseForUpdateDto, Course>().ReverseMap();
    }
}