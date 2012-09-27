Gatherer Ripper
===============

Gatherer ripper is a C# solution including two main components:

  - A library (MagicRipper) which provides a way to enumerate the sets and cards in the Gatherer database, and
  - A simple WPF interface (GathererRipper) for downloading the whole Gatherer into a SQLite database (its content, if you're curious, is about 50 Megabytes and it takes some 48 hours to completely download it)
  
The library MagicRipper's entry points are the class MagicRipper.Ripper and its methods GetExpansions and GetCards. See the included documentation help file.