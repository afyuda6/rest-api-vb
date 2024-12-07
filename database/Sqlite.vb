Imports System.Data.SQLite

Public Module Sqlite
    Private ReadOnly _connectionString As String = "Data Source=rest_api_cs.db;Version=3;"

    Public Function Connect() As SQLiteConnection
        Dim connection As New SQLiteConnection(_connectionString)
        connection.Open()
        Return connection
    End Function

    Public Sub InitializeDatabase()
        Using connection = Connect()
            Dim command As New SQLiteCommand("DROP TABLE IF EXISTS users;", connection)
            command.ExecuteNonQuery()

            command = New SQLiteCommand("CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY, name TEXT NOT NULL);", connection)
            command.ExecuteNonQuery()
        End Using
    End Sub
End Module
