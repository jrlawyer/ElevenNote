﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Models
{
    public class NoteEditModel
    {
        public int NoteId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsStarred { get; set; }

        //public object DateTimeOffset { get; set; }

        public override string ToString()
        {
            return $"[{NoteId}] {Title}";
        }
    }
}

