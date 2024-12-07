Imports System.Data.SQLite
Imports System.Net
Imports System.Text
Imports System.Web

Public Module User
    Public Async Function UserHandle(request As HttpListenerRequest, response As HttpListenerResponse) As Task
        Select Case request.HttpMethod
            Case "GET"
                Await HandleReadUsers(response)
            Case "POST"
                Await HandleCreateUser(request, response)
            Case "PUT"
                Await HandleUpdateUser(request, response)
            Case "DELETE"
                Await HandleDeleteUser(request, response)
            Case Else
                response.StatusCode = CInt(HttpStatusCode.MethodNotAllowed)
                response.ContentType = "application/json"
                Dim errorResponse = New With {
                    .status = "Method Not Allowed",
                    .code = 405
                }
                Dim jsonResponse = Json.JsonSerializer.Serialize(errorResponse)
                Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
                response.ContentLength64 = buffer.Length
                Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
                response.OutputStream.Close()
        End Select
    End Function

    Private Async Function HandleReadUsers(response As HttpListenerResponse) As Task
        Dim users As New List(Of Object)
        Using connection = Sqlite.Connect()
            Dim command As New SQLiteCommand("SELECT id, name FROM users;", connection)
            Using reader = command.ExecuteReader()
                While reader.Read()
                    users.Add(New With {
                        .id = reader("id"),
                        .name = reader("name")
                    })
                End While
            End Using
        End Using

        response.StatusCode = CInt(HttpStatusCode.OK)
        response.ContentType = "application/json"
        Dim responseBody = New With {
            .status = "OK",
            .code = 200,
            .data = users
        }
        Dim jsonResponse = Json.JsonSerializer.Serialize(responseBody)
        Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
        response.ContentLength64 = buffer.Length
        Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
        response.OutputStream.Close()
    End Function

    Private Async Function HandleCreateUser(request As HttpListenerRequest, response As HttpListenerResponse) As Task
        Dim requestBody = Await New IO.StreamReader(request.InputStream).ReadToEndAsync()
        Dim userData = HttpUtility.ParseQueryString(requestBody)

        If userData("name") Is Nothing Then
            response.StatusCode = CInt(HttpStatusCode.BadRequest)
            response.ContentType = "application/json"
            Dim errorResponse = New With {
                .status = "Bad Request",
                .code = 400,
                .errors = "Missing 'name' parameter"
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(errorResponse)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength64 = buffer.Length
            Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
            response.OutputStream.Close()
        Else
            Using connection = Sqlite.Connect()
                Dim command As New SQLiteCommand("INSERT INTO users (name) VALUES (@Name);", connection)
                command.Parameters.AddWithValue("@Name", userData("name"))
                command.ExecuteNonQuery()
            End Using

            response.StatusCode = CInt(HttpStatusCode.Created)
            response.ContentType = "application/json"
            Dim responseBody = New With {
                .status = "Created",
                .code = 201
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(responseBody)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength64 = buffer.Length
            Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
            response.OutputStream.Close()
        End If
    End Function

    Private Async Function HandleUpdateUser(request As HttpListenerRequest, response As HttpListenerResponse) As Task
        Dim requestBody = Await New IO.StreamReader(request.InputStream).ReadToEndAsync()
        Dim userData = HttpUtility.ParseQueryString(requestBody)

        If userData("name") Is Nothing OrElse userData("id") Is Nothing Then
            response.StatusCode = CInt(HttpStatusCode.BadRequest)
            response.ContentType = "application/json"
            Dim errorResponse = New With {
                .status = "Bad Request",
                .code = 400,
                .errors = "Missing 'id' or 'name' parameter"
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(errorResponse)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength64 = buffer.Length
            Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
            response.OutputStream.Close()
        Else
            Using connection = Sqlite.Connect()
                Dim command As New SQLiteCommand("UPDATE users SET name = @Name WHERE id = @Id;", connection)
                command.Parameters.AddWithValue("@Name", userData("name"))
                command.Parameters.AddWithValue("@Id", userData("id"))
                command.ExecuteNonQuery()
            End Using

            response.StatusCode = CInt(HttpStatusCode.OK)
            response.ContentType = "application/json"
            Dim responseBody = New With {
                .status = "OK",
                .code = 200
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(responseBody)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength64 = buffer.Length
            Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
            response.OutputStream.Close()
        End If
    End Function

    Private Async Function HandleDeleteUser(request As HttpListenerRequest, response As HttpListenerResponse) As Task
        Dim requestBody = Await New IO.StreamReader(request.InputStream).ReadToEndAsync()
        Dim userData = HttpUtility.ParseQueryString(requestBody)

        If userData("id") Is Nothing Then
            response.StatusCode = CInt(HttpStatusCode.BadRequest)
            response.ContentType = "application/json"
            Dim errorResponse = New With {
                .status = "Bad Request",
                .code = 400,
                .errors = "Missing 'id' parameter"
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(errorResponse)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength64 = buffer.Length
            Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
            response.OutputStream.Close()
        Else
            Using connection = Sqlite.Connect()
                Dim command As New SQLiteCommand("DELETE FROM users WHERE id = @Id;", connection)
                command.Parameters.AddWithValue("@Id", userData("id"))
                command.ExecuteNonQuery()
            End Using

            response.StatusCode = CInt(HttpStatusCode.OK)
            response.ContentType = "application/json"
            Dim responseBody = New With {
                .status = "OK",
                .code = 200
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(responseBody)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength64 = buffer.Length
            Await response.OutputStream.WriteAsync(buffer, 0, buffer.Length)
            response.OutputStream.Close()
        End If
    End Function
End Module
