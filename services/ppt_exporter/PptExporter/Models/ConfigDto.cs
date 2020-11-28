using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PptExporter.Models
{
    public class ConfigDto
    {
        public int AfterSlide { get; set; } = 0;
        public AnimationType Animation { get; set; } = AnimationType.Simple;
        public string PptId { get; set; }
    }

    public enum AnimationType
    {
        Simple
    }
}
