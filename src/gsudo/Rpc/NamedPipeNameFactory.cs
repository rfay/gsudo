﻿using gsudo.Helpers;
using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;

namespace gsudo.Rpc
{
    static class NamedPipeNameFactory
    {
        public static string GetPipeName(string allowedSid, int allowedPid)
        {
            if (allowedPid < 0) allowedPid = 0;

            var data = $"{allowedSid}_{allowedPid}{(InputArguments.TrustedInstaller ? "_TI" : string.Empty)}";
#if !DEBUG
            data = GetHash(data);
#endif
            return $"{GetPipePrefix()}_{data}";
        }

        private static string GetHash(string data)
        {
            using (var hashingAlg = System.Security.Cryptography.SHA256.Create())
            {
                var hash = hashingAlg.ComputeHash(UTF8Encoding.UTF8.GetBytes(data));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2", CultureInfo.InvariantCulture));
                }
                return sb.ToString();
            }
        }

        private static string GetPipePrefix()
        {
            if (!string.IsNullOrEmpty(InputArguments.User))
            {
                if (InputArguments.User == WindowsIdentity.GetCurrent().Name)
                    if (!ProcessHelper.IsMemberOfLocalAdmins())
                        return "gsudo";
                    else
                        return "ProtectedPrefix\\Administrators\\gsudo";

                return "ProtectedPrefix\\Administrators\\gsudo";
            }
            else
                return "ProtectedPrefix\\Administrators\\gsudo";
        }    
    }
}
