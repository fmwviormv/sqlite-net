sqlite-net is designed to make working with sqlite very easy in a .NET environment.

The library contains simple attributes that you can use to control the construction of tables. In a simple stock program, you might use:

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
var db = new SQLiteConnection("foofoo");
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

The generic parameter to the `Query` method specifies the type of object to create for each row. It can be one of your table classes, or anyother class whose public properties match the column returned by the query. For instance, we could rewrite the above query as:

```
public class Val {
	public decimal Money { get; set; }
	public DateTime Date { get; set; }
}
public static IEnumerable<Val> QueryVals (SQLiteConnection db, Stock stock)
{
	return db.Query<Val> ("select 'Price' as 'Money', 'Time' as 'Date' from Valuation where StockId = ?", stock.Id);
}
```

Updates must be performed manually using the `Execute` method of `SQLiteConnection`.