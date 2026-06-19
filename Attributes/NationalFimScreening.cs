using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public interface IStringEncryptor
{
    string EncryptString(string plainText);
    string DecryptString(string encryptedText);
}

public class TripleDESStringEncryptor : IStringEncryptor
{
    private byte[] _key;
    private byte[] _iv;
    private static readonly Encoding LegacyEncoding = Encoding.GetEncoding(1252);

    public TripleDESStringEncryptor()
    {
        _key = System.Text.ASCIIEncoding.ASCII.GetBytes("GSYAHAGCBDUUADIADKOPAAAW");
        _iv = System.Text.ASCIIEncoding.ASCII.GetBytes("USAZBGAW");
    }

    #region IStringEncryptor Members
    public string EncryptString(string plainText)
    {
        using var provider = new TripleDESCryptoServiceProvider();
        using var encryptor = provider.CreateEncryptor(_key, _iv);
        return Transform(plainText, encryptor);
    }

    public string DecryptString(string encryptedText)
    {
        using var provider = new TripleDESCryptoServiceProvider();
        using var decryptor = provider.CreateDecryptor(_key, _iv);
        return Transform(encryptedText, decryptor);
    }
    #endregion

    private string Transform(string text, ICryptoTransform transform)
    {
        if (text == null)
        {
            return null;
        }
        using (MemoryStream stream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                byte[] input = LegacyEncoding.GetBytes(text);
                cryptoStream.Write(input, 0, input.Length);
                cryptoStream.FlushFinalBlock();
                               
                return LegacyEncoding.GetString(stream.ToArray());
            }
        }
    }
}