/*Hommy:Login*/
using Microsoft.AspNetCore.Mvc;
using static System.Formats.Asn1.AsnWriter;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using car_test.Functions;
using System.Security.Principal;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using car_test.Models;

namespace car_test.Models
{
    public class userData
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public userData(string _Id, string _Password)
        {
            Id = _Id;
            Password = _Password;
            Token = getToken();

        }
        public bool checkUser()
        {
            return Token == databaseProcess.getDBToken(Id);
        }

        private string getToken()
        {
            string _Token = (Password+"asd1234").ToMD5();
            return _Token;
        }

    }
}
