using System;

namespace Server
{
    public static class Constants
    {
        public const String Audience = "https://localhost:44355/"; //intended audience;current port of where we're serving the debug project
        public const String Issuer = Audience; //issuer is issuing token by itself
        public const String Secret = "This is a super secret key";     
    }
}
