Imports System.Data.SQLite
Imports System.Net
Imports System.Text
Imports System.Web
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.DependencyInjection

Public Module User
    Public Async Function HandleReadUsers(response As HttpResponse) As Task
        Dim users As New List(Of Object)
        Using connection = Connect()
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
        response.ContentLength = buffer.Length
        Await response.Body.WriteAsync(buffer, 0, buffer.Length)
    End Function

    Private Async Function HandleCreateUser(request As HttpRequest, response As HttpResponse) As Task
        Dim requestBody = Await New IO.StreamReader(request.Body).ReadToEndAsync()
        Dim userData = HttpUtility.ParseQueryString(requestBody)
        If String.IsNullOrWhiteSpace(userData("name")) Then
            response.StatusCode = CInt(HttpStatusCode.BadRequest)
            response.ContentType = "application/json"
            Dim errorResponse = New With {
                .status = "Bad Request",
                .code = 400,
                .errors = "Missing 'name' parameter"
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(errorResponse)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength = buffer.Length
            Await response.Body.WriteAsync(buffer, 0, buffer.Length)
        Else
            Using connection = Connect()
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
            response.ContentLength = buffer.Length
            Await response.Body.WriteAsync(buffer, 0, buffer.Length)
        End If
    End Function

    Private Async Function HandleUpdateUser(request As HttpRequest, response As HttpResponse) As Task
        Dim requestBody = Await New IO.StreamReader(request.Body).ReadToEndAsync()
        Dim userData = HttpUtility.ParseQueryString(requestBody)
        If String.IsNullOrWhiteSpace(userData("name")) OrElse String.IsNullOrWhiteSpace(userData("id")) Then
            response.StatusCode = CInt(HttpStatusCode.BadRequest)
            response.ContentType = "application/json"
            Dim errorResponse = New With {
                .status = "Bad Request",
                .code = 400,
                .errors = "Missing 'id' or 'name' parameter"
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(errorResponse)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength = buffer.Length
            Await response.Body.WriteAsync(buffer, 0, buffer.Length)
        Else
            Using connection = Connect()
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
            response.ContentLength = buffer.Length
            Await response.Body.WriteAsync(buffer, 0, buffer.Length)
        End If
    End Function

    Private Async Function HandleDeleteUser(request As HttpRequest, response As HttpResponse) As Task
        Dim requestBody = Await New IO.StreamReader(request.Body).ReadToEndAsync()
        Dim userData = HttpUtility.ParseQueryString(requestBody)
        If String.IsNullOrWhiteSpace(userData("id")) Then
            response.StatusCode = CInt(HttpStatusCode.BadRequest)
            response.ContentType = "application/json"
            Dim errorResponse = New With {
                .status = "Bad Request",
                .code = 400,
                .errors = "Missing 'id' parameter"
            }
            Dim jsonResponse = Json.JsonSerializer.Serialize(errorResponse)
            Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
            response.ContentLength = buffer.Length
            Await response.Body.WriteAsync(buffer, 0, buffer.Length)
        Else
            Using connection = Connect()
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
            response.ContentLength = buffer.Length
            Await response.Body.WriteAsync(buffer, 0, buffer.Length)
        End If
    End Function

    Public Sub ConfigureApp(app As IApplicationBuilder)
        app.UseCors("AllowSpecificOrigins")
        app.UseRouting()
        app.UseEndpoints(Sub(endpoints)
            endpoints.Map("/users", Async Function(context As HttpContext)
                Select Case context.Request.Method.ToUpper()
                    Case "GET"
                        Await HandleReadUsers(context.Response)
                    Case "POST"
                        Await HandleCreateUser(context.Request, context.Response)
                    Case "PUT"
                        Await HandleUpdateUser(context.Request, context.Response)
                    Case "DELETE"
                        Await HandleDeleteUser(context.Request, context.Response)
                    Case "OPTIONS"
                        context.response.StatusCode = CInt(HttpStatusCode.OK)
                        context.response.ContentType = "application/json"
                        Dim responseBody = ""
                        Dim jsonResponse = Json.JsonSerializer.Serialize(responseBody)
                        Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
                        context.Response.ContentLength = buffer.Length
                        Await context.Response.Body.WriteAsync(buffer, 0, buffer.Length)
                    Case Else
                        context.response.StatusCode = CInt(HttpStatusCode.MethodNotAllowed)
                        context.response.ContentType = "application/json"
                        Dim responseBody = New With {
                                .status = "Method Not Allowed",
                                .code = 405
                                }
                        Dim jsonResponse = Json.JsonSerializer.Serialize(responseBody)
                        Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
                        context.Response.ContentLength = buffer.Length
                        Await context.Response.Body.WriteAsync(buffer, 0, buffer.Length)
                End Select
            End Function)
            endpoints.MapFallback(Async Function(context)
                context.response.StatusCode = CInt(HttpStatusCode.NotFound)
                context.response.ContentType = "application/json"
                Dim responseBody = New With {
                        .status = "Not Found",
                        .code = 404
                        }
                Dim jsonResponse = Json.JsonSerializer.Serialize(responseBody)
                Dim buffer = Encoding.UTF8.GetBytes(jsonResponse)
                context.Response.ContentLength = buffer.Length
                Await context.Response.Body.WriteAsync(buffer, 0, buffer.Length)
            End Function)
        End Sub)
    End Sub
    
    Public Sub ConfigureServices(services As IServiceCollection)
        services.AddCors(Sub(options)
            options.AddPolicy("AllowSpecificOrigins",
                Sub(policy)
                    policy.AllowAnyOrigin().WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS").WithHeaders("Content-Type")
                End Sub)
        End Sub)
        services.AddRouting()
    End Sub
End Module
