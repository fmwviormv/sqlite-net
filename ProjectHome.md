# MOVED TO GITHUB #

sqlite-net is now hosted on github at http://github.com/praeclarum/sqlite-net. Please use that code as it has many advancements over this legacy code. Thanks!

-Frank

# OLD CONTENT #

sqlite-net is an open source, minimal library to allow .NET and Mono applications to store data in [SQLite 3 databases](http://www.sqlite.org). It is written in C# 3.0 and is meant to be simply compiled in with your projects. It was first designed to work with [MonoTouch](http://monotouch.net/) on the iPhone, but should work in any other CLI environment.

sqlite-net was designed as a quick and convenient database layer. Its design follows from these **goals**:

  * Very easy to integrate with existing projects and with MonoTouch projects.

  * Fast and efficient.

  * Methods for executing queries safely (using parameters) and for retrieving the results of those query in a strongly typed fashion.

  * Linq support so that you don't have to write SQL (see LinqSupport).

  * Works with your data model without forcing you to change your classes. (Contains a small reflection-driven ORM layer.)

  * It has 0 dependencies aside from a [compiled form of the sqlite3 library](http://www.sqlite.org/download.html).

**Non-goals** include:

  * Not an implementation of `IDbConnection` and its family. This is not a full SQLite driver. If you need that, go get [System.Data.SQLite](http://sqlite.phxsoftware.com/) or [csharp-sqlite](http://code.google.com/p/csharp-sqlite/).

There is a port of the library to work with the **Compact Framework** thanks to Steve Wagner. You can find his library, [sqlite-net-compactframework](http://github.com/lanwin/sqlite-net-compactframework), at github.

The design is similar to that used by Demis Bellot in the [OrmLite sub project of ServiceStack](http://code.google.com/p/servicestack/source/browse/#svn/trunk/Common/ServiceStack.Common/ServiceStack.OrmLite). There is another SQLite ORM project that work with MonoTouch, [catnap-orm](http://code.google.com/p/catnap-orm/wiki/Introduction) by Tim Scott, with similar goals.


---


The library contains a few attributes that you can use to control the construction of tables. In a simple stock program, you might use:

```
public class Stock
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	[MaxLength(8)]
	public string Symbol { get; set; }
}

public class Valuation
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	[Indexed]
	public int StockId { get; set; }
	public DateTime Time { get; set; }
	public decimal Price { get; set; }
}
```

With these, you can automatically generate tables in your database by calling `CreateTable`:

```
var db = new SQLiteConnection("stocks.db");
db.CreateTable<Stock>();
db.CreateTable<Valuation>();
```

You can insert rows in the database using `Insert`. If the table contains an auto-incremented primary key, then the value for that key will be available to you after the insert:

```
public static void AddStock(SQLiteConnection db, string symbol) {
	var s = db.Insert(new Stock() {
		Symbol = symbol
	});
	Console.WriteLine("{0} == {1}", s.Symbol, s.Id);
}
```

You can query the database using the `Query` method of `SQLiteConnection`:

```
public static IEnumerable<Valuation> QueryValuations (SQLiteConnection db, Stock stock)
{
	return db.Query<Valuation> ("select * from Valuation where StockId = ?", stock.Id);
}
```

this query can also be written using Linq syntax so that you type safety and so that your refactors are safe:

```
public static IEnumerable<Valuation> QueryValuations (SQLiteConnection db, Stock stock)
{
	return db.Table<Valuation> ().Where(v => v.StockId == stock.Id);
}
```

The generic parameter to the `Query` method specifies the type of object to create for each row. It can be one of your table classes, or anyother class whose public properties match the column returned by the query. The `Table` method requires that a table exists with the same name as the type being returned.

```
public class Val {
	public decimal Money { get; set; }
	public DateTime Date { get; set; }
}
public static IEnumerable<Val> QueryVals (SQLiteConnection db, Stock stock)
{
	return db.Query<Val> ("select \"Price\" as \"Money\", \"Time\" as \"Date\" from Valuation where StockId = ?", stock.Id);
}
```

Updates are performed in whole using the `Update` method of `SQLiteConnection`.


---


This is an open source project that welcomes contributions/suggestions/bug reports from those who use it. If you have any ideas on how to improve the library, please contact [Frank Krueger](mailto:fak@praeclarum.org).

