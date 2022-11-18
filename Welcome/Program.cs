namespace Welcome;
class Program
{
    static void Main(string[] args)
    {
         string[] lines = System.IO.File.ReadAllLines(@"welcometext.txt");

        foreach (string line in lines)
        {
            Console.WriteLine("\t" + line);
        }
       

        
    }
}
