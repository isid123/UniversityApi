﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApi.Data
{
    public class Subject
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public required string Title { get; set; }
        public int Credits { get; set; }
        public List<Exam>? Exams { get; set; }

        [ForeignKey(nameof(CourseId))]
        Course? Course { get; set; }
    }
}
