﻿namespace WebApiDia2.Models
{
    public class LoginModel

    {

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ClientType { get; set; } // "web", "mobile", "api", etc.

    }


}
