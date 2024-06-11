﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.DTOs.Request
{
    public class EmailSendDto
    {
        public EmailSendDto(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
       
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
