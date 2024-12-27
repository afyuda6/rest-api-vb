Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Hosting

Public Module MainModule
    Public Sub Main(args As String())
        InitializeDatabase()
        Dim port As Integer = If(Environment.GetEnvironmentVariable("PORT"), 6013)
        CreateHostBuilder(args, port).Build().Run()
    End Sub

    Private Function CreateHostBuilder(args As String(), port As Integer) As IHostBuilder
        Return Host.CreateDefaultBuilder(args).
            ConfigureWebHostDefaults(
                Sub(webBuilder)
                    webBuilder.UseKestrel()
                    webBuilder.UseUrls($"http://0.0.0.0:{port}")
                    webBuilder.Configure(Sub(app) ConfigureApp(app))
                    webBuilder.ConfigureServices(Sub(services) ConfigureServices(services))
                End Sub)
    End Function
End Module