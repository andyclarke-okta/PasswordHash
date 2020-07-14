using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using PasswordHash.Models;
using CryptHash.Core.Hash;

namespace PasswordHash
{
    public class CreateHashedPassword
    {
        private readonly ILogger<CreateHashedPassword> _logger;

        public CreateHashedPassword(ILogger<CreateHashedPassword> logger)
        {
            _logger = logger;
        }

        public void DoAction(string name)
        {

            string password = null;

            Console.Write("Enter a password: ");
            password = Console.ReadLine();


            Console.WriteLine("ClearText Psw " + password);


            BCRYPT_HashPassword("BCRYPT", password, 10);

            HashWithSalt("SHA1",password, 10, System.Security.Cryptography.SHA1.Create());
            HashWithSalt("SHA256", password, 10, System.Security.Cryptography.SHA256.Create());
            HashWithSalt("SHA512", password, 10, System.Security.Cryptography.SHA512.Create());
            HashWithSalt("MD5", password, 10, System.Security.Cryptography.MD5.Create());
        }



        public Password HashWithSalt(string textAlgo, string password, int saltLength, System.Security.Cryptography.HashAlgorithm hashAlgo)
        {
            byte[] saltBytes = new byte[saltLength];
            //byte[] hashedBytes;

            Password output = new Password();
            output.CleartextPsw = password;
            try
            {
                using (var keyGenerator = RandomNumberGenerator.Create())
                {
                    keyGenerator.GetBytes(saltBytes);
                }

                byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
                List<byte> passwordWithSaltBytes = new List<byte>();
                passwordWithSaltBytes.AddRange(passwordAsBytes);
                passwordWithSaltBytes.AddRange(saltBytes);
                byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());

                output.HashedPassword = Convert.ToBase64String(digestBytes);
                output.Salt = Convert.ToBase64String(saltBytes);

                _logger.LogDebug("cleartext " + output.CleartextPsw + textAlgo + " hashed " + output.HashedPassword + textAlgo + " salt postfix " + output.Salt);

                Console.WriteLine(" ");
                Console.WriteLine(textAlgo +" hashed " + output.HashedPassword);
                Console.WriteLine(textAlgo + " salt postfix " + output.Salt);
            }
            catch (Exception ex)
            {

            }
            return output;
        }




        public Password BCRYPT_HashPassword(string textAlgo, string password, int workfactor)
        {
            Password output = new Password();
            try
            {
                output.CleartextPsw = password;
                //var encryptedPassword = BcryptHashPassword(record.CleartextPsw);

                var salt = BCrypt.Net.BCrypt.GenerateSalt(workfactor);   //10 is the workfactor
                var encryptedPassword = new Password
                {
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt),
                    Salt = salt
                };
                output.HashedPassword = encryptedPassword.HashedPassword.Replace(encryptedPassword.Salt, "");
                output.Salt = encryptedPassword.Salt.Replace("$2a$10$", "");

                _logger.LogDebug("cleartext " + output.CleartextPsw + textAlgo + " hashed " + output.HashedPassword + textAlgo +" salt " + output.Salt);

                Console.WriteLine(" ");
                Console.WriteLine(textAlgo + " workfactor " + workfactor.ToString());
                Console.WriteLine(textAlgo +" hashed " + output.HashedPassword);
                Console.WriteLine(textAlgo +" salt " + output.Salt);
            }
            catch (Exception ex)
            {

            }
            return output;
        }
    }

    //public class HashWithSaltResult
    //{
    //    public string Salt { get; }
    //    public string Digest { get; set; }

    //    public HashWithSaltResult(string salt, string digest)
    //    {
    //        Salt = salt;
    //        Digest = digest;
    //    }
    //}



}
