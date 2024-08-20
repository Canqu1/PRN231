using AutoMapper;
using BackEnd.DTO;
using BackEnd.Models;

namespace BackEnd
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Ánh xạ từ UpdateStudentDto sang Student
            CreateMap<StudentUpdateDto, Student>()
                .ForMember(dest => dest.StudentDetails, opt => opt.Ignore()); // Vì StudentDetails cần xử lý riêng

            // Ánh xạ từ UpdateStudentDto sang StudentDetail 
            CreateMap<StudentUpdateDto, StudentDetail>()
                .ForMember(dest => dest.Student, opt => opt.Ignore()); // Sinh viên được ánh xạ từ Student

        }
    }
}
