using System;
using System.Collections.Generic;
using System.IO;

namespace VokabelTrainer
{
    class Program
    {
        static Dictionary<string, string> users = new Dictionary<string, string>()
        {
            { "admin", "1234" },
            { "user", "pass" }
        };

        static string userFile = "users.txt";
        static string vokabelFile = Path.Combine(Environment.CurrentDirectory, "vokabeln.txt");

        static void Main(string[] args)
        {
            Console.Title = "Vokabeltrainer";
            Console.WriteLine("=== Willkommen beim Vokabeltrainer ===\n");

            LoadUsers();

            if (Login())
            {
                ShowMenu();
            }
            else
            {
                Console.WriteLine("Zu viele Fehlversuche. Programm wird beendet.");
            }
        }

        // Benutzer laden
        static void LoadUsers()
        {
            if (!File.Exists(userFile))
            {
                SaveUsers();
                return;
            }

            string[] lines = File.ReadAllLines(userFile);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 2)
                    users[parts[0]] = parts[1];
            }
        }

        // Benutzer speichern
        static void SaveUsers()
        {
            List<string> lines = new List<string>();
            foreach (var user in users)
                lines.Add($"{user.Key};{user.Value}");
            File.WriteAllLines(userFile, lines);
        }

        // Login
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

        // Hauptmenü
        static void ShowMenu()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("=== Hauptmenü ===");
                Console.WriteLine("1. Neuen Benutzer registrieren");
                Console.WriteLine("2. Neue Vokabel hinzufügen");
                Console.WriteLine("3. Vokabeln anzeigen");
                Console.WriteLine("4. Vokabel bearbeiten");
                Console.WriteLine("5. Vokabel löschen");
                Console.WriteLine("6. Quiz starten");
                Console.WriteLine("7. Beenden");
                Console.Write("Wähle eine Option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterNewUser();
                        break;
                    case "2":
                        AddVokabel();  // ✅ Create
                        break;
                    case "3":
                        ShowVokabeln(); // ✅ Read
                        break;
                    case "4":
                        EditVokabel(); // ✅ Update
                        break;
                    case "5":
                        DeleteVokabel(); // ✅ Delete
                        break;
                    case "6":
                        Console.WriteLine("Quiz wird bald gestartet...\n");
                        break;
                    case "7":
                        running = false;
                        Console.WriteLine("Programm beendet. Tschüss!");
                        break;
                    default:
                        Console.WriteLine("Ungültige Auswahl!\n");
                        break;
                }
            }
        }

        // ✅ Neue Vokabel hinzufügen
        static void AddVokabel()
        {
            Console.Write("Deutsch: ");
            string deutsch = Console.ReadLine();

            Console.Write("Türkisch: ");
            string turkisch = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(deutsch) || string.IsNullOrWhiteSpace(turkisch))
            {
                Console.WriteLine("Eingabe darf nicht leer sein!\n");
                return;
            }

            File.AppendAllText(vokabelFile, $"{deutsch};{turkisch}\n");
            Console.WriteLine("Vokabel erfolgreich hinzugefügt!\n");
        }

        // ✅ Vokabeln anzeigen
        static void ShowVokabeln()
        {
            if (!File.Exists(vokabelFile))
            {
                Console.WriteLine("Keine Vokabeln vorhanden.\n");
                return;
            }

            string[] lines = File.ReadAllLines(vokabelFile);
            if (lines.Length == 0)
            {
                Console.WriteLine("Keine Vokabeln vorhanden.\n");
                return;
            }

            Console.WriteLine("\n=== Vokabelliste ===");
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length == 2)
                    Console.WriteLine($"{i + 1}. Deutsch: {parts[0]} - Türkisch: {parts[1]}");
            }
            Console.WriteLine();
        }

        // ✅ Vokabel bearbeiten
        static void EditVokabel()
        {
            if (!File.Exists(vokabelFile))
            {
                Console.WriteLine("Keine Vokabeln vorhanden.\n");
                return;
            }

            var lines = File.ReadAllLines(vokabelFile);
            if (lines.Length == 0)
            {
                Console.WriteLine("Keine Vokabeln zum Bearbeiten.\n");
                return;
            }

            ShowVokabeln();
            Console.Write("Nummer der zu bearbeitenden Vokabel: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > lines.Length)
            {
                Console.WriteLine("Ungültige Auswahl!\n");
                return;
            }

            Console.Write("Neue Deutsch Vokabel: ");
            string newDeutsch = Console.ReadLine();
            Console.Write("Neue Türkisch Vokabel: ");
            string newTurkisch = Console.ReadLine();

            lines[index - 1] = $"{newDeutsch};{newTurkisch}";
            File.WriteAllLines(vokabelFile, lines);
            Console.WriteLine("Vokabel erfolgreich bearbeitet!\n");
        }

        // ✅ Vokabel löschen
        static void DeleteVokabel()
        {
            if (!File.Exists(vokabelFile))
            {
                Console.WriteLine("Keine Vokabeln vorhanden.\n");
                return;
            }

            var lines = File.ReadAllLines(vokabelFile);
            if (lines.Length == 0)
            {
                Console.WriteLine("Keine Vokabeln zum Löschen.\n");
                return;
            }

            ShowVokabeln();
            Console.Write("Nummer der zu löschenden Vokabel: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > lines.Length)
            {
                Console.WriteLine("Ungültige Auswahl!\n");
                return;
            }

            var list = new List<string>(lines);
            list.RemoveAt(index - 1);
            File.WriteAllLines(vokabelFile, list);
            Console.WriteLine("Vokabel erfolgreich gelöscht!\n");
        }

        // Benutzer registrieren
        static void RegisterNewUser()
        {
            Console.Write("Neuer Benutzername: ");
            string newUser = Console.ReadLine();

            if (users.ContainsKey(newUser))
            {
                Console.WriteLine("Benutzername existiert bereits!\n");
                return;
            }

            Console.Write("Neues Passwort: ");
            string newPass = Console.ReadLine();

            users[newUser] = newPass;
            SaveUsers();
            Console.WriteLine("Benutzer erfolgreich registriert!\n");
        }
    }
}
