### How do I link the sqlite3 binaries into my MonoTouch project? ###

You don't have to do anything because the iPhone SDK already includes the sqlite3 binary and Mono will handle the hard work of linking to that library. Just add SQLite.cs to your project, and you should be all set.


### The database is being closed before the Query is finished. What's up? ###

The problem is that queries runs lazily - the connection needs to be open iterating the results of the query.

If you want to close the DB right after the query has finished, simply call .ToList() or .ToArray() to force the query to be evaluated, then close the connection.


