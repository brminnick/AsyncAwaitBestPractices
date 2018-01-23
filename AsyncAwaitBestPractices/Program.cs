using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AsyncAwaitBestPractices
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            var goodAsyncObject = new GoodAsyncAwait();

            PrintPersonList(await goodAsyncObject.GetPersonModels());
        }

        static void PrintPersonList(List<PersonModel> personList)
        {
            Console.WriteLine($"{personList.Count} Entries Found");
            Console.WriteLine();

            foreach (var person in personList)
                Console.WriteLine(person);

            Console.WriteLine();
        }
    }
}
