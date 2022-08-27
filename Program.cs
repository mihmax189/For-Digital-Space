using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Models;

using System.Linq;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            const int size = 10000;

            Console.WriteLine("Generate persons...");
            Generator.Generator generator = new Generator.Generator();
            List<Person> persons = generator.Generate(size);

            Console.WriteLine("Write to file...");
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = Path.Combine(docPath, "Person.json");

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                //WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            using (StreamWriter stream = new StreamWriter(fileName))
            {
                foreach (Person person in persons)
                {
                    stream.WriteLine(JsonSerializer.Serialize(person, options));
                }
            }

            persons = new List<Person>();

            if (File.Exists(fileName))
            {
                using (StreamReader stream = new StreamReader(fileName))
                {
                    string person;
                    do
                    {
                        person = stream.ReadLine();
                        if (person == null)
                            break;

                        persons.Add(JsonSerializer.Deserialize<Person>(person, options));
                    }
                    while (true);
                }
            }

            calcAndShowInfo(persons);
        }

        private static int childAge(Int64 sec)
        {
            DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(sec);

            int years = DateTime.Now.Year - (offset.DateTime.Year);

            return (DateTime.Now.Month > offset.DateTime.Month || (DateTime.Now.Month == offset.DateTime.Month 
                && DateTime.Now.Day >= offset.DateTime.Day)) ? years : years - 1;
        }

        private static void calcAndShowInfo(List<Person> list)
        {
            int countPeople = 0, countCards = 0, countChildren = 0;
            double averageAgeChild = 0.0;

            foreach (Person person in list)
            {
                countPeople += (person.Children.Length + 1);    // parent + count children
                countCards += (person.CreditCardNumbers.Length);

                foreach (Child child in person.Children)
                    averageAgeChild += childAge(child.BirthDate);

                countChildren += person.Children.Length;
            }

            Console.WriteLine("******** INFORMATION ********");
            Console.WriteLine("Count of people: {0}", countPeople);
            Console.WriteLine("Count of cards: {0}", countCards);
            Console.WriteLine("Average age of children: {0}", averageAgeChild / countChildren);
        }
    }
}
