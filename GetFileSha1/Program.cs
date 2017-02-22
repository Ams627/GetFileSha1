using System;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;
using Nito.AsyncEx;
using System.Net.Http;

namespace GetFileSha1
{
    internal class Program
    {
        private static string Sha1(string filename)
        {
            string result;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(fs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                    result = formatted.ToString();
                }
            }
            return result;
        }

        static async void MainAsync(string[] args)
        {
            var url = "https://www.w3.org/TR/grddl-primer/hl7-sample.xml";
            var uri = new Uri(url);
            var filename = "q:/temp/s.xml";

            using (var client = new HttpClient())
            using (var s = await client.GetStreamAsync(uri))
            using (var o = File.OpenWrite(filename))
            {
                await s.CopyToAsync(o);
            }
            Console.WriteLine($"Sha1: {Sha1(filename)}");
        }

        private static void Main(string[] args)
        {
            try
            {
                AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }
        }
    }
}
