using BCrypt.Net;

string password = "Editor123";
string hash = BCrypt.Net.BCrypt.HashPassword(password, 11); 
Console.WriteLine(hash);
