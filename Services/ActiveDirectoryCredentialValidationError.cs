﻿using System;
using System.Text.RegularExpressions;

namespace MultiFactor.SelfService.Windows.Portal.Services
{
    /// <summary>
    /// Active Directory credential validation
    /// </summary>
    public class ActiveDirectoryCredentialValidationResult
    {
        public bool IsValid { get; private set; }
        public bool UserMustChangePassword { get; private set; }
        public string Reason { get; private set; }


        public static ActiveDirectoryCredentialValidationResult Ok()
        {
            return new ActiveDirectoryCredentialValidationResult
            {
                IsValid = true,
            };
        }

        public static ActiveDirectoryCredentialValidationResult KnownError(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return UnknowError();
            }

            var pattern = @"data ([0-9a-e]{3})";
            var match = Regex.Match(errorMessage, pattern);

            if (match.Success && match.Groups.Count == 2)
            {
                var data = match.Groups[1].Value;

                switch (data)
                {
                    case "525":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "User not found" };
                    case "52e":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "Invalid credentials" };
                    case "530":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "Not permitted to logon at this time" };
                    case "531":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "Not permitted to logon at this workstation​" };
                    case "532":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "Password expired", UserMustChangePassword = true };
                    case "533":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "Account disabled" };
                    case "701":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "Account expired" };
                    case "773":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "User must change password", UserMustChangePassword = true };
                    case "775":
                        return new ActiveDirectoryCredentialValidationResult { Reason = "User account locked" };
                }
            }

            return UnknowError(errorMessage);
        }

        public static ActiveDirectoryCredentialValidationResult UnknowError(string errorMessage = null)
        {
            return new ActiveDirectoryCredentialValidationResult { Reason = errorMessage ?? "Unknown error"};
        }
    }
}