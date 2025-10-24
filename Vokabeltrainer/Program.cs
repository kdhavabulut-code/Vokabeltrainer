using System;                          // Grundlegende Klassen und Datentypen (z. B. Console, String, DateTime)
using System.Collections.Generic;      // Für generische Sammlungen wie List<T>, Dictionary<K,V> usw.
using System.Data.SQLite;              // Für die Arbeit mit SQLite-Datenbanken
using System.Diagnostics.Metrics;      // Zum Erfassen und Überwachen von Leistungsmetriken
using System.IO;                       // Für Datei- und Verzeichnisoperationen
using System.Reflection;               // Zum Zugriff auf Metadaten und Informationen über Typen/Assemblys
using System.Runtime.Intrinsics.X86;   // Für CPU-nahe Optimierungen und SIMD-Befehle (leistungsintensive Berechnungen)

namespace VokabelTrainer
{
    class Program
    {
        // Wörterbuch für Benutzername + Passwort
        static Dictionary<string, string> users = new Dictionary<string, string>()
        {
            { "admin", "1234" },
            { "user", "pass" }
        };

        static string userFile = "users.txt";
        // Name der Textdatei, in der Benutzerdaten gespeichert werden

        static string dbFile = Path.Combine(Environment.CurrentDirectory, "vokabeltrainer.db");
        // Pfad zur SQLite-Datenbankdatei im aktuellen Arbeitsverzeichnis

        static string connectionString = $"Data Source={dbFile};Version=3;";
        // Verbindungszeichenfolge für die SQLite-Datenbank

        static void Main(string[] args)
        {
            // Setzt den Titel des Konsolenfensters
            Console.Title = "Vokabeltrainer";

            // Begrüßungstext für den Benutzer
            Console.WriteLine("=== Willkommen beim Vokabeltrainer ===\n");

            // Lädt die vorhandenen Benutzerdaten
            LoadUsers();

            // Initialisiert die Datenbank (z. B. erstellt Tabellen oder lädt Vokabeln)
            InitDatabase();

            // Führt den Login-Prozess durch
            if (Login())
            {
                // Wenn der Login erfolgreich ist, wird das Hauptmenü angezeigt
                ShowMenu();
            }
            else
            {
                // Wenn zu viele falsche Versuche gemacht wurden, wird das Programm beendet
                Console.WriteLine("Zu viele Fehlversuche. Programm wird beendet.");
            }
        }


        // -------------------------
        // Benutzer laden / speichern
        // -------------------------
        static void LoadUsers()
        {
            // Überprüfen, ob die Benutzerdatei existiert
            if (!File.Exists(userFile))
            {
                // Wenn nicht, erstelle eine neue Benutzerdatei
                SaveUsers();
                // Methode beenden
                return;
            }

            // Alle Zeilen der Datei lesen
            string[] lines = File.ReadAllLines(userFile);

            // Jede Zeile durchgehen
            foreach (var line in lines)
            {
                // Zeile anhand des Zeichens ';' aufteilen
                var parts = line.Split(';');

                // Prüfen, ob die Zeile genau zwei Teile hat (Benutzername und Passwort)
                if (parts.Length == 2)
                    // Benutzername und Passwort im Dictionary 'users' speichern
                    users[parts[0]] = parts[1];
            }
        }


        static void SaveUsers()
        {
            // Neue Liste erstellen, die die Zeilen für die Datei enthält
            List<string> lines = new List<string>();

            // Jede Benutzereintragung im Dictionary durchgehen
            foreach (var user in users)
                // Benutzername und Passwort mit ';' trennen und zur Liste hinzufügen
                lines.Add($"{user.Key};{user.Value}");

            // Alle Zeilen in die Datei 'userFile' schreiben
            File.WriteAllLines(userFile, lines);
        }


        // -------------------------
        // Login
        // -------------------------
        static bool Login()
        {
            // Maximale Anzahl an Versuchen festlegen
            int attempts = 3;

            // Solange noch Versuche übrig sind, wiederholen
            while (attempts > 0)
            {
                // Benutzer nach dem Benutzernamen fragen
                Console.Write("Benutzername: ");
                string username = Console.ReadLine();

                // Benutzer nach dem Passwort fragen
                Console.Write("Passwort: ");
                string password = Console.ReadLine();

                // Überprüfen, ob der Benutzer existiert und das Passwort korrekt ist
                if (users.ContainsKey(username) && users[username] == password)
                {
                    // Erfolgreicher Login
                    Console.WriteLine("\nLogin erfolgreich!\n");
                    return true; // Methode beendet und Erfolg zurückgeben
                }
                else
                {
                    // Falsche Eingabe: Versuche reduzieren
                    attempts--;
                    Console.WriteLine($"Falsche Eingabe. Verbleibende Versuche: {attempts}\n");
                }
            }

            // Wenn alle Versuche aufgebraucht sind: Login fehlgeschlagen
            return false;
        }


        // -------------------------
        // Hauptmenü
        // -------------------------
        static void ShowMenu()
        {
            // Variable, um das Hauptmenü aktiv zu halten
            bool running = true;

            // Solange das Programm läuft, Menü wiederholen
            while (running)
            {
                // Hauptmenü anzeigen
                Console.WriteLine("=== Hauptmenü ===");
                Console.WriteLine("1. Neuen Benutzer registrieren");
                Console.WriteLine("2. Voreingestellte Vokabeln hinzufügen"); // Neue Option
                Console.WriteLine("3. Neue Vokabel hinzufügen");
                Console.WriteLine("4. Vokabeln anzeigen");
                Console.WriteLine("5. Vokabel bearbeiten");
                Console.WriteLine("6. Vokabel löschen");
                Console.WriteLine("7. Quiz starten");
                Console.WriteLine("8. Beenden");
                Console.Write("Wähle eine Option: ");

                // Benutzereingabe lesen
                string choice = Console.ReadLine();

                // Entscheidung basierend auf der Benutzereingabe
                switch (choice)
                {
                    case "1":
                        // Einen neuen Benutzer registrieren
                        RegisterNewUser();
                        break;

                    case "2":
                        // Voreingestellte (Standard-)Vokabeln hinzufügen
                        FillDefaultVokabeln();
                        break;

                    case "3":
                        // Eine neue Vokabel manuell hinzufügen
                        AddVokabel();
                        break;

                    case "4":
                        // Alle gespeicherten Vokabeln anzeigen
                        ShowVokabeln();
                        break;

                    case "5":
                        // Eine vorhandene Vokabel bearbeiten
                        EditVokabel();
                        break;

                    case "6":
                        // Eine Vokabel löschen
                        DeleteVokabel();
                        break;

                    case "7":
                        // Ein Quiz mit den gespeicherten Vokabeln starten
                        StartQuiz();
                        break;

                    case "8":
                        // Programm beenden
                        running = false;
                        Console.WriteLine("Programm beendet. Tschüss!");
                        break;

                    default:
                        // Ungültige Eingabe
                        Console.WriteLine("Ungültige Auswahl!\n");
                        break;
                }
            }
        }


        // -------------------------
        // Neue Vokabel hinzufügen
        // -------------------------
        static void AddVokabel()
        {
            // Variable, um zu bestimmen, ob der Benutzer weitere Vokabeln hinzufügen möchte
            bool weiter = true;

            // Schleife läuft, solange der Benutzer „j“ (ja) eingibt
            while (weiter)
            {
                // Benutzer nach der deutschen Bedeutung fragen
                Console.Write("Deutsch: ");
                string deutsch = Console.ReadLine();

                // Benutzer nach der türkischen Bedeutung fragen
                Console.Write("Türkisch: ");
                string turkisch = Console.ReadLine();

                // Überprüfen, ob die Eingabe leer ist
                if (string.IsNullOrWhiteSpace(deutsch) || string.IsNullOrWhiteSpace(turkisch))
                {
                    Console.WriteLine("Eingabe darf nicht leer sein!\n");
                }
                else
                {
                    // Verbindung zur SQLite-Datenbank herstellen
                    using (var conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();

                        // SQL-Befehl zum Einfügen einer neuen Vokabel vorbereiten
                        string sql = "INSERT INTO Vokabeln (Deutsch, Turkisch) VALUES (@d, @t)";

                        // SQL-Befehl mit Parametern ausführen (sicher gegen SQL-Injection)
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@d", deutsch);
                            cmd.Parameters.AddWithValue("@t", turkisch);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Erfolgsnachricht anzeigen
                    Console.WriteLine("Vokabel erfolgreich in die Datenbank hinzugefügt!\n");
                }

                // Benutzer fragen, ob er noch eine Vokabel hinzufügen möchte
                Console.Write("Noch eine Vokabel hinzufügen? (j/n): ");
                string antwort = Console.ReadLine();

                // Wenn der Benutzer nicht „j“ eingibt, Schleife beenden
                if (string.IsNullOrWhiteSpace(antwort) || antwort.ToLower() != "j")
                    weiter = false;
            }
        }


        // -------------------------
        // Vokabeln anzeigen
        // -------------------------
        static void ShowVokabeln()
        {
            // Öffnet eine Verbindung zur SQLite-Datenbank
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open(); // Verbindung aktivieren

                // SQL-Befehl: alle Vokabeln nach ihrer ID sortiert auswählen
                string sql = "SELECT Id, Deutsch, Turkisch FROM Vokabeln ORDER BY Id";

                // SQL-Befehl vorbereiten und ausführen
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n=== Vokabelliste (aus der Datenbank) ===");

                    bool any = false; // Prüfen, ob Einträge vorhanden sind

                    // Liest jede Zeile aus der Datenbank und zeigt sie an
                    while (reader.Read())
                    {
                        any = true;
                        Console.WriteLine($"{reader["Id"]}. Deutsch: {reader["Deutsch"]} - Türkisch: {reader["Turkisch"]}");
                    }

                    // Wenn keine Einträge gefunden wurden, Meldung anzeigen
                    if (!any)
                    {
                        Console.WriteLine("Keine Vokabeln vorhanden.\n");
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                } // Der Reader und Command werden automatisch geschlossen
            } // Die Verbindung wird automatisch geschlossen
        }


        // -------------------------
        // Vokabel bearbeiten
        // -------------------------
        static void EditVokabel()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string listSql = "SELECT Id, Deutsch, Turkisch FROM Vokabeln ORDER BY Id";
                using (var listCmd = new SQLiteCommand(listSql, conn))
                using (var reader = listCmd.ExecuteReader())
                {
                    Console.WriteLine("\n=== Vokabelliste (zum Bearbeiten) ===");
                    bool any = false;
                    while (reader.Read())
                    {
                        any = true;
                        Console.WriteLine($"{reader["Id"]}. Deutsch: {reader["Deutsch"]} - Türkisch: {reader["Turkisch"]}");
                    }
                    if (!any)
                    {
                        Console.WriteLine("Keine Vokabeln zum Bearbeiten.\n");
                        return;
                    }
                    Console.WriteLine();
                }

                Console.Write("Nummer (Id) der zu bearbeitenden Vokabel: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Ungültige Eingabe!\n");
                    return;
                }

                Console.Write("Neue Deutsch Vokabel: ");
                string newDeutsch = Console.ReadLine();
                Console.Write("Neue Türkisch Vokabel: ");
                string newTurkisch = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newDeutsch) || string.IsNullOrWhiteSpace(newTurkisch))
                {
                    Console.WriteLine("Eingabe darf nicht leer sein!\n");
                    return;
                }

                string updateSql = "UPDATE Vokabeln SET Deutsch = @d, Turkisch = @t WHERE Id = @id";
                using (var updateCmd = new SQLiteCommand(updateSql, conn))
                {
                    updateCmd.Parameters.AddWithValue("@d", newDeutsch);
                    updateCmd.Parameters.AddWithValue("@t", newTurkisch);
                    updateCmd.Parameters.AddWithValue("@id", id);

                    int affected = updateCmd.ExecuteNonQuery();
                    if (affected > 0)
                        Console.WriteLine("Vokabel erfolgreich bearbeitet!\n");
                    else
                        Console.WriteLine("Keine Vokabel mit dieser Id gefunden.\n");
                }
            }
        }

        // -------------------------
        // Vokabel löschen
        // -------------------------
        static void DeleteVokabel()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string listSql = "SELECT Id, Deutsch, Turkisch FROM Vokabeln ORDER BY Id";
                using (var listCmd = new SQLiteCommand(listSql, conn))
                using (var reader = listCmd.ExecuteReader())
                {
                    Console.WriteLine("\n=== Vokabelliste (zum Löschen) ===");
                    bool any = false;
                    while (reader.Read())
                    {
                        any = true;
                        Console.WriteLine($"{reader["Id"]}. Deutsch: {reader["Deutsch"]} - Türkisch: {reader["Turkisch"]}");
                    }
                    if (!any)
                    {
                        Console.WriteLine("Keine Vokabeln zum Löschen.\n");
                        return;
                    }
                    Console.WriteLine();
                }

                Console.Write("Nummer (Id) der zu löschenden Vokabel: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Ungültige Eingabe!\n");
                    return;
                }

                string deleteSql = "DELETE FROM Vokabeln WHERE Id = @id";
                using (var deleteCmd = new SQLiteCommand(deleteSql, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@id", id);
                    int affected = deleteCmd.ExecuteNonQuery();
                    if (affected > 0)
                        Console.WriteLine("Vokabel erfolgreich gelöscht!\n");
                    else
                        Console.WriteLine("Keine Vokabel mit dieser Id gefunden.\n");
                }
            }
        }

        // -------------------------
        // Datenbank erstellen
        // -------------------------
        static void InitDatabase()
        {
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Vokabeln (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Deutsch TEXT NOT NULL,
                                Turkisch TEXT NOT NULL
                               );";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // -------------------------
        // Quizmodus
        // -------------------------
        static void StartQuiz()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, Deutsch, Turkisch FROM Vokabeln ORDER BY RANDOM()";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    List<(string Deutsch, string Turkisch)> vokabeln = new List<(string, string)>();

                    while (reader.Read())
                    {
                        vokabeln.Add((reader["Deutsch"].ToString(), reader["Turkisch"].ToString()));
                    }

                    if (vokabeln.Count == 0)
                    {
                        Console.WriteLine("Keine Vokabeln für das Quiz vorhanden.\n");
                        return;
                    }

                    Console.WriteLine("Wählen Sie die Quizrichtung:");
                    Console.WriteLine("1. Deutsch → Türkisch");
                    Console.WriteLine("2. Türkisch → Deutsch");
                    Console.Write("Ihre Wahl (1/2): ");
                    string wahl = Console.ReadLine();
                    bool deutschZuTurkisch = wahl == "1";

                    int punkte = 0;

                    foreach (var v in vokabeln)
                    {
                        string frage, richtigeAntwort;

                        if (deutschZuTurkisch)
                        {
                            frage = v.Deutsch;
                            richtigeAntwort = v.Turkisch;
                            Console.Write($"\nÜbersetze ins Türkische: {frage} -> ");
                        }
                        else
                        {
                            frage = v.Turkisch;
                            richtigeAntwort = v.Deutsch;
                            Console.Write($"\nÜbersetze ins Deutsche: {frage} -> ");
                        }

                        string antwort = Console.ReadLine();

                        if (antwort.Trim().ToLower() == richtigeAntwort.ToLower())
                        {
                            Console.WriteLine("Richtig!");
                            punkte++;
                        }
                        else
                        {
                            Console.WriteLine($"Falsch! Richtige Antwort: {richtigeAntwort}");
                        }

                        Console.Write("Nächste Frage? (j/n): ");
                        string weiter = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(weiter) || weiter.ToLower() != "j")
                            break;
                    }

                    Console.WriteLine($"\nQuiz beendet! Du hast {punkte} von {vokabeln.Count} Fragen richtig beantwortet.\n");
                }
            }
        }

        // -------------------------
        // Neuer Benutzer registrieren
        // -------------------------
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

        // -------------------------
        // Neue Methode: Voreingestellte Vokabeln hinzufügen
        // -------------------------
        static void FillDefaultVokabeln()
        {
            // Almanca ve Türkçe kelimeler: basitten zora
            var vokabeln = new List<(string Deutsch, string Turkisch)>
            {
("Haus", "Ev"),
("Baum", "Ağaç"),
("Wasser", "Su"),
("Katze", "Kedi"),
("Hund", "Köpek"),
("Auto", "Araba"),
("Straße", "Sokak"),
("Buch", "Kitap"),
("Apfel", "Elma"),
("Mensch", "İnsan"),
("Zeit", "Zaman"),
("Tag", "Gün"),
("Nacht", "Gece"),
("Kind", "Çocuk"),
("Freund", "Arkadaş"),
("Lehrer", "Öğretmen"),
("Schule", "Okul"),
("Tisch", "Masa"),
("Stuhl", "Sandalye"),
("Fenster", "Pencere"),
("Tür", "Kapı"),
("Lampe", "Lamba"),
("Zimmer", "Oda"),
("Hausaufgabe", "Ev ödevi"),
("Garten", "Bahçe"),
("Stadt", "Şehir"),
("Dorf", "Köy"),
("Land", "Ülke"),
("Fluss", "Nehir"),
("Berg", "Dağ"),
("Meer", "Deniz"),
("See", "Göl"),
("Himmel", "Gökyüzü"),
("Sonne", "Güneş"),
("Mond", "Ay"),
("Stern", "Yıldız"),
("Wolke", "Bulut"),
("Regen", "Yağmur"),
("Schnee", "Kar"),
("Wind", "Rüzgar"),
("Feuer", "Ateş"),
("Erde", "Toprak"),
("Luft", "Hava"),
("Hand", "El"),
("Fuß", "Ayak"),
("Kopf", "Baş"),
("Auge", "Göz"),
("Ohr", "Kulak"),
("Mund", "Ağız"),
("Nase", "Burun"),
("Haar", "Saç"),
("Herz", "Kalp"),
("Blume", "Çiçek"),
("Brot", "Ekmek"),
("Milch", "Süt"),
("Käse", "Peynir"),
("Fisch", "Balık"),
("Fleisch", "Et"),
("Reis", "Pirinç"),
("Salz", "Tuz"),
("Zucker", "Şeker"),
("Ei", "Yumurta"),
("Kaffee", "Kahve"),
("Tee", "Çay"),
("Saft", "Meyve suyu"),
("Brücke", "Köprü"),
("Zug", "Tren"),
("Bus", "Otobüs"),
("Flugzeug", "Uçak"),
("Schiff", "Gemi"),
("Fahrrad", "Bisiklet"),
("Uhr", "Saat"),
("Woche", "Hafta"),
("Monat", "Ay"),
("Jahr", "Yıl"),
("Sekunde", "Saniye"),
("Minute", "Dakika"),
("Stunde", "Saat"),
("Arbeit", "İş"),
("Beruf", "Meslek"),
("Firma", "Şirket"),
("Chef", "Patron"),
("Kollege", "İş arkadaşı"),
("Computer", "Bilgisayar"),
("Telefon", "Telefon"),
("Internet", "İnternet"),
("E-Mail", "E-posta"),
("Adresse", "Adres"),
("Name", "İsim"),
("Familie", "Aile"),
("Vater", "Baba"),
("Mutter", "Anne"),
("Bruder", "Erkek kardeş"),
("Schwester", "Kız kardeş"),
("Eltern", "Ebeveynler"),
("Bäckerei", "Fırın"),
("Supermarkt", "Süpermarket"),
("Krankenhaus", "Hastane"),
("Apotheke", "Eczane"),
("Polizei", "Polis"),
("Post", "Postane"),
("Park", "Park"),
("Markt", "Pazar"),
("Restaurant", "Restoran"),
("Kino", "Sinema")

                // İstenirse 100 veya daha fazla kelime eklenebilir
            };

           using (var conn = new SQLiteConnection("Data Source=vokabeltrainer.db"))
            {
                conn.Open();

                foreach (var vokabel in vokabeln)
                {
                    string Deutsch = vokabel.Item1;
                    string Turkisch = vokabel.Item2;

                    // Aynı kelime zaten var mı kontrol et
                    string checkSql = "SELECT COUNT(*) FROM Vokabeln WHERE Deutsch = @d";
                    using (var checkCmd = new SQLiteCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@d", Deutsch);
                        long count = (long)checkCmd.ExecuteScalar();
                        if (count > 0)
                            continue; // Varsa atla
                    }

                    // Yoksa ekle
                    string sql = "INSERT INTO Vokabeln (Deutsch, Turkisch) VALUES (@d, @t)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@d", Deutsch);
                        cmd.Parameters.AddWithValue("@t", Turkisch);
                        cmd.ExecuteNonQuery();
                    }
                }
            }



            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                foreach (var v in vokabeln)
                {
                    // Prüfen, ob die Vokabel schon existiert
                    string sqlCheck = "SELECT COUNT(*) FROM Vokabeln WHERE Deutsch = @d";
                    using (var cmdCheck = new SQLiteCommand(sqlCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@d", v.Deutsch);
                        long count = (long)cmdCheck.ExecuteScalar();
                        if (count == 0)
                        {
                            string sqlInsert = "INSERT INTO Vokabeln (Deutsch, Turkisch) VALUES (@d, @t)";
                            using (var cmdInsert = new SQLiteCommand(sqlInsert, conn))
                            {
                                cmdInsert.Parameters.AddWithValue("@d", v.Deutsch);
                                cmdInsert.Parameters.AddWithValue("@t", v.Turkisch);
                                cmdInsert.ExecuteNonQuery();
                            }
                        }
                    }
                }

                Console.WriteLine("Voreingestellte Vokabeln wurden erfolgreich hinzugefügt!\n");
            }
        }
    }
}
    








