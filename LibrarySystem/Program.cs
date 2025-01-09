using LibrarySystem;
using System.ComponentModel.Design;
using System.Security.Cryptography;

var context = new AppDbContext();

var userService = new UserService(context);
var bookService = new BookService(context);

while (true)
{
    Console.WriteLine("\nBibliotek");
    Console.WriteLine("\n1. Registrera nytt konto");
    Console.WriteLine("2. Logga in");
    Console.WriteLine("3. Ta bort användare");
    Console.WriteLine("4. Avsluta program");
    Console.Write("\nVälj ett alternativ: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            RegisterUser(userService);
            break;

        case "2":
            LoginUser(userService);
            break;

        case "3":
            DeleteUser(userService);
            break;

        case "4":
            Console.WriteLine("\nAvslutar program...");
            return;

        default:
            Console.WriteLine("\nOgiltigt alternativ, försök igen.");
            break;
    }

    void RegisterUser(UserService userService)
    {
        Console.Clear();
        Console.WriteLine("\nKontoregistrering");

        while (true)
        {
            Console.Write("\nAnvändarnamn: ");
            var username = Console.ReadLine();
            Console.Write("Lösenord: ");
            var password = Console.ReadLine();

            bool isAdmin = false;
            while (true)
            {
                Console.Write("\nÄr du administratör (Ja/Nej): ");
                string isAdminInput = Console.ReadLine().ToLower();

                if (isAdminInput == "ja")
                {
                    isAdmin = true;
                    break;
                }
                else if (isAdminInput == "nej")
                {
                    isAdmin = false;
                    break;
                }
                else
                {
                    Console.WriteLine("\nOgiltig inmatning. Ange Ja eller Nej.");
                }
            }

            bool success = userService.RegisterUser(username, password, isAdmin);
            if (success)
            {
                Console.WriteLine("\nAnvändare registrerad.");
                Console.WriteLine("\nTryck på valfri knapp för att återgå till huvudmeny...");
                Console.ReadKey();
                Console.Clear();
                break;
            }
            else
            {
                Console.WriteLine("\nRegistreringen misslyckades. Försök igen.");
            }
        }
    }

    void LoginUser(UserService userService)
    {
        Console.Clear();
        Console.WriteLine("\nInloggning");

        Console.Write("\nAnvändarnamn: ");
        var username = Console.ReadLine();
        Console.Write("Lösenord: ");
        var password = Console.ReadLine();

        var user = userService.AuthenticateUser(username, password);

        if (user != null)
        {
            Console.WriteLine("\nInloggning lyckades.");
            Console.WriteLine("\nTryck på valfri knapp för att gå till meny...");
            Console.ReadKey();
            Console.Clear();
            ShowMenu(user); // Skickar vidare användaren till rätt meny
        }
        else
        {
            Console.WriteLine("\nInloggning misslyckades. Försök igen.");
            Console.ReadKey();
            Console.Clear();
        }
    }


    void DeleteUser(UserService userService)
    {
        Console.Clear();
        Console.WriteLine("\nTa bort användare");

        while (true)
        {
            Console.Write("\nAnvändarnamn: ");
            var username = Console.ReadLine();

            bool success = userService.DeleteUser(username);
            if (success)
            {
                Console.WriteLine("\nAnvändare har tagits bort.");
                Console.WriteLine("\nTryck på valfri knapp för att återgå till hvuudmeny...");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            else
            {
                Console.WriteLine("\nAnvändare hittades inte.");
                Console.WriteLine("\nTryck på valfri knapp för att återgå till hvuudmeny...");
                Console.ReadKey();
                Console.Clear();
                return;
            }
        }
    }

    void ShowMenu(User user)
    {
        if (user.Role)
        {
            ShowAdminMenu();
        }
        else
        {
            ShowUserMenu(user);
        }
    }

    void ShowAdminMenu()
    {
        while (true)
        {
            Console.WriteLine("\n1. Lägg till bok");
            Console.WriteLine("2. Ta bort bok");
            Console.WriteLine("3. Uppdatera bokinformation");
            Console.WriteLine("4. Lista alla böcker");
            Console.WriteLine("5. Sök efter bok");
            Console.WriteLine("6. Sortera böcker efter titel");
            Console.WriteLine("7. Logga ut / Återgå till huvudmeny");
            Console.Write("\nVälj ett alternativ: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Console.Write("\nTitel: ");
                    string title = Console.ReadLine();
                    Console.Write("Författare: ");
                    string author = Console.ReadLine();
                    Console.Write("ISBN: ");
                    string isbn = Console.ReadLine();

                    bookService.AddBook(title, author, isbn);
                    Console.WriteLine("\nBoken har lagts till.");
                    Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "2":
                    Console.Clear();
                    Console.Write("\nAnge ISBN för att ta bort bok: ");
                    string isbnToDelete = Console.ReadLine();

                    if (bookService.DeleteBook(isbnToDelete))
                    {
                        Console.WriteLine("\nBoken har tagits bort.");
                    }
                    else
                    {
                        Console.WriteLine("\nBoken kan inte hittas.");
                    }
                    Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "3":
                    Console.Clear();
                    Console.Write("\nAnge ISBN för uppdatering av bok: ");
                    string isbnToUpdate = Console.ReadLine();

                    var book = bookService.GetBookByISBN(isbnToUpdate);
                    if (book == null)
                    {
                        Console.WriteLine("\nBoken kan inte hittas.");
                        Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    }

                    Console.Write("Ny titel: ");
                    string newTitle = Console.ReadLine();
                    Console.Write("Ny författare: ");
                    string newAuthor = Console.ReadLine();

                    if (bookService.UpdateBook(isbnToUpdate, newTitle, newAuthor))
                    {
                        Console.WriteLine("\nBoken har uppdaterats.");
                    }
                    else
                    {
                        Console.WriteLine("\nGick ej att uppdatera boken.");
                    }
                    Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "4":
                    Console.Clear();
                    ListBooks(bookService);
                    break;

                case "5":
                    Console.Clear();
                    SearchBooks(bookService);
                    break;

                case "6":
                    Console.Clear();
                    SortBooks(bookService);
                    break;

                case "7":
                    Console.Clear();
                    return;

                default:
                    Console.WriteLine("\nOgiltigt alternativ, försök igen.");
                    break;
            }
        }
    }

    void ShowUserMenu(User user)
    {
        while (true)
        {
            Console.WriteLine("\n1. Kontrollera bok tillgänglighet");
            Console.WriteLine("2. Låna bok");
            Console.WriteLine("3. Returnera bok");
            Console.WriteLine("4. Lista alla böcker");
            Console.WriteLine("5. Sök efter bok");
            Console.WriteLine("6. Sortera böcker efter titel");
            Console.WriteLine("7. Logga ut / Återgå till huvudmeny");
            Console.Write("\nVälj ett alternativ: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Console.Write("\nAnge ISBN för boken: ");
                    string isbnToCheck = Console.ReadLine();

                    var availability = bookService.IsBookAvailable(isbnToCheck);
                    Console.WriteLine(availability ? "\nBoken är tillgänglig." : "\nBoken är utlånad.");
                    Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "2":
                    Console.Clear();
                    Console.Write("\nAnge ISBN för boken att låna: ");
                    string isbnToLoan = Console.ReadLine();

                    if (bookService.LoanBook(user.Id, isbnToLoan))
                    {
                        Console.WriteLine("\nDu har lånat boken.");
                    }
                    else
                    {
                        Console.WriteLine("\nBoken är redan utlånad eller kunde inte hittas.");
                    }
                    Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "3":
                    Console.Clear();
                    Console.Write("\nAnge ISBN för boken att returnera: ");
                    string isbnToReturn = Console.ReadLine();

                    if (bookService.ReturnBook(user.Id, isbnToReturn))
                    {
                        Console.WriteLine("\nBoken har returnerats.");
                    }
                    else
                    {
                        Console.WriteLine("\nDet gick ej att returnera boken.");
                    }
                    Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "4":
                    Console.Clear();
                    ListBooks(bookService);
                    break;

                case "5":
                    Console.Clear();
                    SearchBooks(bookService);
                    break;

                case "6":
                    Console.Clear();
                    SortBooks(bookService);
                    break;

                case "7":
                    Console.Clear();
                    return;

                default:
                    Console.WriteLine("\nOgiltigt alternativ, försök igen.");
                    break;
            }
        }
    }

    void ListBooks (BookService bookService)
    {
        var books = bookService.GetAllBooks();
        if (books.Count == 0)
        {
            Console.WriteLine("Inga böcker finns tillgängliga.");
        }
        else
        {
            foreach (var book in books)
            {
                Console.WriteLine($"\nTitel: {book.Title} \nFörfattare: {book.Author} \nISBN: {book.ISBN}");
            }
        }
        Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
        Console.ReadKey();
        Console.Clear();
    }

    void SearchBooks (BookService bookService)
    {
        Console.Write("\nAnge sökord för titel eller författare: ");
        var query = Console.ReadLine();
        var books = bookService.SearchBooks(query);

        if (books.Count == 0)
        {
            Console.WriteLine("\nInga böcker matchade din sökning.");
        }
        else
        {
            foreach (var book in books)
            {
                Console.WriteLine($"\nTitel: {book.Title} \nFörfattare: {book.Author} \nISBN: {book.ISBN}");
            }
        }
        Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
        Console.ReadKey();
        Console.Clear();
    }

    void SortBooks (BookService bookService)
    {
        var books = bookService.GetBooksSortedByTitle();
        Console.WriteLine("\nBöcker sorterade efter titel:");
        foreach (var book in books)
        {
            Console.WriteLine($"\nTitel: {book.Title} \nFörfattare: {book.Author} \nISBN: {book.ISBN}");
        }
        Console.WriteLine("\nTryck på valfri knapp för att återgå till meny...");
        Console.ReadKey();
        Console.Clear();
    }
}