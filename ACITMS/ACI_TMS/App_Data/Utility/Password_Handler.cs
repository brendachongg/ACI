using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;

namespace GeneralLayer
{
    public class Password_Handler
    {
        public string GeneratePassword(int length, bool lowercase, bool uppercase, bool numbers, bool symbols)
        {
            if (length <= 0 || (!lowercase && !uppercase && !numbers && !symbols))
            {
                return "### ERROR ###";
            }

            string availableCharacters = "";

            if (lowercase)
            {
                availableCharacters += "abcdefghijklmnopqrstuvwxyz";
            }

            if (uppercase)
            {
                availableCharacters += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }

            if (numbers)
            {
                availableCharacters += "0123456789";
            }

            if (symbols)
            {
                availableCharacters += "`~!@#$%^&*";
            }

            Random rnd = new Random();
            string password = "";
            for (int i = 0; i < length; i++)
            {
                int randNumber = rnd.Next(0, availableCharacters.Length - 1);
                password += availableCharacters[randNumber];
            }

            return password;
        }
    }
}