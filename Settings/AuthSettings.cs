﻿namespace scabackend.Settings
{
    public class AuthSettings
    {
        public string secret_key { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
        public int expiration { get; set; }
    }
}
