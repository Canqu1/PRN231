﻿namespace BackEnd.DTO
{
    public class StudentReq
    {
        public string Name { get; set; }
        public int AccountID { get; set; }
        public int Age { get; set; }
        public bool IsRegularStudent { get; set; }

        public string Address { get; set; }

        public string AdditionalInformation { get; set; }
        public string PhoneNumber { get; set; }

    }
    public class StudentProfileDTO
    {
        public int StudentId { get; set; }
        public int Userid { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsRegularStudent { get; set; }
        public StudentDetailDTO Details { get; set; } // Thay đổi ở đây
    }

    public class StudentDetailDTO
    {
        public string Address { get; set; }
        public string AdditionalInformation { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class StudentDetailReqDTO
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsRegularStudent { get; set; }
        public string Address { get; set; }
        public string AdditionalInformation { get; set; }
        public List<EvaluationDTO> Evaluation { get; set; }
    }
    public class StudentReqSubjectDTO
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsRegularStudent { get; set; }
    }
}
