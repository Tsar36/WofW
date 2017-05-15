using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models.Mappers;
using WorldOfWords.API.Models.IMappers;

namespace WorldOfWords.API.Models
{
    public class CourseMapper : ICourseMapper
    {
        private IWordSuiteMapper _mapper;
        private ILanguageMapper _languageMapper;

        public CourseMapper(IWordSuiteMapper mapper, ILanguageMapper languageMapper)
        {
            _mapper = mapper;
            _languageMapper = languageMapper;
        }
        public CourseModel Map(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException("course");
            }
            CourseModel courseModel = new CourseModel();
            courseModel.Id = course.Id;
            courseModel.Name = course.Name;
            courseModel.Language = _languageMapper.ToApiModel(course.Language);
            courseModel.WordSuites = _mapper.Map(course.WordSuites.ToList());           
            return courseModel;
        }
        public Course Map(CourseEditModel course)
        {
            return new Course()
            {
                Id = course.Id,
                Name = course.Name,
                LanguageId = course.LanguageId,
                OwnerId = course.OwnerId
            };
        }

        public List<CourseModel> Map(List<Course> courses)
        {
            return courses.Select(x=> Map(x)).ToList();
        }
    }
}
