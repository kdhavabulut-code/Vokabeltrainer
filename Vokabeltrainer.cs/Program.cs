namespace Vokabeltrainer.cs
{
    using System;
    using System.Collections.Generic;

    namespace VokabelTrainer
    {
        class Program//ERSTE CLASS
        {
            static Dictionary<string, string> users = new Dictionary<string, string>()
        {
            { "admin", "1234" },
            { "user", "pass" }
        };

            static void Main(string[] args)
            {
                Console.Title = "Vokabeltrainer";
                Console.WriteLine("=== Willkommen beim Vokabeltrainer ===\n");

                if (Login())
                {
                    ShowMenu();
                }
                else
                {
                    Console.WriteLine("Zu viele Fehlversuche. Programm wird beendet.");
                }
            }

            static bool Login()
            {
                int attempts = 3;
                while (attempts > 0)
                {
                    Console.Write("Benutzername: ");
                    string username = Console.ReadLine();
                    Console.Write("Passwort: ");
                    string password = Console.ReadLine();

                    if (users.ContainsKey(username) && users[username] == password)
                    {
                        Console.WriteLine("\nLogin erfolgreich!\n");
                        return true;
                    }
                    else
                    {
                        attempts--;
                        Console.WriteLine($"Falsche Eingabe. Verbleibende Versuche: {attempts}\n");
                    }
                }
                return false;
            }

            static void ShowMenu()
            {
                bool running = true;
                while (running)
                {
                    Console.WriteLine("=== Hauptmenü ===");
                    Console.WriteLine("1. Neue Vokabel hinzufügen");
                    Console.WriteLine("2. Vokabeln anzeigen");
                    Console.WriteLine("3. Quiz starten");
                    Console.WriteLine("4. Beenden");
                    Console.Write("Wähle eine Option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Hier wird neue Vokabel hinzugefügt (bald)...");
                            break;
                        case "2":
                            Console.WriteLine("Hier werden Vokabeln angezeigt (bald)...");
                            break;
                        case "3":
                            Console.WriteLine("Quiz wird bald gestartet...");
                            break;
                        case "4":
                            running = false;
                            Console.WriteLine("Programm beendet. Tschüss!");
                            break;
                        default:
                            Console.WriteLine("Ungültige Auswahl!\n");
                            break;
                    }
                }
            }
        }
    }

}
