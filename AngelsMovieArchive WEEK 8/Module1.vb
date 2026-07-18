Imports System.IO
Imports Newtonsoft.Json

Module Module1

    Dim jsonFile As String

    Sub Main()
        Dim desktopPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        Dim archiveFolder As String = Path.Combine(desktopPath, "AngelsMovieArchive")
        Dim ownedFile As String = Path.Combine(archiveFolder, "OwnedMovies.txt")
        Dim wishlistFile As String = Path.Combine(archiveFolder, "WishlistMovies.txt")
        jsonFile = Path.Combine(archiveFolder, "MovieArchive.json")

        Directory.CreateDirectory(archiveFolder)

        If Not File.Exists(ownedFile) Then
            File.Create(ownedFile).Close()
        End If

        If Not File.Exists(wishlistFile) Then
            File.Create(wishlistFile).Close()
        End If

        If Not File.Exists(jsonFile) Then
            File.WriteAllText(jsonFile, "[]")
        End If

        Dim choice As String = ""

        Do
            Console.Clear()
            Console.WriteLine("ANGEL'S MOVIE ARCHIVE")
            Console.WriteLine("----------------------")
            Console.WriteLine("1. Add owned movie")
            Console.WriteLine("2. Add movie to wishlist")
            Console.WriteLine("3. Search movies")
            Console.WriteLine("4. View all movies")
            Console.WriteLine("5. Exit")
            Console.WriteLine()
            Console.Write("Choose an option: ")
            choice = Console.ReadLine()

            If choice = "1" Then
                AddMovie(ownedFile, "Owned")
            ElseIf choice = "2" Then
                AddMovie(wishlistFile, "Wishlist")
            ElseIf choice = "3" Then
                SearchMovies(ownedFile, wishlistFile)
            ElseIf choice = "4" Then
                ViewAllMovies(ownedFile, wishlistFile)
            ElseIf choice = "5" Then
                Console.WriteLine("Goodbye.")
            Else
                Console.WriteLine("Invalid option. Try again.")
                Pause()
            End If

        Loop Until choice = "5"
    End Sub

    ' Adds movie but then asks for movie information
    ' It then Saves everything with information given.
    ' Also first time we see a sub procedure this just adds something.
    ' Sub = set of commands to execute something but no value is given
    Sub AddMovie(filePath As String, status As String)
        Console.Clear()
        Console.WriteLine("ADD " & status.ToUpper() & " MOVIE")
        Console.WriteLine("----------------------")

        Console.Write("Movie name: ")
        Dim movieName As String = Console.ReadLine()

        Console.Write("Release year: ")
        Dim releaseYear As String = Console.ReadLine()

        Console.Write("Director: ")
        Dim director As String = Console.ReadLine()

        If movieName.Trim() = "" Then
            Console.WriteLine("Movie name cannot be blank.")
            Pause()
            Exit Sub
        End If

        Dim movieLine As String = movieName & " | " & releaseYear & " | " & director & " | " & status
        File.AppendAllText(filePath, movieLine & Environment.NewLine)

        SaveMovieToJson(movieName, releaseYear, director, status)

        Console.WriteLine()
        Console.WriteLine("Movie saved to " & status & ".")
        Console.WriteLine("Movie also saved to MovieArchive.json.")
        Pause()
    End Sub

    ' Saves the movie information into a JSON file.
    ' Serialize means turning the MovieInfo object into JSON text.
    ' Deserialize means reading the old JSON file back into a movie list.
    Sub SaveMovieToJson(movieName As String, releaseYear As String, director As String, status As String)

        Dim movies As List(Of MovieInfo)
        Dim jsonText As String = File.ReadAllText(jsonFile)

        If jsonText.Trim() = "" Then
            movies = New List(Of MovieInfo)()
        Else
            movies = JsonConvert.DeserializeObject(Of List(Of MovieInfo))(jsonText)
        End If

        If movies Is Nothing Then
            movies = New List(Of MovieInfo)()
        End If

        Dim newMovie As New MovieInfo()
        newMovie.MovieName = movieName
        newMovie.ReleaseYear = releaseYear
        newMovie.Director = director
        newMovie.Status = status

        movies.Add(newMovie)

        Dim updatedJson As String = JsonConvert.SerializeObject(movies, Formatting.Indented)

        File.WriteAllText(jsonFile, updatedJson)

    End Sub

    ' no movie found :(
    Sub SearchMovies(ownedFile As String, wishlistFile As String)
        Console.Clear()
        Console.WriteLine("SEARCH MOVIES")
        Console.WriteLine("-------------")
        Console.Write("Search by name, year, or director: ")
        Dim searchText As String = Console.ReadLine().ToLower()

        Console.WriteLine()
        Console.WriteLine("Results:")
        Console.WriteLine("--------")

        Dim found As Boolean = False

        found = SearchFile(ownedFile, searchText) Or found
        found = SearchFile(wishlistFile, searchText) Or found

        If found = False Then
            Console.WriteLine("No movies found.")
        End If

        Pause()
    End Sub

    ' This goes well if the information of the movie was put correctly.
    ' True means the movie is found
    ' Function time!! now this gives the value the Sub couldnt 
    Function SearchFile(filePath As String, searchText As String) As Boolean
        Dim found As Boolean = False
        Dim lines() As String = File.ReadAllLines(filePath)

        For Each line As String In lines
            If line.ToLower().Contains(searchText) Then
                Console.WriteLine(line)
                found = True
            End If
        Next

        Return found
    End Function

    ' Console lines just print text on console screen by the way
    Sub ViewAllMovies(ownedFile As String, wishlistFile As String)
        Console.Clear()
        Console.WriteLine("ALL MOVIES")
        Console.WriteLine("----------")

        Console.WriteLine()
        Console.WriteLine("Owned Movies:")
        Console.WriteLine("-------------")
        ShowFile(ownedFile)

        Console.WriteLine()
        Console.WriteLine("Wishlist Movies:")
        Console.WriteLine("----------------")
        ShowFile(wishlistFile)

        Pause()
    End Sub

    Sub ShowFile(filePath As String)
        Dim lines() As String = File.ReadAllLines(filePath)

        If lines.Length = 0 Then
            Console.WriteLine("No movies saved yet.")
        Else
            For Each line As String In lines
                Console.WriteLine(line)
            Next
        End If
    End Sub

    Sub Pause()
        Console.WriteLine()
        Console.WriteLine("Press ENTER to continue...")
        Console.ReadLine()
    End Sub

End Module