Imports System.Net
Imports System.Threading.Tasks

Public Module MainModule
    Private listener As HttpListener
    Private baseUrl As String = "http://localhost:6013/"

    Public Sub Main()
        InitializeDatabase()
        listener = New HttpListener()
        listener.Prefixes.Add(baseUrl)
        listener.Start()
        Task.Run(AddressOf ListenForRequests)
        Console.ReadLine()
    End Sub

    Private Async Function ListenForRequests() As Task
        While True
            Await UserHandle(listener)
        End While
    End Function
End Module
