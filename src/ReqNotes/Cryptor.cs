using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ReqNotes;

public static class Cryptor
{
    public static string CryptKey = string.Empty;

    public static string HashMD5(string data)
    {
        using MD5 md5 = MD5.Create();
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(data);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes);
    }

    public static bool CheckMD5(string data, string verifyData)
    {
        string hash = HashMD5(data);
        if (hash == verifyData)
            return true;
        return false;
    }

    public static string Crypt(string data)
    {
        string iv = "";
        for (int i = 0; i < 16; i++)
            iv += Cryptor.CryptKey[i % Cryptor.CryptKey.Length];
        string key = HashMD5(iv);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
        byte[] encDataBytes = EncryptStringToBytes_Aes(data, keyBytes, ivBytes);
        string encDataBase64 = Convert.ToBase64String(encDataBytes);
        return encDataBase64;
    }

    public static string Decrypt(string dataCrypted)
    {
        string iv = "";
        for (int i = 0; i < 16; i++)
            iv += Cryptor.CryptKey[i % Cryptor.CryptKey.Length];
        string key = HashMD5(iv);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
        byte[] cryDataBytes = Convert.FromBase64String(dataCrypted);
        string data = DecryptStringFromBytes_Aes(cryDataBytes, keyBytes, ivBytes);
        return data;
    }

    private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        byte[] encrypted;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
        }
        return encrypted;
    }

    private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        string plaintext = string.Empty;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msDecrypt = new(cipherText);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            plaintext = srDecrypt.ReadToEnd();
        }
        return plaintext;
    }
}