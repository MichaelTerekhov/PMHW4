using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;

namespace Task_3
{
    class Program
    {
        private static List<Note> cacheNote;
        static void Main(string[] args)
        {
            Hello();
            Manager();
        }

        private static void Manager()
        {
            Console.WriteLine("Your personal assistant welcomes you.\n" +
                "What operation do you want to carry out");
           
                while (true)
                {

                Operations();
                Console.Write("Select -> ");
                    bool input = int.TryParse(Console.ReadLine(), out int parsed);
                    if (!input)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Unsupported command! Try again");
                        Console.ResetColor();
                    continue;
                    }
                    else
                    {
                        if (parsed == 1)
                        {
                            SearchNotes();
                            continue;
                        }
                        else if (parsed == 2)
                        {
                            ViewANote();
                            continue;
                        }
                        else if (parsed == 3)
                        {
                        CreateANote();
                            continue;
                        }
                        else if (parsed == 4)
                        {
                             DeleteNote();   
                            continue;
                        }
                        if (parsed == 5)
                        {
                            break;
                        }
                    }
                }
            
        }

        private static void DeleteNote()
        {
            var jsonCache = File.ReadAllText("notes.json");
            cacheNote = JsonConvert.DeserializeObject<List<Note>>(jsonCache);

            Console.WriteLine("\nPlease enter id of note, which you would like to DELETE.");
            int parsed;
            while (true)
            {
                Console.Write("ID-> ");
                bool input = int.TryParse(Console.ReadLine(), out parsed);
                if (!input)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Wrong input param");
                    Console.ResetColor();
                    continue;
                }
                break;
            }
            bool noteFound = false;
            foreach (var m in cacheNote)
            {
                if (m.Id == parsed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    noteFound = true;
                    Console.WriteLine("\n" + m.ToString() + "\n");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You seriously want to permanently DELETE this note. ");
                    Console.ResetColor();
                    string solution;
                    while (true)
                    {
                        Console.WriteLine("Please write yes / no or y / n");
                        solution = Console.ReadLine().Replace(" ", "").ToLower();
                        if (String.IsNullOrEmpty(solution))
                        {
                            Console.WriteLine("Unknown operation!");
                            continue;
                        }
                        if (solution == "yes" || solution == "no" || solution == "y" || solution == "n")
                            break;
                        else 
                        {
                            Console.WriteLine("Unknown operation!");
                            continue;
                        }
                    }
                    if (solution == "yes" || solution == "y")
                    {
                        cacheNote.Remove(m);
                        var json = JsonConvert.SerializeObject(cacheNote, Formatting.Indented);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The note has been deleted");
                        File.WriteAllText("notes.json", json);
                        Console.ResetColor();
                    }
                    return;
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            if (!noteFound)
                Console.WriteLine($"\nSorry, but there is no such note with [{parsed} id] in the database :(\n");
            Console.ResetColor();
        }

        private static void CreateANote()
        {
            Console.WriteLine();

            Console.WriteLine("Let's try to create a note. Just enter the body of your note (main text)\n");
            var jsonCache = File.ReadAllText("notes.json");
            cacheNote = JsonConvert.DeserializeObject<List<Note>>(jsonCache);
            while (true)
            {
                Console.Write("Body of your note-> ");
                var body = Console.ReadLine().TrimStart().TrimEnd();
                if (String.IsNullOrEmpty(body))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You cant create note with empty body!!!");                   
                    Console.ResetColor();
                    return;
                }
                var maxId = 0;
                foreach (var m in cacheNote)
                    if (m.Id >= maxId)
                        maxId = m.Id;
                string title="";
                for (var i = 0; i < 32; i++)
                {
                    if (body.Length <= 32)
                    { 
                        title = body;
                        break;
                    }
                    title += body[i];
                }
                var createdNote = new Note{ CreatedOn = DateTime.UtcNow, Id = maxId + 1, Title = title, Text = body };
                cacheNote.Add(createdNote);
                var json = JsonConvert.SerializeObject(cacheNote, Formatting.Indented);
                File.WriteAllText("notes.json",json);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Creation date: {createdNote.CreatedOn}]\n" +
                $"[Id: {createdNote.Id}] Title: {createdNote.Title}\n" +
                $"Note body:\n" +
                $"{createdNote.Text}");
                Console.ResetColor();
                break;
            }

        }

        private static void SearchNotes()
        {
            Console.WriteLine();

            var jsonCache = File.ReadAllText("notes.json");
            cacheNote = JsonConvert.DeserializeObject<List<Note>>(jsonCache);
            Console.WriteLine("\nPlease enter id of note, which you would like to see.");
            string search;
            while (true)
            {
                Console.Write("Searching Parametr-> ");
                search = Console.ReadLine();
                if (String.IsNullOrEmpty(search))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    cacheNote = cacheNote.OrderBy(u => u.Id).ToList();
                    Console.WriteLine("Searching results:\n");
                    foreach (var m in cacheNote)
                        Console.WriteLine($"[Creation date: {m.CreatedOn}]\n" +
                         $"[Id: {m.Id}] Title: {m.Title}\n");
                    Console.ResetColor();
                    return;
                }
                break;
            }
            Console.WriteLine("\nSearch results:\n");

            var searchingResults = cacheNote.Where(
                u => u.CreatedOn.ToString(CultureInfo.InvariantCulture).Contains(search) 
            || u.Id.ToString().Contains(search) || u.Title.Contains(search) 
            || u.Text.Contains(search)).ToList();

            if (searchingResults.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Sorry, but there is no such note that includes a substring [{search}]\n");
                Console.ResetColor();
                return;
            }
            cacheNote = cacheNote.OrderBy(u => u.Id).ToList();
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var m in searchingResults)
                Console.WriteLine($"[Creation date: {m.CreatedOn}]\n" +
                           $"[Id: {m.Id}] Title: {m.Title}\n");
            Console.ResetColor();
        }

        private static void ViewANote()
        {
            Console.WriteLine();

            var jsonCache = File.ReadAllText("notes.json");
            cacheNote = JsonConvert.DeserializeObject<List<Note>>(jsonCache);

            Console.WriteLine("\nPlease enter id of note, which you would like to see.");
            int parsed;
            while (true)
            {
                Console.Write("Searching ID-> ");
                bool input = int.TryParse(Console.ReadLine(), out parsed);
                if (!input)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Wrong input param");
                    Console.ResetColor();
                    continue;
                }
                break;
            }
            bool noteFound = false;
            foreach (var m in cacheNote)
            {
                if (m.Id == parsed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    noteFound = true;
                    Console.WriteLine("\n"+m.ToString()+"\n");
                    Console.ResetColor();
                    return;
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            if (!noteFound)
                Console.WriteLine($"\nSorry, but there is no such note with [{parsed} id] in the database :(\n");
            Console.ResetColor();
        }

        private static void Hello()
        {
            Console.WriteLine("3. Welcome to note manager.You are given the opportunity to search, browse, create, delete notes.\n" +
                "Each time a new note you add is saved to the file\n" +
                "so that you can view it in the future.\n" +
                "(c)Michael Terekhov\n\n");
        }

        private static void Operations()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("1.\tSearch Notes\n" +
                "2.\tViewing a note\n" +
                "3.\tMake a note\n" +
                "4.\tDelete note\n" +
                "5.\tExit\n\n" +
                "Just write item number of list\t");
            Console.ResetColor();
        }

    }
}
