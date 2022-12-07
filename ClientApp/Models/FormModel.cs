﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class FormModel
    {
        public string FileName { get; }
        public string FileExtension { get; }
        public byte [] FileBytes { get; }
        


        public FormModel()
        {

        }

        public FormModel(string fn,string fe, byte[] fb)
        {
            FileName = fn;
            FileExtension = fe;
            FileBytes = fb;
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
