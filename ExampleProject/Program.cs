using System;
using System.Collections.Generic;
using Stretchr;

namespace ExampleProject
{
    internal class Person : Resource
    {
        public string Name
        {
            get { return Get("name"); }
            set { Set("name", value); }
        }

        public int Age
        {
            get { return Get("age"); }
            set { Set("age", value); }
        }
    }

    internal class Program
    {
        private const string StretchrAccount = "TODO Replace this with your Stretchr Account name";
        private const string StretchrProject = "TODO Replace this with your Stretchr project name";
        private const string StretchrApikey = "TODO Repalce this with your Stretchr API key";

        private static Client Stretchr { get; set; }

        private static void Main(string[] args)
        {
            //
            // (required) Create the Stretchr client
            //
            Stretchr = new Client(
                StretchrAccount,
                StretchrProject,
                StretchrApikey,
                // In real applications, you don't need to worry about setting
                // an ITransport.  See GetTransport below for details.
                GetTransport()
                );

            //
            // Create a new person
            //
            var mat = new Person
            {
                Name = "Mat",
                Age = 30
            };

            var r = Stretchr.At("people").Create(mat);
            if (r.IsSuccess)
            {
                Console.Write("Mat was created and has ID: " + mat.Id);
            }
            else
            {
                Console.Write("Failed to create mat: " + r.ErrorMessage);
            }

            //
            // Read mat by his ID
            // (if this fails, an exception will be raised)
            //
            var readMat = Stretchr.At("people/" + mat.Id).MustReadOne<Person>();
            Console.Write("Read Mat back, and I know his age is: " + readMat.Age);

            // 
            // Change Mat's name using update
            //
            mat.Name = "Mathew";
            r = Stretchr.At("people/" + mat.Id).Update(mat);
            if (r.IsSuccess)
            {
                Console.Write("Updated Mat's name to Mathew.");
            }
            else
            {
                Console.Write("Failed to update mat: " + r.ErrorMessage);
            }

            //
            // Create a few more people in different ways
            //
            Console.WriteLine("Making more people...");
            Stretchr.At("people").Create(new Dictionary<string, dynamic> {{"Name", "Tyler"}});
            Stretchr.At("people").Create(new Dictionary<string, dynamic> {{"Name", "Ryan"}});
            Stretchr.At("people").Create(new Dictionary<string, dynamic> {{"Name", "Christian"}});

            //
            // Read all people
            //
            Console.WriteLine("Reading all people...");
            var people = Stretchr.At("people").MustReadMany<Person>();
            foreach (var person in people)
            {
                Console.WriteLine("There is a person called " + person.Name + " (with ID " + person.Id + ")");
            }
        }

        private void CreateResourceUsingResourceClass()
        {
            var person = new Resource {{"name", "Mat"}, {"age", 30}};

            var response = Stretchr.At("people").Create(person);
            if (!response.IsSuccess)
            {
                throw new Exception("Failed to create resource: " + response.ErrorMessage);
            }
        }

        private void CreateResourceUsingDictionary()
        {
            var response = Stretchr.At("people").Create(new Dictionary<string, dynamic>
            {
                {"name", "Mat"},
                {"age", 30}
            });
            if (!response.IsSuccess)
            {
                throw new Exception("Failed to create resource: " + response.ErrorMessage);
            }
        }

        /// <summary>
        ///     GetTransport gets a LoggingTransport that prints to Standard Output.
        ///     This is only necessary if you want to see what's going on - otherwise,
        ///     you can just let the Client worry about the transport.
        /// </summary>
        private static ITransport GetTransport()
        {
            return new LoggingTransport(Console.OpenStandardOutput(), new HttpTransport());
        }
    }
}