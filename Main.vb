Imports System.Net
Imports System.Threading.Tasks

Public Module MainModule
    Private _listener As HttpListener
    Private _baseUrl As String = "http://localhost:6013/"

    Public Sub Main()
        InitializeDatabase()
        _listener = New HttpListener()
        _listener.Prefixes.Add(_baseUrl)
        _listener.Start()
        Task.Run(AddressOf ListenForRequests)
        Console.ReadLine()
    End Sub

    Private Async Function ListenForRequests() As Task
        While True
            Dim context = Await _listener.GetContextAsync()
            Await UserHandle(context)
        End While
    End Function
End Module
