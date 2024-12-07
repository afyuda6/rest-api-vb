Imports System.Net
Imports System.Threading.Tasks

Public Module MainModule
    Private _listener As HttpListener
    Private _baseUrl As String = "http://localhost:6013/"

    Public Sub Main()
        Sqlite.InitializeDatabase()
        _listener = New HttpListener()
        _listener.Prefixes.Add(_baseUrl)
        _listener.Start()
        Task.Run(AddressOf ListenForRequests)

        Console.ReadLine()
    End Sub
    
    Private Async Sub WriteResponseAsync(response As HttpListenerResponse, buffer As Byte())
        Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
        response.OutputStream.Close()
    End Sub

    Private Async Function ListenForRequests() As Task
        While True
            Dim context = Await _listener.GetContextAsync()
            Dim request = context.Request
            Dim response = context.Response

            Try
                If request.Url.AbsolutePath.Equals("/users") OrElse request.Url.AbsolutePath.Equals("/users/") Then
                    Await User.UserHandle(request, response)
                Else
                    response.StatusCode = CInt(HttpStatusCode.NotFound)
                    response.ContentType = "application/json"
                    Dim errorResponse = New With {
                        .status = "Not Found",
                        .code = 404
                    }
                    Dim jsonResponse = Text.Json.JsonSerializer.Serialize(errorResponse)
                    Dim buffer = Text.Encoding.UTF8.GetBytes(jsonResponse)
                    response.ContentLength64 = buffer.Length
                    Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
                    response.OutputStream.Close()
                End If
            Catch ex As Exception
                response.StatusCode = CInt(HttpStatusCode.InternalServerError)
                response.ContentType = "application/json"
                Dim errorResponse = New With {
                        .status = "Internal Server Error",
                        .code = 500
                        }
                Dim jsonResponse = Text.Json.JsonSerializer.Serialize(errorResponse)
                Dim buffer = Text.Encoding.UTF8.GetBytes(jsonResponse)
                response.ContentLength64 = buffer.Length
                WriteResponseAsync(response, buffer)
            End Try
        End While
    End Function
End Module
