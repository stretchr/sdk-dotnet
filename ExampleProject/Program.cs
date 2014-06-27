using System;
using System.Collections.Generic;
using Stretchr;

/*
 * This example project uses a live sandboxed account.
 * It's possible that other people playing with the same sandbox
 * will produce unexpected results.
 * 
 * For a sensible experience, create your own account and projects.
 * 
*/

namespace ExampleProject
{
    internal class Person : Resource
    {
        public string Name
        {
            get { return Get("name"); }
            set { Set("name", value); }
        }

        public long Age
        {
            get { return Get("age"); }
            set { Set("age", value); }
        }

        public string JobTitle
        {
            get { return Get("position"); } 
            set { Set("position", value); }
        }
    }

    internal class Program
    {
        // TODO: Replace this with your Stretchr Account name
        private const string StretchrAccount = "play";

        // TODO: Replace this with your Stretchr project name
        private const string StretchrProject = "sandbox";

        // TODO: Repalce this with your Stretchr API key
        private const string StretchrApikey = "7f138d973113b335a8ac260e41f30ce0";

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
            // Delete all people
            //
            ChangesResponse response = Stretchr.At("people").Delete();
            if (response.IsSuccess)
            {
                Console.WriteLine(" -- Deleted all People for a fresh start...");
            }
            else
            {
                Console.WriteLine(" -- Didn't delete /people because " + response.ErrorMessage);
            }

            //
            // Create a new person
            //
            var mat = new Person
            {
                Name = "Mat",
                Age = 30
            };

            ChangesResponse r = Stretchr.At("people").Create(mat);
            if (r.IsSuccess)
            {
                Console.WriteLine("Mat was created and has ID: " + mat.Id);
            }
            else
            {
                Console.WriteLine("Failed to create mat: " + r.ErrorMessage);
            }

            //
            // Read mat by his ID
            // (if this fails, an exception will be raised)
            //
            var readMat = Stretchr.At("people/" + mat.Id).MustReadOne<Person>();
            Console.WriteLine("Read Mat back, and I know his age is: " + readMat.Age);

            // 
            // Change Mat's name using update
            //
            mat.Name = "Mathew";
            r = Stretchr.At("people/" + mat.Id).Update(mat);
            if (r.IsSuccess)
            {
                Console.WriteLine("Updated Mat's name to Mathew.");
            }
            else
            {
                Console.WriteLine("Failed to update mat: " + r.ErrorMessage);
            }

            //
            // Read Mat's name again
            //
            readMat = Stretchr.At("people/" + mat.Id).MustReadOne<Person>();
            if (r.IsSuccess)
            {
                Console.WriteLine("Mat's name is now: " + readMat.Name);
            }
            else
            {
                Console.WriteLine("Failed to read mat again: " + r.ErrorMessage);
            }

            //
            // Create a few more people in different ways
            //
            Console.WriteLine("Making more people...");
            Stretchr.At("people").Create(new Person { Name = "Ryan", JobTitle = "Product Developer"});
            Stretchr.At("people").Create(new Person { Name = "Tyler", JobTitle = "Platform Developer"});
            Stretchr.At("people").Create(new Person { Name = "Christian", JobTitle = "CEO"});

            //
            // Read all people
            //
            Console.WriteLine("Reading all people...");
            IList<Person> people = Stretchr.At("people").MustReadMany<Person>();
            foreach (Person person in people)
            {
                Console.WriteLine("There is a person called " + person.Name + " (with ID " + person.Id + ")");
            }

            Console.WriteLine("In a browser, see: " + Stretchr.At("people").Url());
            Console.WriteLine("Press return to exit.");
            Console.ReadLine();
        }

        private void CreateResourceUsingResourceClass()
        {
            var person = new Resource {{"name", "Mat"}, {"age", 30}};

            ChangesResponse response = Stretchr.At("people").Create(person);
            if (!response.IsSuccess)
            {
                throw new Exception("Failed to create resource: " + response.ErrorMessage);
            }
        }

        private void CreateResourceUsingDictionary()
        {
            ChangesResponse response = Stretchr.At("people").Create(new Dictionary<string, dynamic>
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