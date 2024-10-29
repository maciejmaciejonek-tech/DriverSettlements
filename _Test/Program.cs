// See https://aka.ms/new-console-template for more information

using DriverSettlements.ExternalServicesCrowler;

var thread = new Thread(() =>
{
    var test = new Test();
    test.Execute();
});

thread.SetApartmentState(ApartmentState.STA);
thread.Start();
while (true)
{
    Thread.Sleep(1000);
}

Console.WriteLine("Hello, World!");
