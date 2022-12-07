using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcClient
{
    internal class Photo
    {
        public string PhotoName { get; set; }   
        public string PhotoExtension { get; set; }  
        public byte[] PhotoBytes { get; set; }     


        public Photo(string name, string extension, byte[] bytes)
        {
            PhotoName = name;
            PhotoExtension = extension; 
            PhotoBytes = bytes; 
        }

    }
}
