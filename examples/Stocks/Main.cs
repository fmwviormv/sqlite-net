using System;
using System.Collections.Generic;
using System.Linq;
using Path = System.IO.Path;

using SQLite;

namespace Stocks
{
	class Valuation
	{
		[PrimaryKey]
		public int Id { get; set; }
		[Indexed]
		public int StockId { get; set; }
		public DateTime Time { get; set; }
		public decimal Price { get; set; }
	}

	class Stock
	{
		[PrimaryKey]
		public int Id { get; set; }
		[MaxLength(8)]
		public string Symbol { get; set; }

		public override string ToString ()
		{
			return Symbol;
		}
	}

	class Database : SQLiteConnection
	{
		public Database (string path) : base(path)
		{
		}
		public IEnumerable<Valuation> QueryValuations (Stock stock)
		{
			return Query<Valuation> ("select * from Valuation where StockId = ?", stock.Id);
		}
		public Valuation QueryLatestValuation (Stock stock)
		{
			return Query<Valuation> ("select * from Valuation where StockId = ? order by Time desc limit 1", stock.Id).FirstOrDefault ();
		}
		public Stock QueryStock (string stockSymbol)
		{
			return Query<Stock> ("select * from Stock where Symbol = ?", stockSymbol).FirstOrDefault ();
		}
		public IEnumerable<Stock> QueryAllStocks ()
		{
			return Query<Stock> ("select * from Stock order by Symbol");
		}
	}

	class Program
	{
		public static void Main (string[] args)
		{
			new Program ().Run ();
		}

		Database _db;

		void DisplayStock (string stockSymbol)
		{
			var stock = _db.QueryStock (stockSymbol);
			
			if (stock == null) {
				Console.WriteLine ("I don't know about {0}", stockSymbol);
				Console.WriteLine ("Run \"up {0}\" to update the stock", stockSymbol);
			} else {
				
				//
				// Display the last 1 week
				//				
				foreach (var v in _db.QueryValuations (stock)) {
					Console.WriteLine ("  {0}", v);
				}
				
			}
		}

		void UpdateStock (string stockSymbol)
		{
			//
			// Ensure that there is a valid Stock in the DB
			//
			var stock = _db.QueryStock (stockSymbol);
			if (stock == null) {
				stock = new Stock { Symbol = stockSymbol };
			}
			
			//
			// When was it last valued?
			//
			var latest = _db.QueryLatestValuation (stock);
			var latestDate = latest != null ? latest.Time : new DateTime (1950, 1, 1);
			
			//
			// Get the latest valuations
			//
			var newVals = new YahooScraper ().GetValuations (stock, latestDate + TimeSpan.FromHours (23), DateTime.Now);
			foreach (var v in newVals) {
				_db.Insert (v);
			}
		}

		void ListStocks ()
		{
			foreach (var stock in _db.QueryAllStocks ()) {
				Console.WriteLine (stock);
			}
		}

		void DisplayBanner ()
		{
			Console.WriteLine ("Stocks - a demo of sqlite-net");
			Console.WriteLine ("Using {0}", _db.Database);
		}

		void DisplayHelp (string cmd)
		{
			Action<string, string> display = (c, h) => { Console.WriteLine ("{0} {1}", c, h); };
			var cmds = new SortedDictionary<string, string> {
				{
					"ls",
					"\t List all known stocks"
				},
				{
					"exit",
					"\t Exit stocks"
				},
				{
					"up stock",
					" Updates stock"
				},
				{
					"help",
					"\t Displays help"
				},
				{
					"stock",
					"\t Displays latest valuations for stock"
				}
			};
			if (cmds.ContainsKey (cmd)) {
				display (cmd, cmds[cmd]);
			} else {
				foreach (var ch in cmds) {
					display (ch.Key, ch.Value);
				}
			}
		}

		void Run ()
		{
			var WS = new char[] {
				' ',
				'\t',
				'\r',
				'\n'
			};
			
			var dbPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "Stocks.db");
			
			_db = new Database (dbPath);
			_db.CreateTable<Stock> ();
			_db.CreateTable<Valuation> ();
			
			
			DisplayBanner ();
			DisplayHelp ("");
			
			for (;;) {
				Console.Write ("$ ");
				var cmdline = Console.ReadLine ();
				
				var args = cmdline.Split (WS, StringSplitOptions.RemoveEmptyEntries);
				if (args.Length < 1)
					continue;
				var cmd = args[0].ToLowerInvariant ();
				
				if (cmd == "?" || cmd == "help") {
					DisplayHelp ("");
				} else if (cmd == "ls") {
					ListStocks ();
				} else if (cmd == "up") {
					if (args.Length == 2) {
						UpdateStock (args[1].ToUpperInvariant ());
					} else {
						DisplayHelp ("up stock");
					}
				} else {
					DisplayStock (cmd.ToUpperInvariant ());
				}
			}
		}
	}

	class YahooScraper
	{
		public IEnumerable<Valuation> GetValuations (Stock stock, DateTime start, DateTime end)
		{
			throw new NotImplementedException ();
		}
	}
}