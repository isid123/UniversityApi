﻿namespace UniversityApi.Data
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int Credits { get; set; }
        public List<Exam>? Exams { get; set; }
    }
}
