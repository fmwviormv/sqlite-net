
```
public class Favorite {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Url { get; set; }
}

public class User {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }

    public Favorite[] GetFavorites(SQLiteConnection c) {

        var q = from f in c.Table<Favorite>()
                where f.UserId == Id
                select f;

        return q.ToArray();
    }

    public void AddFavorite(string url, SQLiteConnection c) {
        var fav = new Favorite() {
            UserId = Id,
            Url = url
        };
        c.Insert(fav);
    }

    public void RemoveFavorite(string url, SQLiteConnection c) {
        var q = from f in c.Table<Favorite>()
                where f.Url == url && f.UserId == Id
                select f;
        var fav = q.FirstOrDefault();
        if (fav != null) {
            c.Delete(fav);
        }
        else {
            throw new Exception("Could not find favorite " + url + " for user " + Username);
        }
    }

    public void UpdatePassword(string newPassword, SQLiteConnection c) {
        PasswordHash = HashPassword(newPassword);
        c.Update(this);        
    }

    public static void UpdatePassword(string username, string oldPassword, string newPassword, SQLiteConnection c) {
        var u = TryAuthenticate(username, oldPassword);
        if (u != null) {
            u.UpdatePassword(newPassword, c);
        }
        else {
            throw new Exception("Could not authenticate " + username);
        }
    }

    public static User TryAuthenticate(string username, string password, SQLiteConnection c) {
      var hash = HashPassword(password);

      var q = from u in db.Table<User>()
              where u.Username == username && PasswordHash == hash
              select u;

      return q.FirstOrDefault();
    }
}
```