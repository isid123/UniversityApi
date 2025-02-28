using UniversityApi.Data;

namespace UniversityApi.DTO
{
    public class Mapper
    {
        public StudentDTO MapEntityToDto(Student student)
        {
            return new StudentDTO
            {
                Id = student.Id,
                Name = student.Name,
                Surname = student.Surname,
            };
        }
        public Student MapDtoToEntity(StudentDTO studentDTO)
        {
            return new Student
            {
                Id = studentDTO.Id,
                Name = studentDTO.Name,
                Surname = studentDTO.Surname,
            };
        }

        public SubjectDTO MapEntityToDto(Subject subject)
        {
            return new SubjectDTO
            {
                Id = subject.Id,
                Title = subject.Title,
                Credits = subject.Credits,
            };
        }
        public Subject MapDtoToEntity(SubjectDTO subjectDTO)
        {
            return new Subject
            {
                Id = subjectDTO.Id,
                Title = subjectDTO.Title,
                Credits = subjectDTO.Credits,
            };
        }
        public ExamDTO MapEntityToDto(Exam exam)
        {
            return new ExamDTO
            {
                StudentId = exam.StudentId,
                SubjectId = exam.SubjectId,
                Grade = exam.Grade
            };
        }
        public Exam MapDtoToEntity(ExamDTO examDTO)
        {
            return new Exam
            {
                StudentId = examDTO.StudentId,
                SubjectId = examDTO.SubjectId,
                Grade = examDTO.Grade
            };
        }

        public CourseDTO MapEntityToDto(Course course)
        {
            return new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                StartCourse = course.StartCourse,
            };
        }
        public Course MapDtoToEntity(CourseDTO courseDTO)
        {
            return new Course
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                StartCourse = courseDTO.StartCourse,
            };
        }
        public SubscriptionDTO MapEntityToDto(Subscription subscription)
        {
            return new SubscriptionDTO
            {
                CourseId = subscription.CourseId,
                StudentId = subscription.StudentId,
            };
        }
        public Subscription MapDtoToEntity(SubscriptionDTO subscriptionDTO)
        {
            return new Subscription
            {
                CourseId = subscriptionDTO.CourseId,
                StudentId = subscriptionDTO.StudentId,
            };
        }
    }
}
